// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    internal class IsolatedMarginV1 : TaskAbstract
    {
        public IsolatedMarginV1() : base()
        {

        }

        private string TaskName { get; set; }
        private IsolatedMarginWallet Wallet { get; set; }
        private IsolatedTaskDetailV1 TaskDetail { get; set; }

        internal async Task Execute(string taskName, SymbolTypeEnum symbolType, decimal price, decimal initialQuoteBuyingQuantity)
        {
            try
            {
                TaskName = taskName;
                CreateTaskDetailIfNotExists(initialQuoteBuyingQuantity, symbolType);
                CreateWalletIfNotExists();

                string newOrderResult;
                MarginAccountNewOrderResponse newOrderResponse;
                switch (TaskDetail.TradingStatus)
                {
                    case IsolatedMarginV1StatusEnum.WaitingForStartOrder:
                        decimal amount = initialQuoteBuyingQuantity;
                        if (amount > Wallet.Assets.First().QuoteAsset.Free)
                        {
                            decimal borrowAmount = initialQuoteBuyingQuantity - Wallet.Assets.First().QuoteAsset.Free;
                            await MarginAccountTradeBinanceService.MarginAccountBorrow(Wallet.Assets.First().QuoteAsset.Asset.ToString(), borrowAmount, true, SymbolTypeEnum.BTCUSDT.ToString(), 10000);
                            UpdateWalletData();

                            amount = Math.Round(Wallet.Assets.First().QuoteAsset.Free, 2, MidpointRounding.ToZero);
                        }

                        newOrderResult = MarginAccountTradeBinanceService.MarginAccountNewOrder(symbolType.ToString(), Side.BUY, OrderType.MARKET, TaskDetail.IsIsolated, quoteOrderQty: amount, recvWindow: TaskDetail.RecvWindow).Result;
                        newOrderResponse = JsonConvert.DeserializeObject<MarginAccountNewOrderResponse>(newOrderResult);

                        TaskDetail.BuyingPrice = newOrderResponse.Fills.Sum(a => a.Price * a.Qty) / newOrderResponse.Fills.Sum(a => a.Qty);
                        TaskDetail.TotalPayment = newOrderResponse.Fills.Sum(a => a.Price * a.Qty);
                        TaskDetail.LossLayer = 1;
                        TaskDetail.TradingStatus = IsolatedMarginV1StatusEnum.Started;

                        LogHelper.LogMessage($"{TaskDetail.BuyingPrice} ortalama fiyatı ile 1. seviye satınalma yapıldı.", MessageTypeEnum.BUY);

                        UpdateWalletData();
                        break;
                    case IsolatedMarginV1StatusEnum.Started:
                        if (price * Wallet.Assets.First().BaseAsset.Free >= TaskDetail.TotalPayment * 1.01m)
                        {
                            newOrderResult = MarginAccountTradeBinanceService.MarginAccountNewOrder(symbolType.ToString(), Side.SELL, OrderType.MARKET, TaskDetail.IsIsolated, quantity: Math.Round(Wallet.Assets.First().BaseAsset.Free, 5, MidpointRounding.ToZero), recvWindow: TaskDetail.RecvWindow).Result;
                            newOrderResponse = JsonConvert.DeserializeObject<MarginAccountNewOrderResponse>(newOrderResult);

                            decimal sellingPrice = newOrderResponse.Fills.Sum(a => a.Price * a.Qty) / newOrderResponse.Fills.Sum(a => a.Qty);
                            LogHelper.LogMessage($"{sellingPrice} ortalama fiyatı ile satış yapıldı.", MessageTypeEnum.BUY);

                            UpdateWalletData();

                            if (Wallet.Assets.First().QuoteAsset.Borrowed > 0)
                            {
                                await MarginAccountTradeBinanceService.MarginAccountRepay(Wallet.Assets.First().QuoteAsset.Asset.ToString(), Wallet.Assets.First().QuoteAsset.Borrowed, true, SymbolTypeEnum.BTCUSDT.ToString(), 10000);
                            }


                            TaskDetail.TradingStatus = IsolatedMarginV1StatusEnum.Finished;
                        }
                        else if (price < TaskDetail.BuyingPrice * 0.97m && TaskDetail.LossLayer < 8)
                        {
                            decimal quoteAssetBuyingAmount = initialQuoteBuyingQuantity * TaskDetail.LossLayer * 2;

                            if (quoteAssetBuyingAmount > Wallet.Assets.First().QuoteAsset.Free)
                            {
                                decimal borrowAmount = quoteAssetBuyingAmount - Wallet.Assets.First().QuoteAsset.Free;
                                await MarginAccountTradeBinanceService.MarginAccountBorrow(Wallet.Assets.First().QuoteAsset.Asset.ToString(), borrowAmount, true, SymbolTypeEnum.BTCUSDT.ToString(), 10000);
                                UpdateWalletData();

                                quoteAssetBuyingAmount = Math.Round(Wallet.Assets.First().QuoteAsset.Free, 2, MidpointRounding.ToZero);
                            }

                            newOrderResult = MarginAccountTradeBinanceService.MarginAccountNewOrder(symbolType.ToString(), Side.BUY, OrderType.MARKET, TaskDetail.IsIsolated, quoteOrderQty: quoteAssetBuyingAmount, recvWindow: TaskDetail.RecvWindow).Result;
                            newOrderResponse = JsonConvert.DeserializeObject<MarginAccountNewOrderResponse>(newOrderResult);

                            TaskDetail.BuyingPrice = newOrderResponse.Fills.Sum(a => a.Price * a.Qty) / newOrderResponse.Fills.Sum(a => a.Qty);
                            TaskDetail.TotalPayment += newOrderResponse.Fills.Sum(a => a.Price * a.Qty);
                            TaskDetail.LossLayer *= 2m;

                            LogHelper.LogMessage($"{TaskDetail.BuyingPrice} ortalama fiyatı ile {TaskDetail.LossLayer}. seviye satınalma yapıldı.", MessageTypeEnum.BUY);

                            UpdateWalletData();
                        }
                        break;
                    case IsolatedMarginV1StatusEnum.Finished:
                        TaskDetail.TradingStatus = IsolatedMarginV1StatusEnum.WaitingForStartOrder;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Task içinde bir hat oluştu \n MESSAGE : {ex.Message} \n STACK : {ex.StackTrace} \n INNERMESSAGE : {ex.InnerException?.Message ?? "-"} \n INNERSTACK : {ex.InnerException?.StackTrace ?? "-"}");
                return;
            }
        }
        private void CreateWalletIfNotExists()
        {
            if (!WalletDictionary.ContainsKey(TaskName))
            {
                IsolatedMarginWallet newWallet = new();
                WalletDictionary[TaskName] = newWallet;
                Wallet = newWallet;
                UpdateWalletData();
                LogHelper.LogMessage($"Cüzdan oluşturuldu.", MessageTypeEnum.INFO);
            }
        }
        private void CreateTaskDetailIfNotExists(decimal initialBuyingQuantity, SymbolTypeEnum symbolType)
        {
            if (!TaskDetailDictionary.ContainsKey(TaskName))
            {
                IsolatedTaskDetailV1 newTaskDetail = new() {
                    InitialBuyingQuantity = initialBuyingQuantity,
                    SymbolType = symbolType
                };
                TaskDetailDictionary[TaskName] = newTaskDetail;
                TaskDetail = newTaskDetail;
                LogHelper.LogMessage($"Göre detayları oluşturuldu.", MessageTypeEnum.INFO);
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
                    decimal oldBaseTotalQuantity = Wallet.Assets[0].BaseAsset.TotalAsset;
                    decimal newBaseTotalQuantity = coinDataList.Assets[0].BaseAsset.TotalAsset;

                    if (oldBaseTotalQuantity != newBaseTotalQuantity)
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
                LogHelper.LogMessage($"{assetContainer.BaseAsset.Asset} --> Total {assetContainer.BaseAsset.TotalAsset}", MessageTypeEnum.WALLETDATA);
                LogHelper.LogMessage($"{assetContainer.BaseAsset.Asset} --> Borrowed {assetContainer.BaseAsset.Borrowed}", MessageTypeEnum.WALLETDATA);
                LogHelper.LogMessage($"{assetContainer.BaseAsset.Asset} --> Interest {assetContainer.BaseAsset.Interest}", MessageTypeEnum.WALLETDATA);
                LogHelper.LogMessage($"{assetContainer.QuoteAsset.Asset} --> Total {assetContainer.QuoteAsset.TotalAsset}", MessageTypeEnum.WALLETDATA);
                LogHelper.LogMessage($"{assetContainer.QuoteAsset.Asset} --> Borrowed {assetContainer.QuoteAsset.Borrowed}", MessageTypeEnum.WALLETDATA);
                LogHelper.LogMessage($"{assetContainer.QuoteAsset.Asset} --> Interest {assetContainer.QuoteAsset.Interest}", MessageTypeEnum.WALLETDATA);
            }

            LogHelper.LogMessage($"Cüzdan verisi güncellendi.", MessageTypeEnum.INFO);
        }
    }
}
