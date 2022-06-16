// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Domain.Enum;

namespace Domain.Model
{
    public class IsolatedAsset
    {
        public IsolatedAssetDetail BaseAsset { get; set; }
        public IsolatedAssetDetail QuoteAsset { get; set; }
        public SymbolTypeEnum Symbol { get; set; }
        public bool IsolatedCreated { get; set; }
        public decimal MarginLevel { get; set; }
        public MarginLevelStatusEnum MarginLevelStatus { get; set; }
        public decimal MarginRatio { get; set; }
        public decimal IndexPrice { get; set; }
        public decimal LiquidatePrice { get; set; }
        public decimal LiquidateRate { get; set; }
        public bool TradeEnabled { get; set; }
    }
}
