// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Domain.Enum;

namespace TradingAutomationWorker.Test
{
    internal class Isolated10XTest1
    {
        private static Wallet? MyWallet { get; set; }
        private static M1StatusEnum? JobStatus { get; set; }
        private static decimal BuyingPairProcessPrice { get; set; }
        private static decimal SellingPairProcessPrice { get; set; }
        private static SymbolTypeEnum ProfitPair { get; set; }
        private static decimal BuyingPairMaxPrice { get; set; }
        private static decimal SellingPairMinPrice { get; set; }

        public Isolated10XTest1()
        {
            if (MyWallet is null)
            {
                MyWallet = new()
                {
                    BuyPair = new CurrencyPair()
                    {
                        CurrencyPairType = SymbolTypeEnum.BTCUSDT,
                        BaseAsset = new Currency()
                        {
                            CurrencyType = AssetTypeEnum.BTC,
                            Free = 0m,
                            Borrowed = 0m
                        },
                        QuoteAsset = new Currency()
                        {
                            CurrencyType = AssetTypeEnum.USDT,
                            Free = 30m,
                            Borrowed = 0m
                        }
                    },
                    SellPair = new CurrencyPair()
                    {
                        CurrencyPairType = SymbolTypeEnum.BTCBUSD,
                        BaseAsset = new Currency()
                        {
                            CurrencyType = AssetTypeEnum.BTC,
                            Free = 0m,
                            Borrowed = 0m
                        },
                        QuoteAsset = new Currency()
                        {
                            CurrencyType = AssetTypeEnum.BUSD,
                            Free = 30m,
                            Borrowed = 0m
                        }
                    }
                };
            }

            if (JobStatus is null)
            {
                JobStatus = M1StatusEnum.NotStarted;
            }
        }

        public void Execute(decimal buyPairPrice, decimal sellPairPrice, decimal spotToMarginTransferLimit, bool newTimeRange, decimal lossAmount)
        {
            switch (JobStatus)
            {
                case M1StatusEnum.NotStarted:
                    //buy çiftindeki quot asset bakiyesi base assete taşınır
                    MyWallet.BuyPair.BaseAsset.Free = MyWallet.BuyPair.QuoteAsset.Free / buyPairPrice;
                    MyWallet.BuyPair.QuoteAsset.Free = 0m;

                    //buy ve sell çiftlerindeki base asset spot cüzdana gönderilir
                    MyWallet.SpotFree += MyWallet.BuyPair.BaseAsset.Free + MyWallet.SellPair.BaseAsset.Free;
                    MyWallet.BuyPair.BaseAsset.Free = 0m;
                    MyWallet.SellPair.BaseAsset.Free = 0m;

                    //spot cüzdandan buy ve sell çiftlerine bakiye gönderilir
                    if (MyWallet.SpotFree > spotToMarginTransferLimit)
                    {
                        MyWallet.BuyPair.BaseAsset.Free = spotToMarginTransferLimit / 2;
                        MyWallet.SellPair.BaseAsset.Free = spotToMarginTransferLimit / 2;
                        MyWallet.SpotFree -= spotToMarginTransferLimit;
                    }
                    else
                    {
                        MyWallet.BuyPair.BaseAsset.Free = MyWallet.BuyPair.BaseAsset.Free / 2;
                        MyWallet.SellPair.BaseAsset.Free = MyWallet.BuyPair.BaseAsset.Free / 2;
                        MyWallet.SpotFree = 0;
                    }

                    //buy çiftinin base asseti quote asssete çevrilir
                    MyWallet.BuyPair.QuoteAsset.Free = MyWallet.BuyPair.BaseAsset.Free * buyPairPrice;
                    MyWallet.BuyPair.BaseAsset.Free = 0m;

                    //jobdata status güncellenir
                    JobStatus = M1StatusEnum.WalletSynced;

                    //todo:jobdata veritabanı güncellenir

                    break;
                case M1StatusEnum.WalletSynced:
                    if (newTimeRange)
                    {
                        //Borç alımı yapılır
                        MyWallet.BuyPair.QuoteAsset.Borrowed = MyWallet.BuyPair.QuoteAsset.Free * 9;
                        MyWallet.BuyPair.QuoteAsset.Free += MyWallet.BuyPair.QuoteAsset.Borrowed;

                        MyWallet.SellPair.BaseAsset.Borrowed = MyWallet.SellPair.BaseAsset.Free * 9;
                        MyWallet.SellPair.BaseAsset.Free += MyWallet.SellPair.BaseAsset.Borrowed;

                        //işlemlere girilir
                        MyWallet.BuyPair.BaseAsset.Free = MyWallet.BuyPair.QuoteAsset.Free / buyPairPrice;
                        MyWallet.BuyPair.QuoteAsset.Free = 0;
                        BuyingPairProcessPrice = buyPairPrice;

                        MyWallet.SellPair.QuoteAsset.Free = MyWallet.SellPair.BaseAsset.Free * sellPairPrice;
                        MyWallet.SellPair.BaseAsset.Free = 0;
                        SellingPairProcessPrice = sellPairPrice;

                        JobStatus = M1StatusEnum.WaitingPriceMovements;
                    }
                    break;
                case M1StatusEnum.WaitingPriceMovements:
                    if (buyPairPrice < BuyingPairProcessPrice - lossAmount)
                    {
                        MyWallet.BuyPair.QuoteAsset.Free = MyWallet.BuyPair.BaseAsset.Free * buyPairPrice;
                        MyWallet.BuyPair.BaseAsset.Free = 0;

                        MyWallet.BuyPair.QuoteAsset.Free -= MyWallet.BuyPair.QuoteAsset.Borrowed;
                        MyWallet.BuyPair.QuoteAsset.Borrowed = 0;

                        ProfitPair = SymbolTypeEnum.BTCBUSD;

                        JobStatus = M1StatusEnum.LossPositionClosed;
                    }
                    else if (sellPairPrice > SellingPairProcessPrice + lossAmount)
                    {
                        MyWallet.SellPair.BaseAsset.Free = MyWallet.SellPair.QuoteAsset.Free / sellPairPrice;
                        MyWallet.SellPair.QuoteAsset.Free = 0;

                        MyWallet.SellPair.BaseAsset.Free -= MyWallet.SellPair.BaseAsset.Borrowed;
                        MyWallet.SellPair.BaseAsset.Borrowed = 0;

                        ProfitPair = SymbolTypeEnum.BTCUSDT;

                        JobStatus = M1StatusEnum.LossPositionClosed;
                    }
                    break;
                case M1StatusEnum.LossPositionClosed:
                    //switch (ProfitPair)
                    //{
                    //    //kar alma kısımları ya da kar alamıyorsa işlemden çıkış komutları yazılacak
                    //    //maksimum ve minimum fiyat verileri waiting price movements ve losspositionclosed kısımlarında takip edilecek (iki adet static property oluştur takip için)
                    //    case TradingPairEnum.BTCUSDT:
                    //        if (buyPairPrice < BuyingPairProcessPrice - lossAmount)
                    //        {
                    //            MyWallet.BuyPair.QuoteAsset.Free = MyWallet.BuyPair.BaseAsset.Free * buyPairPrice;
                    //            MyWallet.BuyPair.BaseAsset.Free = 0;

                    //            MyWallet.BuyPair.QuoteAsset.Free -= MyWallet.BuyPair.QuoteAsset.Borrowed;
                    //            MyWallet.BuyPair.QuoteAsset.Borrowed = 0;

                    //            ProfitPair = TradingPairEnum.BTCBUSD;

                    //            JobStatus = M1StatusEnum.LossPositionClosed;
                    //        }
                    //    case TradingPairEnum.BTCBUSD:
                    //        break;
                    //    default:
                    //        break;
                    //}
                    break;
                case M1StatusEnum.ProfitPositionClosed:
                    break;
                default:
                    break;
            }

        }

        private class Wallet
        {
            public decimal SpotFree { get; set; }
            public CurrencyPair BuyPair { get; set; }
            public CurrencyPair SellPair { get; set; }
        }
        private class CurrencyPair
        {
            public SymbolTypeEnum CurrencyPairType { get; set; }
            public Currency BaseAsset { get; set; }
            public Currency QuoteAsset { get; set; }
        }
        private class Currency
        {
            public AssetTypeEnum CurrencyType { get; set; }
            public decimal Free { get; set; }
            public decimal Borrowed { get; set; }
        }
    }
}
