//// Licensed to the .NET Foundation under one or more agreements.
//// The .NET Foundation licenses this file to you under the MIT license.
//// See the LICENSE file in the project root for more information.

//using Binance.Spot.Models;
//using Domain.Common;
//using Domain.Enum;
//using Domain.Abstract;
//using Domain.Test.Model;
//using Domain.Model;
//using Newtonsoft.Json;
//using Helper;

//namespace AzureTest.Test
//{
//    /// <summary>
//    /// zararda anlık satım yapılır, pozisyon karda sıkıştırılır
//    /// </summary>
//    internal class SpotTest3 : TaskAbstract
//    {
//        private static Binance.Spot.Wallet BinanceWallet { get; set; }
//        private static Binance.Spot.SpotAccountTrade SpotAccountTrade { get; set; }
//        public SpotTest3(List<TestWalletData> taskWalletDataList, List<TestTaskData> taskDataList) : base(taskWalletDataList, taskDataList)
//        {
//            if (BinanceWallet is null)
//                BinanceWallet = new Binance.Spot.Wallet(Constant.BaseUrl, Constant.ApiKey, Constant.SecretKey);

//            if (SpotAccountTrade is null)
//                SpotAccountTrade = new Binance.Spot.SpotAccountTrade(Constant.BaseUrl, Constant.ApiKey, Constant.SecretKey);
//        }

//        internal async Task Execute(string taskName, decimal price, DateTime priceDate, decimal profitPrice, decimal lossPrice, decimal buyLimit)
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
//                        if (priceDate.Minute % 15 == 0)
//                        {
//                            if (quoteAssetWallet.Quantity > buyLimit)
//                            {
//                                string x = SpotAccountTrade.NewOrder(TradingPairEnum.BTCUSDT.ToString(), Side.BUY, OrderType.MARKET, quoteOrderQty: Math.Round(buyLimit, 2, MidpointRounding.ToZero), recvWindow: 10000).Result;
//                                LogHelper.LogMessage($"Satın alma işlemi yapıldı. (Alış fiyatı : {price})", MessageTypeEnum.BUY);
//                            }
//                            else
//                            {
//                                var x = SpotAccountTrade.NewOrder(TradingPairEnum.BTCUSDT.ToString(), Side.BUY, OrderType.MARKET, quoteOrderQty: Math.Round(quoteAssetWallet.Quantity, 2, MidpointRounding.ToZero), recvWindow: 10000).Result;
//                                LogHelper.LogMessage($"Satın alma işlemi yapıldı. (Alış fiyatı : {price})", MessageTypeEnum.BUY);
//                            }

//                            UpdateWalletData(taskWalletData);
//                            taskData.BuyingPrice = price;
//                            taskData.TradingStatus = SpotTradingStatusEnum.InBTC;
//                        }
//                        break;
//                    case SpotTradingStatusEnum.InBTC:

//                        if ((price - taskData.BuyingPrice > profitPrice)
//                            || (taskData.BuyingPrice - price > lossPrice))
//                        {
//                            string res = SpotAccountTrade.NewOrder(TradingPairEnum.BTCUSDT.ToString(), Side.SELL, OrderType.MARKET, quantity: Math.Round(baseAssetWallet.Quantity, 5, MidpointRounding.ToZero), recvWindow: 10000).Result;
//                            LogHelper.LogMessage($"Satış yapıldı. (Satış fiyatı : {price})", MessageTypeEnum.SELL);

//                            UpdateWalletData(taskWalletData);
//                            taskData.TradingStatus = SpotTradingStatusEnum.InUSDT;
//                        }
//                        //else if ((priceDate.Minute % 15 == 0) && (taskData.OldTradingHour.Value.Minute % 15 != 0))
//                        //{
//                        //    if (price < taskData.BuyingPrice)
//                        //    {
//                        //        taskData.BuyingPrice = price;
//                        //        LogHelper.LogMessage($"Satın alma ücreti güncellendi. (Yeni fiyat : {price})", MessageTypeEnum.INFO);
//                        //    }
//                        //    else
//                        //        LogHelper.LogMessage($"Yeni zaman dilimine aynı pozisyonda deam ediliyor.", MessageTypeEnum.INFO);
//                        //}
//                        break;
//                    default:
//                        break;
//                }

//                taskData.OldTradingHour = priceDate;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Task içinde bir hat oluştu \n MESSAGE : {ex.Message} \n STACK : {ex.StackTrace} \n INNERMESSAGE : {ex.InnerException?.Message ?? "-"} \n INNERSTACK : {ex.InnerException?.StackTrace ?? "-"}");
//                return;
//            }
//        }

//        private void UpdateWalletData(List<SpotWallet> taskWalletData)
//        {
//            bool isOk = false;
//            List<CoinData> coinDataList;
//            do
//            {
//                LogHelper.LogMessage($"Cüzdan verisi alınıyor.", MessageTypeEnum.INFO);
//                string binanceResult = BinanceWallet.AllCoinsInformation(10000).Result;
//                coinDataList = JsonConvert.DeserializeObject<List<CoinData>>(binanceResult);

//                SpotWallet testCorrectionWallet = taskWalletData[0];

//                if (testCorrectionWallet.Quantity != coinDataList.First(a => a.Coin == testCorrectionWallet.CurrencyType.ToString()).Free)
//                    isOk = true;

//            } while (!isOk);

//            foreach (SpotWallet item in taskWalletData)
//            {
//                item.Quantity = coinDataList.First(a => a.Coin == item.CurrencyType.ToString()).Free;

//                LogHelper.LogMessage($"{item.CurrencyType} --> {item.Quantity}", MessageTypeEnum.WALLETDATA);
//            }

//            LogHelper.LogMessage($"Cüzdan verisi güncellendi.", MessageTypeEnum.INFO);
//        }
//    }
//}
