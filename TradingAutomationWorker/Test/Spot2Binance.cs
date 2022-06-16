//// Licensed to the .NET Foundation under one or more agreements.
//// The .NET Foundation licenses this file to you under the MIT license.
//// See the LICENSE file in the project root for more information.

//using Binance.Spot.Models;
//using Domain.Common;
//using Domain.Enum;
//using Domain.Model;
//using Domain.Abstract;
//using Domain.Test.Model;
//using Newtonsoft.Json;
//using Domain.Model;

//namespace TradingAutomationWorker.Test
//{
//    internal class Spot2Binance : TaskAbstract
//    {
//        private static Binance.Spot.Wallet BinanceWallet { get; set; }
//        private static Binance.Spot.SpotAccountTrade SpotAccountTrade { get; set; }
//        public Spot2Binance(List<TestWalletData> taskWalletDataList, List<TestTaskData> taskDataList) : base(taskWalletDataList, taskDataList)
//        {
//            if (BinanceWallet is null)
//                BinanceWallet = new Binance.Spot.Wallet(Constant.BaseUrl, Constant.ApiKey, Constant.SecretKey);

//            if (SpotAccountTrade is null)
//                SpotAccountTrade = new Binance.Spot.SpotAccountTrade(Constant.BaseUrl, Constant.ApiKey, Constant.SecretKey);
//        }

//        internal void Execute(string taskName, decimal price, DateTime priceDate, decimal profitPrice, decimal lossPrice, decimal buyLimit)
//        {
//            try
//            {
//                List<SpotWallet> taskWalletData = WalletDictionary[taskName];
//                SpotWallet baseAssetWallet = taskWalletData.First(a => a.IsBaseAsset);
//                SpotWallet quoteAssetWallet = taskWalletData.First(a => !a.IsBaseAsset);

//                if (baseAssetWallet.Quantity == 0 && quoteAssetWallet.Quantity == 0)
//                {
//                    UpdateWalletData(taskWalletData);
//                }

//                TestTaskData taskData = TaskDataDictionary[taskName];

//                //if (taskData.OldTradingHour.HasValue && taskData.OldTradingHour.Value.Hour != priceDate.Hour)
//                //{
//                //    Console.WriteLine($"{priceDate.ToString("yyyy-MM-dd HH:00")} --> {quoteAssetWallet.Quantity}");
//                //}

//                switch (taskData.TradingStatus)
//                {
//                    case SpotTradingStatusEnum.InUSDT:
//                        if (priceDate.Minute == 0)
//                        {
//                            if (quoteAssetWallet.Quantity > buyLimit)
//                                SpotAccountTrade.NewOrder(TradingPairEnum.BTCUSDT.ToString(), Side.BUY, OrderType.MARKET, quoteOrderQty: buyLimit, recvWindow: 10000);
//                            else
//                                SpotAccountTrade.NewOrder(TradingPairEnum.BTCUSDT.ToString(), Side.BUY, OrderType.MARKET, quoteOrderQty: quoteAssetWallet.Quantity, recvWindow: 10000);

//                            UpdateWalletData(taskWalletData);
//                            taskData.BuyingPrice = price;
//                            taskData.TradingStatus = SpotTradingStatusEnum.InBTC;
//                        }
//                        break;
//                    case SpotTradingStatusEnum.InBTC:

//                        if ((price - taskData.BuyingPrice > profitPrice)
//                            || (taskData.BuyingPrice - price > lossPrice)
//                            || (priceDate.Minute == 59))
//                        {
//                            SpotAccountTrade.NewOrder(TradingPairEnum.BTCUSDT.ToString(), Side.SELL, OrderType.MARKET, quantity: baseAssetWallet.Quantity, recvWindow: 10000);

//                            UpdateWalletData(taskWalletData);
//                            taskData.TradingStatus = SpotTradingStatusEnum.InUSDT;
//                        }
//                        break;
//                    default:
//                        break;
//                }

//                taskData.OldTradingHour = priceDate;
//            }
//            catch (Exception)
//            {
//                Console.WriteLine("Task içinde bir hat oluştu");
//                return;
//            }
//        }

//        private void UpdateWalletData(List<SpotWallet> taskWalletData)
//        {
//            string binanceResult = BinanceWallet.AllCoinsInformation(10000).Result;
//            List<CoinData> coinDataList = JsonConvert.DeserializeObject<List<CoinData>>(binanceResult);

//            foreach (SpotWallet item in taskWalletData)
//            {
//                item.Quantity = coinDataList.First(a => a.Coin == item.CurrencyType.ToString()).Free;
//                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm")} --> {item.CurrencyType} {item.Quantity}");
//            }
//        }
//    }
//}
