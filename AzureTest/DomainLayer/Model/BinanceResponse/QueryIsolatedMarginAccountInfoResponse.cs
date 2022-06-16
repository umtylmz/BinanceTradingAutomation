// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Domain.Enum;

namespace Domain.Model.BinanceResponse
{
    public class QueryIsolatedMarginAccountInfoResponse
    {
        public List<AssetContainer> Assets { get; set; }
    }

    public class AssetContainer
    {
        public AssetData BaseAsset { get; set; }
        public AssetData QuoteAsset { get; set; }
        public SymbolTypeEnum Symbol { get; set; } //string olma ihtimali var
        public bool MsolatedCreated { get; set; }
        public decimal MarginLevel { get; set; }
        public string MarginLevelStatus { get; set; } //enum'ını yap
        public decimal MarginRatio { get; set; }
        public decimal IndexPrice { get; set; }
        public decimal LiquidatePrice { get; set; }
        public decimal LiquidateRate { get; set; }
        public bool TradeEnabled { get; set; }
        public bool Enabled { get; set; }
        //        "symbol": "BTCBUSD",
        //"isolatedCreated": true,
        //"marginLevel": "999",
        //"marginLevelStatus": "EXCESSIVE",
        //"marginRatio": "10",
        //"indexPrice": "30269.54301514",
        //"liquidatePrice": "0",
        //"liquidateRate": "0",
        //"tradeEnabled": true,
        //"enabled": true
    }

    public class AssetData
    {
        public AssetTypeEnum Asset { get; set; } //string olabilir
        public bool BorrowEnabled { get; set; }
        public decimal Borrowed { get; set; }
        public decimal Free { get; set; }
        public decimal Interest { get; set; }
        public decimal Locked { get; set; }
        public decimal NetAsset { get; set; }
        public decimal NetAssetOfBtc { get; set; }
        public bool RepayEnabled { get; set; }
        public decimal TotalAsset { get; set; }
        //"asset": "BTC",
        //"borrowEnabled": true,
        //"borrowed": "0",
        //"free": "0.00008384",
        //"interest": "0",
        //"locked": "0",
        //"netAsset": "0.00008384",
        //"netAssetOfBtc": "0.00008384",
        //"repayEnabled": true,
        //"totalAsset": "0.00008384"
    }
}
