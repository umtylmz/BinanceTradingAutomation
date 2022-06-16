// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Domain.Enum;

namespace TradingAutomationApi.Domain.Request
{
    public class M1Request
    {
        public string Name { get; set; }
        public SymbolTypeEnum BuyPair { get; set; }
        public SymbolTypeEnum SellPair { get; set; }
        public decimal LossStopPrice { get; set; }
        public decimal ProfitTakingRate { get; set; }
        public decimal MaxTransferQuantityFromSpotToMargin { get; set; }
    }
}
