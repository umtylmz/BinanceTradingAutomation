// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AzureTest.DataAccessLayer.Concrete;
using Binance.Spot.Models;
using Domain.Abstract;
using Domain.Enum;
using Domain.Interface;
using Domain.Model;
using Domain.Model.BinanceResponse;
using Domain.Test.Model;
using Helper;
using Newtonsoft.Json;

namespace AzureTest.Job
{
    internal class IsolatedMarginV2 : TaskAbstract
    {
        public IsolatedMarginV2() : base()
        {
            _isolatedMarginV2TaskDetailDAL = new();
        }

        private IsolatedMarginWallet Wallet { get; set; }
        private IsolatedMarginV2TaskDetail TaskDetail { get; set; }
        private readonly IsolatedMarginV2TaskDetailDAL _isolatedMarginV2TaskDetailDAL;

        internal async Task Execute(string taskName, SymbolTypeEnum symbolType)
        {
            try
            {
                await CreateTaskDetailIfNotExists(taskName, symbolType);
                CreateWalletIfNotExists();

                string binanceResult;
                MarginAccountNewOrderResponse newOrderResponse;
                List<QueryMarginAccountsOpenOrdersResponse> openOrdersResponse;
                switch (TaskDetail.TradingStatus)
                {
                    case IsolatedMarginV1StatusEnum.WaitingForStartOrder:
                        SetInitialBuyingAmount();

                        #region Gerekiyorsa borç alma işlemi yapılır
                        if (TaskDetail.InitialBuyingAmount > Wallet.Assets.First().QuoteAsset.Free)
                        {
                            decimal borrowAmount = TaskDetail.InitialBuyingAmount - Math.Round(Wallet.Assets.First().QuoteAsset.Free, 2, MidpointRounding.ToZero);
                            await MarginAccountTradeBinanceService.MarginAccountBorrow(Wallet.Assets.First().QuoteAsset.Asset.ToString(), borrowAmount, true, Wallet.Assets.First().Symbol.ToString(), 10000);

                            LogHelper.LogMessage($"1. seviye satınalma işlemi için {borrowAmount} USDT tutarında borç alındı.", MessageTypeEnum.BORROW);

                            UpdateWalletData();
                        }
                        #endregion

                        #region İlk satınalma işlemi yapılır
                        binanceResult = MarginAccountTradeBinanceService.MarginAccountNewOrder(symbolType.ToString(), Side.BUY, OrderType.MARKET, TaskDetail.IsIsolated, quoteOrderQty: TaskDetail.InitialBuyingAmount, recvWindow: TaskDetail.RecvWindow).Result;
                        newOrderResponse = JsonConvert.DeserializeObject<MarginAccountNewOrderResponse>(binanceResult);

                        #region Task detayları güncellenir
                        TaskDetail.BuyingPrice = newOrderResponse.Fills.Sum(a => a.Price * a.Qty) / newOrderResponse.Fills.Sum(a => a.Qty);
                        TaskDetail.TotalPayment = newOrderResponse.CummulativeQuoteQty;
                        TaskDetail.LossLayer = 1;
                        await UpdateTaskDetail();
                        #endregion

                        LogHelper.LogMessage($"{TaskDetail.BuyingPrice} ortalama fiyatı ile 1. seviye satınalma yapıldı.", MessageTypeEnum.BUY);

                        UpdateWalletData();
                        #endregion

                        #region 1. seviyeden Kâr alma için satış emri verilir
                        //decimal sellingPrice = Math.Round(TaskDetail.BuyingPrice * 1.01m, 2, MidpointRounding.ToEven);
                        decimal sellingPrice = Math.Round(TaskDetail.BuyingPrice + 100, 2, MidpointRounding.ToEven);
                        binanceResult = MarginAccountTradeBinanceService.MarginAccountNewOrder(symbolType.ToString(), Side.SELL, OrderType.LIMIT, TaskDetail.IsIsolated, quantity: Math.Round(Wallet.Assets[0].BaseAsset.Free, 5, MidpointRounding.ToZero), price: sellingPrice, timeInForce: TimeInForce.GTC, recvWindow: TaskDetail.RecvWindow).Result;
                        newOrderResponse = JsonConvert.DeserializeObject<MarginAccountNewOrderResponse>(binanceResult);

                        LogHelper.LogMessage($"{sellingPrice} fiyatı ile 1. seviyede kâr alma talimatı verildi.", MessageTypeEnum.SELL);

                        TaskDetail.SellingOrderID = newOrderResponse.OrderId;
                        await UpdateTaskDetail();

                        UpdateWalletData();
                        #endregion

                        #region 2. seviye alış işlemi için gerekiyorsa borç alınır
                        decimal buyingAmountFor2thLevel = TaskDetail.InitialBuyingAmount * 2;
                        if (buyingAmountFor2thLevel > Wallet.Assets.First().QuoteAsset.Free)
                        {
                            decimal borrowAmount = buyingAmountFor2thLevel - Math.Round(Wallet.Assets.First().QuoteAsset.Free, 2, MidpointRounding.ToZero);
                            await MarginAccountTradeBinanceService.MarginAccountBorrow(Wallet.Assets.First().QuoteAsset.Asset.ToString(), borrowAmount, true, Wallet.Assets.First().Symbol.ToString(), 10000);

                            LogHelper.LogMessage($"2. seviye satınalma işlemi için {borrowAmount} USDT tutarında borç alındı.", MessageTypeEnum.BORROW);

                            UpdateWalletData();
                        }
                        #endregion

                        #region 2. seviye alış emri verilir
                        decimal buyingPrice = Math.Round(TaskDetail.BuyingPrice * 0.97m, 2, MidpointRounding.ToEven);
                        decimal buyingQuantityFor2thLevel = Math.Round(buyingAmountFor2thLevel / buyingPrice, 5, MidpointRounding.ToZero);
                        binanceResult = MarginAccountTradeBinanceService.MarginAccountNewOrder(symbolType.ToString(), Side.BUY, OrderType.LIMIT, TaskDetail.IsIsolated, quantity: buyingQuantityFor2thLevel, price: buyingPrice, timeInForce: TimeInForce.GTC, recvWindow: TaskDetail.RecvWindow).Result;
                        newOrderResponse = JsonConvert.DeserializeObject<MarginAccountNewOrderResponse>(binanceResult);

                        LogHelper.LogMessage($"{buyingPrice} fiyatı ile {buyingQuantityFor2thLevel} adet 2. seviye satın alma talimatı verildi.", MessageTypeEnum.BUY);

                        TaskDetail.BuyingOrderId = newOrderResponse.OrderId;
                        await UpdateTaskDetail();

                        UpdateWalletData();
                        #endregion

                        TaskDetail.TradingStatus = IsolatedMarginV1StatusEnum.Started;
                        await UpdateTaskDetail();
                        break;
                    case IsolatedMarginV1StatusEnum.Started:
                        binanceResult = MarginAccountTradeBinanceService.QueryMarginAccountsOpenOrders(Wallet.Assets[0].Symbol.ToString(), true, recvWindow: TaskDetail.RecvWindow).Result;
                        openOrdersResponse = JsonConvert.DeserializeObject<List<QueryMarginAccountsOpenOrdersResponse>>(binanceResult);

                        if (TaskDetail.LossLayer < 8 && (!openOrdersResponse.Any(a => a.OrderId == TaskDetail.BuyingOrderId)
                            || !openOrdersResponse.Any(a => a.OrderId == TaskDetail.SellingOrderID)))
                        {
                            //open order yoksa cancel fonksiyonu hata veriyor, open order sayısına göre if şartı koyulabilir
                            await MarginAccountTradeBinanceService.MarginAccountCancelAllOpenOrdersOnASymbol(Wallet.Assets[0].Symbol.ToString(), true, TaskDetail.RecvWindow);

                            LogHelper.LogMessage($"Bir emrin gerçekleşmesi sebebiyle emirlerden çıkıldı.", MessageTypeEnum.INFO);

                            UpdateWalletData();
                        }
                        else if (TaskDetail.LossLayer == 8 && !openOrdersResponse.Any(a => a.OrderId == TaskDetail.SellingOrderID))
                        {
                            LogHelper.LogMessage($"Kâr alma sonrası işlemlere devam ediliyor...", MessageTypeEnum.INFO);
                            await Repay();
                            TaskDetail.TradingStatus = IsolatedMarginV1StatusEnum.Finished;
                            await UpdateTaskDetail();
                            break;
                        }
                        else
                            break;

                        binanceResult = MarginAccountTradeBinanceService.QueryMarginAccountsAllOrders(Wallet.Assets[0].Symbol.ToString(), true, Convert.ToInt64(TaskDetail.SellingOrderID), recvWindow: TaskDetail.RecvWindow).Result;

                        QueryMarginAccountsOpenOrdersResponse sellingPositionOldOrder = JsonConvert.DeserializeObject<List<QueryMarginAccountsOpenOrdersResponse>>(binanceResult).First(a => a.OrderId == TaskDetail.SellingOrderID);

                        binanceResult = MarginAccountTradeBinanceService.QueryMarginAccountsAllOrders(Wallet.Assets[0].Symbol.ToString(), true, Convert.ToInt64(TaskDetail.BuyingOrderId), recvWindow: TaskDetail.RecvWindow).Result;

                        QueryMarginAccountsOpenOrdersResponse buyingPositionOldOrder = JsonConvert.DeserializeObject<List<QueryMarginAccountsOpenOrdersResponse>>(binanceResult).First(a => a.OrderId == TaskDetail.BuyingOrderId);

                        if (sellingPositionOldOrder.Status == OrderStatusEnum.FILLED)
                        {
                            LogHelper.LogMessage($"Kâr alma sonrası işlemlere devam ediliyor...", MessageTypeEnum.INFO);
                            await Repay();
                            TaskDetail.TradingStatus = IsolatedMarginV1StatusEnum.Finished;
                            await UpdateTaskDetail();
                            break;
                        }
                        else if (buyingPositionOldOrder.Status == OrderStatusEnum.FILLED)
                        {
                            LogHelper.LogMessage($"Zarar koruma sonrası işlemlere devam ediliyor...", MessageTypeEnum.INFO);

                            TaskDetail.LossLayer *= 2;
                            TaskDetail.TotalPayment += buyingPositionOldOrder.CummulativeQuoteQty;
                            TaskDetail.BuyingPrice = buyingPositionOldOrder.Price;
                            await UpdateTaskDetail();

                            #region mevcut seviyeden zarar kurtarma için satış emri verilir
                            decimal sellingPriceForCurrentLevel = Math.Round(TaskDetail.TotalPayment / Wallet.Assets[0].BaseAsset.Free, 0, MidpointRounding.ToEven);
                            binanceResult = MarginAccountTradeBinanceService.MarginAccountNewOrder(symbolType.ToString(), Side.SELL, OrderType.LIMIT, TaskDetail.IsIsolated, quantity: Math.Round(Wallet.Assets[0].BaseAsset.Free, 5, MidpointRounding.ToZero), price: sellingPriceForCurrentLevel, timeInForce: TimeInForce.GTC, recvWindow: TaskDetail.RecvWindow).Result;
                            newOrderResponse = JsonConvert.DeserializeObject<MarginAccountNewOrderResponse>(binanceResult);

                            LogHelper.LogMessage($"{sellingPriceForCurrentLevel} fiyatı ile {TaskDetail.LossLayer}. seviyesi için satış verildi.", MessageTypeEnum.SELLORDER);

                            TaskDetail.SellingOrderID = newOrderResponse.OrderId;
                            await UpdateTaskDetail();

                            UpdateWalletData();
                            #endregion

                            if (TaskDetail.LossLayer < 8)
                            {
                                #region Yeni seviye alış işlemi için gerekiyorsa borç alınır
                                decimal buyingAmountForNextLevel = TaskDetail.InitialBuyingAmount * TaskDetail.LossLayer * 2;
                                if (buyingAmountForNextLevel > Wallet.Assets.First().QuoteAsset.Free)
                                {
                                    decimal borrowAmount = buyingAmountForNextLevel - Math.Round(Wallet.Assets.First().QuoteAsset.Free, 2, MidpointRounding.ToZero);
                                    await MarginAccountTradeBinanceService.MarginAccountBorrow(Wallet.Assets.First().QuoteAsset.Asset.ToString(), borrowAmount, true, Wallet.Assets.First().Symbol.ToString(), TaskDetail.RecvWindow);

                                    LogHelper.LogMessage($"{TaskDetail.LossLayer * 2}. seviye satınalma işlemi için {borrowAmount} USDT tutarında borç alındı.", MessageTypeEnum.BORROW);

                                    UpdateWalletData();
                                }
                                #endregion

                                #region Yeni seviye alış emri verilir
                                decimal buyingPriceForNextLEvel = Math.Round(TaskDetail.BuyingPrice * 0.97m, 2, MidpointRounding.ToEven);
                                decimal buyingQuantityForNextLevel = Math.Round(buyingAmountForNextLevel / buyingPriceForNextLEvel, 5, MidpointRounding.ToZero);
                                binanceResult = MarginAccountTradeBinanceService.MarginAccountNewOrder(symbolType.ToString(), Side.BUY, OrderType.LIMIT, TaskDetail.IsIsolated, quantity: buyingQuantityForNextLevel, price: buyingPriceForNextLEvel, timeInForce: TimeInForce.GTC, recvWindow: TaskDetail.RecvWindow).Result;
                                newOrderResponse = JsonConvert.DeserializeObject<MarginAccountNewOrderResponse>(binanceResult);

                                LogHelper.LogMessage($"{buyingPriceForNextLEvel} fiyatı ile {buyingQuantityForNextLevel} adet {TaskDetail.LossLayer * 2}. seviye satın alma talimatı verildi.", MessageTypeEnum.BUY);

                                TaskDetail.BuyingOrderId = newOrderResponse.OrderId;
                                await UpdateTaskDetail();

                                UpdateWalletData();
                                #endregion
                            }

                            break;
                        }
                        break;
                    case IsolatedMarginV1StatusEnum.Finished:
                        TaskDetail.TradingStatus = IsolatedMarginV1StatusEnum.WaitingForStartOrder;
                        await UpdateTaskDetail();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Task içinde bir hat oluştu \n MESSAGE : {ex.Message} \n STACK : {ex.StackTrace} \n INNERMESSAGE : {ex.InnerException?.Message ?? "-"} \n INNERSTACK : {ex.InnerException?.StackTrace ?? "-"}");
                Environment.Exit(0);
                return;
            }
        }
        private async Task<bool> CreateTaskDetailIfNotExists(string taskName, SymbolTypeEnum symbolType)
        {
            if (!TaskDetailDictionary.ContainsKey(taskName))
            {
                IsolatedMarginV2TaskDetail taskDetail = _isolatedMarginV2TaskDetailDAL.Get(a => a.TaskName == taskName).Result;

                if (taskDetail == null)
                {
                    taskDetail = new()
                    {
                        Id = taskName,
                        TaskName = taskName,
                        SymbolType = symbolType
                    };

                    await _isolatedMarginV2TaskDetailDAL.Add(taskDetail);


                    LogHelper.LogMessage($"Task Details Created.", MessageTypeEnum.INFO);
                }

                TaskDetailDictionary[taskName] = taskDetail;
                TaskDetail = taskDetail;

                Console.WriteLine($"Task Detail Data : \nTaskName : {TaskDetail.TaskName}\nInitialBuyingAmount : {TaskDetail.InitialBuyingAmount}\nTradingStatus : {TaskDetail.TradingStatus}\nSymbolType : {TaskDetail.SymbolType}\nIsIsolated : {TaskDetail.IsIsolated}\nRecvWindow  {TaskDetail.RecvWindow}\nBuyingPrice : {TaskDetail.BuyingPrice}\nLossLayer : {TaskDetail.LossLayer}\nTotalPayment : {TaskDetail.TotalPayment}\nBuyingOrderId : {TaskDetail.BuyingOrderId}\nSellingOrderID : {TaskDetail.SellingOrderID}");
            }

            return true;
        }
        private void CreateWalletIfNotExists()
        {
            if (!WalletDictionary.ContainsKey(TaskDetail.TaskName))
            {
                IsolatedMarginWallet newWallet = new();
                WalletDictionary[TaskDetail.TaskName] = newWallet;
                Wallet = newWallet;
                UpdateWalletData();
                LogHelper.LogMessage($"Cüzdan oluşturuldu.", MessageTypeEnum.INFO);
            }
        }
        private void UpdateWalletData()
        {
            bool isUpdated = false;
            QueryIsolatedMarginAccountInfoResponse coinDataList;
            do
            {
                LogHelper.LogMessage($"Cüzdan verisi alınıyor.", MessageTypeEnum.INFO);
                string binanceResult = MarginAccountTradeBinanceService.QueryIsolatedMarginAccountInfo($"{TaskDetail.SymbolType}", 10000).Result;
                coinDataList = JsonConvert.DeserializeObject<QueryIsolatedMarginAccountInfoResponse>(binanceResult);

                if (Wallet.Assets.Any())
                {
                    decimal oldBaseFreeQuantity = Wallet.Assets[0].BaseAsset.Free;
                    decimal newBaseFreeQuantity = coinDataList.Assets[0].BaseAsset.Free;

                    if (oldBaseFreeQuantity != newBaseFreeQuantity)
                        isUpdated = true;

                    decimal oldQuoteFreeQuantity = Wallet.Assets[0].QuoteAsset.Free;
                    decimal newQuoteFreeQuantity = coinDataList.Assets[0].QuoteAsset.Free;

                    if (oldQuoteFreeQuantity != newQuoteFreeQuantity)
                        isUpdated = true;
                }
                else
                {
                    isUpdated = true;
                }

            } while (!isUpdated);

            Wallet.Assets = coinDataList.Assets;

            foreach (AssetContainer assetContainer in Wallet.Assets)
            {
                LogHelper.LogMessage($"{assetContainer.BaseAsset.Asset} --> Free {assetContainer.BaseAsset.Free}", MessageTypeEnum.WALLETDATA);
                LogHelper.LogMessage($"{assetContainer.BaseAsset.Asset} --> Locked {assetContainer.BaseAsset.Locked}", MessageTypeEnum.WALLETDATA);
                LogHelper.LogMessage($"{assetContainer.QuoteAsset.Asset} --> Total {assetContainer.QuoteAsset.TotalAsset}", MessageTypeEnum.WALLETDATA);
                LogHelper.LogMessage($"{assetContainer.QuoteAsset.Asset} --> Free {assetContainer.QuoteAsset.Free}", MessageTypeEnum.WALLETDATA);
                LogHelper.LogMessage($"{assetContainer.QuoteAsset.Asset} --> Locked {assetContainer.QuoteAsset.Locked}", MessageTypeEnum.WALLETDATA);
                LogHelper.LogMessage($"{assetContainer.QuoteAsset.Asset} --> Borrowed {assetContainer.QuoteAsset.Borrowed}", MessageTypeEnum.WALLETDATA);
                LogHelper.LogMessage($"{assetContainer.QuoteAsset.Asset} --> Interest {assetContainer.QuoteAsset.Interest}", MessageTypeEnum.WALLETDATA);
            }

            LogHelper.LogMessage($"Cüzdan verisi güncellendi.", MessageTypeEnum.INFO);
        }
        private string MarketBuy(decimal quoteOrderQty)
        {
            return MarginAccountTradeBinanceService.MarginAccountNewOrder(Wallet.Assets[0].Symbol.ToString(), Side.BUY, OrderType.MARKET, TaskDetail.IsIsolated, quoteOrderQty: quoteOrderQty, recvWindow: TaskDetail.RecvWindow).Result;
        }
        private void SetInitialBuyingAmount()
        {
            /* başlangıç tutarı hesabı 4 kademelik alıma göre yapıldı
             * 1x + 2x + 4x + 8x = 15x */
            //usdt için virgülden sonra iki basamak hassasiyetini ayarlamak için her seferinde math.round yaptım
            TaskDetail.InitialBuyingAmount = Math.Round((Math.Round(Wallet.Assets[0].QuoteAsset.Free, 2, MidpointRounding.ToZero) * 9) / 15, 2, MidpointRounding.ToZero);
        }
        private async Task<bool> UpdateTaskDetail()
        {
            await _isolatedMarginV2TaskDetailDAL.Update(TaskDetail);
            return true;
        }
        private async Task Repay()
        {
            if (Wallet.Assets[0].QuoteAsset.Borrowed > 0)
            {
                await MarginAccountTradeBinanceService.MarginAccountRepay(Wallet.Assets[0].QuoteAsset.Asset.ToString(), Wallet.Assets[0].QuoteAsset.Borrowed, true, Wallet.Assets[0].Symbol.ToString(), TaskDetail.RecvWindow);

                LogHelper.LogMessage($"Borç ödemesi yapıldı.", MessageTypeEnum.INFO);

                UpdateWalletData();
            }
        }
    }
}
