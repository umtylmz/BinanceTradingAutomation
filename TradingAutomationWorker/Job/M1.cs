// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Binance.Spot;
using Binance.Spot.Models;
using Domain.Common;
using Domain.Enum;
using Domain.Model;
using Newtonsoft.Json;
using TradingAutomationApi.Domain.Request;
using Domain.Model;

namespace TradingAutomationWorker.Job
{
    internal class M1
    {
        private static Dictionary<string, M1JobData> TaskList { get; set; } = new();
        private static Dictionary<string, IsolatedMarginAccountInfo> AccountInfoList { get; set; } = new();

        private readonly MarginAccountTrade marginAccountTrade;
        private readonly Wallet wallet;

        public M1()
        {
            marginAccountTrade = new MarginAccountTrade(Constant.BaseUrl, Constant.ApiKey, Constant.SecretKey);
            wallet = new Wallet(Constant.BaseUrl, Constant.ApiKey, Constant.SecretKey);

            if (!TaskList.Any())
            {
                //todo:veritabanından veriyi çek
            }
        }

        internal async void Execute(M1Request request)
        {
            M1JobData jobData;
            if (!TaskList.ContainsKey(request.Name))
            {
                jobData = new M1JobData() { Name = request.Name };
                TaskList.Add(request.Name, jobData);
                //todo:veritabanına da kaydet
            }
            else
                jobData = TaskList[request.Name];

            if (!AccountInfoList.ContainsKey(request.Name))
            {
                IsolatedMarginAccountInfo accountInfo = GetMarginAccountInfo(request.BuyPair, request.SellPair).Result;
                AccountInfoList.Add(request.Name, accountInfo);
            }

            switch (jobData.Status)
            {
                case M1StatusEnum.NotStarted:
                    //buy çiftindeki quot asset bakiyesi base assete taşınır
                    await TransferBetweenBaseAndQuoteAsset(request.BuyPair, AccountInfoList[request.Name], AssetTransferDirectionEnum.QuoteToBase);
                    AccountInfoList[request.Name] = GetMarginAccountInfo(request.BuyPair, request.SellPair).Result;

                    //buy ve sell çiftlerindeki base asset spot cüzdana gönderilir
                    await TransferMarginAssetsToSpotWallet(AccountInfoList[request.Name]);
                    AccountInfoList[request.Name] = GetMarginAccountInfo(request.BuyPair, request.SellPair).Result;

                    //spot cüzdandan buy ve sell çiftlerine bakiye gönderilir
                    await TransferSpotAssetToMarginWallet(AccountInfoList[request.Name], request.MaxTransferQuantityFromSpotToMargin);

                    //buy çiftinin base asseti quote asssete çevrilir
                    await TransferBetweenBaseAndQuoteAsset(request.BuyPair, AccountInfoList[request.Name], AssetTransferDirectionEnum.BaseToQuote);
                    AccountInfoList[request.Name] = GetMarginAccountInfo(request.BuyPair, request.SellPair).Result;

                    //jobdata status güncellenir
                    jobData.Status = M1StatusEnum.WalletSynced;

                    //todo:jobdata veritabanı güncellenir

                    break;
                case M1StatusEnum.WalletSynced:
                    //buraya gerek kalmadı
                    //satış pozisyonu olacak cüzdan için btc alımı yapılır
                    //string test = await marginAccountTrade.MarginAccountNewOrder(TradingPairEnum.BTCBUSD.ToString(), Side.BUY, OrderType.MARKET, true, recvWindow: 10000, quoteOrderQty: AccountInfoList[request.Name].Assets.Where(a => a.Symbol == TradingPairEnum.BTCUSDT).First().QuoteAsset.NetAsset);
                    //AccountInfoList[request.Name] = GetMarginAccountInfo(request.BuyPair, request.SellPair).Result;

                    //Borç alımı yapılır
                    string test2 = await marginAccountTrade.MarginAccountBorrow(AssetTypeEnum.USDT.ToString(), AccountInfoList[request.Name].Assets.Where(a => a.Symbol == SymbolTypeEnum.BTCUSDT).First().QuoteAsset.NetAsset * 9, true, AccountInfoList[request.Name].Assets.Where(a => a.Symbol == SymbolTypeEnum.BTCUSDT).First().Symbol.ToString(), 10000);
                    string test3 = await marginAccountTrade.MarginAccountBorrow(AssetTypeEnum.BTC.ToString(), AccountInfoList[request.Name].Assets.Where(a => a.Symbol == SymbolTypeEnum.BTCBUSD).First().BaseAsset.NetAsset * 9, true, AccountInfoList[request.Name].Assets.Where(a => a.Symbol == SymbolTypeEnum.BTCBUSD).First().Symbol.ToString(), 10000);

                    //işlemlere girilir
                    string test4 = await marginAccountTrade.MarginAccountNewOrder(SymbolTypeEnum.BTCUSDT.ToString(), Side.BUY, OrderType.MARKET, true, recvWindow: 10000, quoteOrderQty: AccountInfoList[request.Name].Assets.Where(a => a.Symbol == SymbolTypeEnum.BTCUSDT).First().QuoteAsset.totalAsset);
                    string test5 = await marginAccountTrade.MarginAccountNewOrder(SymbolTypeEnum.BTCBUSD.ToString(), Side.SELL, OrderType.MARKET, true, recvWindow: 10000, quantity: AccountInfoList[request.Name].Assets.Where(a => a.Symbol == SymbolTypeEnum.BTCBUSD).First().BaseAsset.totalAsset);
                    break;
                case M1StatusEnum.WaitingPriceMovements:
                    break;
                case M1StatusEnum.LossPositionClosed:
                    break;
                case M1StatusEnum.ProfitPositionClosed:
                    break;
                default:
                    break;
            }
        }

        internal async Task<IsolatedMarginAccountInfo> GetMarginAccountInfo(SymbolTypeEnum buyPair, SymbolTypeEnum sellPair)
        {
            string accountInfoBinanceResult = marginAccountTrade.QueryIsolatedMarginAccountInfo($"{buyPair},{sellPair}", 10000).Result;
            IsolatedMarginAccountInfo accountInfo = JsonConvert.DeserializeObject<IsolatedMarginAccountInfo>(accountInfoBinanceResult);
            return accountInfo;
        }
        internal async Task TransferBetweenBaseAndQuoteAsset(SymbolTypeEnum tradingPair, IsolatedMarginAccountInfo accountInfo, AssetTransferDirectionEnum transferDirection)
        {
            IsolatedAsset assetPair = accountInfo.Assets.FirstOrDefault(a => a.Symbol == tradingPair);

            switch (transferDirection)
            {
                case AssetTransferDirectionEnum.BaseToQuote:
                    await marginAccountTrade.MarginAccountNewOrder(assetPair.Symbol.ToString(), Side.SELL, OrderType.MARKET, true, recvWindow: 10000, quantity: assetPair.BaseAsset.NetAsset);
                    break;
                case AssetTransferDirectionEnum.QuoteToBase:
                    await marginAccountTrade.MarginAccountNewOrder(assetPair.Symbol.ToString(), Side.BUY, OrderType.MARKET, true, recvWindow: 10000, quoteOrderQty: assetPair.QuoteAsset.NetAsset);
                    break;
            }
        }
        internal async Task TransferMarginAssetsToSpotWallet(IsolatedMarginAccountInfo accountInfo)
        {
            foreach (IsolatedAsset item in accountInfo.Assets)
            {
                await marginAccountTrade.IsolatedMarginAccountTransfer(item.BaseAsset.Asset, item.Symbol.ToString(), IsolatedMarginAccountTransferType.ISOLATED_MARGIN, IsolatedMarginAccountTransferType.SPOT, item.BaseAsset.NetAsset, 10000);
            }
        }

        internal async Task TransferSpotAssetToMarginWallet(IsolatedMarginAccountInfo accountInfo, decimal maxTransferQuantity)
        {
            string binanceResult = wallet.AllCoinsInformation(10000).Result;
            List<CoinData> coinDataList = JsonConvert.DeserializeObject<List<CoinData>>(binanceResult);
            AssetTypeEnum baseCurrency = (AssetTypeEnum)Enum.Parse(typeof(AssetTypeEnum), accountInfo.Assets[0].BaseAsset.Asset);
            decimal totalBaseCurrencyQuantityInSpotWallet = coinDataList.First(a => a.Coin == baseCurrency.ToString()).Free;
            decimal transferQuantity = totalBaseCurrencyQuantityInSpotWallet > maxTransferQuantity ? maxTransferQuantity : totalBaseCurrencyQuantityInSpotWallet;
            foreach (IsolatedAsset item in accountInfo.Assets)
            {
                await marginAccountTrade.IsolatedMarginAccountTransfer(item.BaseAsset.Asset, item.Symbol.ToString(), IsolatedMarginAccountTransferType.SPOT, IsolatedMarginAccountTransferType.ISOLATED_MARGIN, transferQuantity / 2, 10000);
            }
        }

        internal async Task Borrow(SymbolTypeEnum buyPair, SymbolTypeEnum sellPair, IsolatedMarginAccountInfo accountInfo)
        { 
        
        }

    }
}
