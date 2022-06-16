// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Domain.Enum;

namespace Domain.Model.BinanceResponse
{
    public class QueryMarginAccountsOpenOrdersResponse
    {
        public SymbolTypeEnum Symbol { get; set; }
        public string OrderId { get; set; }
        public string ClientOrderId { get; set; }
        public decimal Price { get; set; }
        public decimal OrigQty { get; set; }
        public decimal ExecutedQty { get; set; }
        public decimal CummulativeQuoteQty { get; set; }
        public OrderStatusEnum Status { get; set; }
        public string TimeInForce { get; set; }
        public string Type { get; set; }
        public string Side { get; set; }
        public decimal StopPrice { get; set; }
        public decimal IcebergQty { get; set; }
        public string Time { get; set; }
        public string UpdateTime { get; set; }
        public bool IsWorking { get; set; }
        public bool IsIsolated { get; set; }
    }
}

//{
//    "symbol": "BTCUSDT",
//        "orderId": 10919516496,
//        "clientOrderId": "and_905f61657f4140e3abe7cf4cfe73229e",
//        "price": "20000",
//        "origQty": "0.00066",
//        "executedQty": "0",
//        "cummulativeQuoteQty": "0",
//        "status": "NEW",
//        "timeInForce": "GTC",
//        "type": "LIMIT",
//        "side": "BUY",
//        "stopPrice": "0",
//        "icebergQty": "0",
//        "time": 1654805201919,
//        "updateTime": 1654805201919,
//        "isWorking": true,
//        "isIsolated": true
//    }