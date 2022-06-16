//// Licensed to the .NET Foundation under one or more agreements.
//// The .NET Foundation licenses this file to you under the MIT license.
//// See the LICENSE file in the project root for more information.

//using Domain.Enum;
//using Domain.Abstract;
//using Domain.Test.Model;

//namespace TradingAutomationWorker.Test
//{
//    internal class Spot2 : TaskAbstract
//    {
//        public Spot2(List<TestWalletData> taskWalletDataList, List<TestTaskData> taskDataList) : base(taskWalletDataList, taskDataList)
//        {

//        }

//        internal void Execute(string taskName, decimal price, DateTime priceDate, decimal profitPrice, decimal lossPrice, decimal buyLimit)
//        {
//            List<SpotWallet> taskWalletData = WalletDictionary[taskName];
//            SpotWallet baseAssetWallet = taskWalletData.First(a => a.IsBaseAsset);
//            SpotWallet quoteAssetWallet = taskWalletData.First(a => !a.IsBaseAsset);
//            TestTaskData taskData = TaskDataDictionary[taskName];


//            if (taskData.OldTradingHour.HasValue && taskData.OldTradingHour.Value.Month != priceDate.Month)
//            {
//                Console.WriteLine($"{priceDate.Month} : {quoteAssetWallet.Quantity}");
//            }

//            switch (taskData.TradingStatus)
//            {
//                case SpotTradingStatusEnum.InUSDT:
//                    //if (priceDate.Minute == 0)
//                    if (priceDate.Minute % 15 == 0)
//                    {
//                        if (quoteAssetWallet.Quantity > buyLimit)
//                        {
//                            baseAssetWallet.Quantity = (buyLimit) / price;
//                            quoteAssetWallet.Quantity -= buyLimit;
//                        }
//                        else
//                        {
//                            baseAssetWallet.Quantity = quoteAssetWallet.Quantity / price;
//                            quoteAssetWallet.Quantity = 0;
//                        }

//                        taskData.BuyingPrice = price;
//                        taskData.TradingStatus = SpotTradingStatusEnum.InBTC;
//                    }
//                    break;
//                case SpotTradingStatusEnum.InBTC:

//                    if (price - taskData.BuyingPrice > profitPrice)
//                    {
//                        quoteAssetWallet.Quantity += baseAssetWallet.Quantity * price;
//                        baseAssetWallet.Quantity = 0;
//                        taskData.TradingStatus = SpotTradingStatusEnum.InUSDT;
//                    }
//                    else if (taskData.BuyingPrice - price > lossPrice)
//                    {
//                        quoteAssetWallet.Quantity += baseAssetWallet.Quantity * price;
//                        baseAssetWallet.Quantity = 0;
//                        taskData.TradingStatus = SpotTradingStatusEnum.InUSDT;
//                    }
//                    //else if (priceDate.Minute == 59)
//                    else if ((priceDate.Minute + 1) % 15 == 0)
//                    {
//                        quoteAssetWallet.Quantity += baseAssetWallet.Quantity * price;
//                        baseAssetWallet.Quantity = 0;
//                        taskData.TradingStatus = SpotTradingStatusEnum.InUSDT;
//                    }

//                    if (taskData.TradingStatus == SpotTradingStatusEnum.InUSDT)
//                    {
//                        taskData.BuyingPrice = 0;
//                    }
//                    break;
//                default:
//                    break;
//            }

//            taskData.OldTradingHour = priceDate;
//        }
//    }
//}
