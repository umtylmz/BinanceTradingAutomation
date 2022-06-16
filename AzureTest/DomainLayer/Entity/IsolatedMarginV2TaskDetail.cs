// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Domain.Entity;
using Domain.Enum;
using Domain.Interface;

namespace Domain.Model
{
    public class IsolatedMarginV2TaskDetail : BaseEntity, ITaskDetail
    {
        public string TaskName { get; set; }
        public decimal InitialBuyingAmount { get; set; }
        public IsolatedMarginV1StatusEnum TradingStatus { get; set; } = IsolatedMarginV1StatusEnum.WaitingForStartOrder;
        public SymbolTypeEnum SymbolType { get; set; }
        public bool IsIsolated { get; set; } = true;
        public long RecvWindow { get; set; } = 10000;
        public decimal BuyingPrice { get; set; }
        public decimal LossLayer { get; set; }
        public decimal TotalPayment { get; set; }
        public string BuyingOrderId { get; set; }
        public string SellingOrderID { get; set; }
    }
}
