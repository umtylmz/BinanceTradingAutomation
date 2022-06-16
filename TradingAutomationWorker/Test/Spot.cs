// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Domain.Enum;
using TradingAutomationWorker.Domain.Entity;
using TradingAutomationWorker.EF;

namespace TradingAutomationWorker.Test
{
    internal class Spot
    {
        internal void Test1(List<SymbolPrice> _symbolPrices)
        {
            decimal walletUSDT = 50m;
            decimal walletBTC = 0m;
            List<SymbolPrice> symbolPrices = _symbolPrices;

            //using (TAContext context = new())
            //{
            //    symbolPrices = context.SymbolPrices.Where(a => a.SymbolType == TradingPairEnum.BTCUSDT).OrderBy(a => a.TradingHour).ToList();
            //}

            var test = symbolPrices.AsEnumerable().GroupBy(a => new DateTime(a.TradingHour.Year, a.TradingHour.Month, a.TradingHour.Day, a.TradingHour.Hour, 0, 0));
            int testCount = test.Count();

            decimal buyPrice = 0m;
            decimal maxPrice = 0m;
            decimal minPrice = 0m;

            SpotStatus status = SpotStatus.WaitNextHour;

            int counter = 0;
            foreach (var item in test)
            {
                if (status == SpotStatus.WaitNextHour)
                {
                    status = SpotStatus.InUsd;
                    Console.WriteLine($"{++counter}/{testCount} - {walletUSDT}");
                }

                foreach (var item2 in item.ToList())
                {
                    if (status == SpotStatus.WaitNextHour)
                        continue;

                    foreach (var item3 in GetFourPrice(item2))
                    {
                        if (status == SpotStatus.WaitNextHour)
                            continue;

                        switch (status)
                        {
                            case SpotStatus.InUsd:
                                if (item.ToList().IndexOf(item2) == 0)
                                {
                                    walletBTC = walletUSDT / item3;
                                    walletUSDT = 0;
                                    buyPrice = item3;
                                    maxPrice = buyPrice;
                                    minPrice = buyPrice;
                                    status = SpotStatus.InBtc;
                                }
                                break;
                            case SpotStatus.InBtc:
                                if (item3 >= maxPrice)
                                    maxPrice = item3;
                                else if (item3 < minPrice)
                                {
                                    minPrice = item3;

                                    if (buyPrice - minPrice >= 100m)
                                    {
                                        walletUSDT = walletBTC * item3;
                                        status = SpotStatus.WaitNextHour;
                                        buyPrice = 0;
                                        maxPrice = 0;
                                        minPrice = 0;
                                    }
                                }
                                //else if (item3 <= maxPrice * 0.75m && item3 - buyPrice >= 200)
                                //{
                                //    walletUSDT = walletBTC * item3;
                                //    status = SpotStatus.WaitNextHour;
                                //    buyPrice = 0;
                                //    maxPrice = 0;
                                //    minPrice = 0;
                                //}
                                else if (item3 - buyPrice >= 50m)
                                {
                                    walletUSDT = walletBTC * item3;
                                    status = SpotStatus.WaitNextHour;
                                    buyPrice = 0;
                                    maxPrice = 0;
                                    minPrice = 0;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private List<decimal> GetFourPrice(SymbolPrice symbolPrice)
        {
            if (symbolPrice.Opening >= symbolPrice.Closing)
                return new List<decimal>() { symbolPrice.Opening, symbolPrice.Low, symbolPrice.High, symbolPrice.Closing };
            else
                return new List<decimal>() { symbolPrice.Opening, symbolPrice.High, symbolPrice.Low, symbolPrice.Closing };
        }

        private enum SpotStatus
        {
            InUsd,
            InBtc,
            WaitNextHour
        }
    }
}
