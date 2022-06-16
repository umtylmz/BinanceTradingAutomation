// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Domain.Enum;

namespace Domain.Model.BinanceResponse
{
    public class MarginAccountNewOrderResponse
    {
        public SymbolTypeEnum Symbol { get; set; }
        public string OrderId { get; set; }
        public string ClientOrderId { get; set; }
        public string TransactTime { get; set; }
        public decimal Price { get; set; }
        public decimal OrigQty { get; set; }
        public decimal ExecutedQty { get; set; }
        public decimal CummulativeQuoteQty { get; set; }
        public string Status { get; set; }
        public string TimeInForce { get; set; }
        public string Type { get; set; }
        public string Side { get; set; }
        public bool IsIsolated { get; set; }
        public List<FillResponse> Fills { get; set; }
    }

    public class FillResponse
    {
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public decimal Commission { get; set; }
        public AssetTypeEnum CommissionAsset { get; set; } //string olabilir
    }
}

//{
//    "symbol": "BTCUSDT",
//    "orderId": 10844008117,
//    "clientOrderId": "2onO42Bj1lXsHUMjAfU8IP",
//    "transactTime": 1654347221645,
//    "price": "0",
//    "origQty": "0.00033",
//    "executedQty": "0.00033",
//    "cummulativeQuoteQty": "9.7500018",
//    "status": "FILLED",
//    "timeInForce": "GTC",
//    "type": "MARKET",
//    "side": "BUY",
//    "fills": [
//        {
//        "price": "29545.46",
//            "qty": "0.00033",
//            "commission": "0.00000033",
//            "commissionAsset": "BTC"
//        }
//    ],
//    "isIsolated": true
//        }