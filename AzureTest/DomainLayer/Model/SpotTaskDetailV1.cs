// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Domain.Enum;

namespace Domain.Model
{
    public class SpotTaskDetailV1
    {
        public DateTime? OldTradingHour { get; set; }
        public decimal BuyingPrice { get; set; }
        public SpotTradingStatusEnum TradingStatus { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MinPrice { get; set; }
    }
}
