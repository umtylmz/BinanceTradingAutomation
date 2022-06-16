// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Binance.Spot;
using Domain.Common;
using Domain.Enum;
using Newtonsoft.Json;
using Domain.Model;

namespace AzureTest.Job
{
    internal static class CommonJob
    {
        internal static Dictionary<SymbolTypeEnum, List<SymbolPrice>> tradingPairList = new Dictionary<SymbolTypeEnum, List<SymbolPrice>>();
        internal static async Task GetPrice(params SymbolTypeEnum[] tradingPair)
        {
            Market market = new Market(Constant.BaseUrl, Constant.ApiKey, Constant.SecretKey);
            foreach (SymbolTypeEnum item in tradingPair)
            {
                string binanceResult = market.SymbolPriceTicker(item.ToString()).Result;
                SymbolPrice symbolPrice = JsonConvert.DeserializeObject<SymbolPrice>(binanceResult);
                symbolPrice.AddedDate = DateTime.Now;

                if (!tradingPairList.ContainsKey(item))
                {
                    tradingPairList.Add(item, new List<SymbolPrice>());
                }

                tradingPairList[item].Add(symbolPrice);
            }
        }

        internal static List<SymbolPrice> GetPriceReal(params SymbolTypeEnum[] tradingPair)
        {
            List<SymbolPrice> result = new();
            Market market = new Market(Constant.BaseUrl, Constant.ApiKey, Constant.SecretKey);
            foreach (SymbolTypeEnum item in tradingPair)
            {
                string binanceResult = market.SymbolPriceTicker(item.ToString()).Result;
                SymbolPrice symbolPrice = JsonConvert.DeserializeObject<SymbolPrice>(binanceResult);
                symbolPrice.AddedDate = DateTime.Now;
                result.Add(symbolPrice);
            }
            return result;
        }
    }
}
