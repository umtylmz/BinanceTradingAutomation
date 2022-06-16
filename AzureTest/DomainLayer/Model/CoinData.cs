// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Domain.Model
{
    public class CoinData
    {
        public string Coin { get; set; }
        public bool DepositAllEnable { get; set; }
        public bool WithdrawAllEnable { get; set; }
        public string Name { get; set; }
        public decimal Free { get; set; }
        public decimal Locked { get; set; }
        public decimal Freeze { get; set; }
        public decimal Withdrawing { get; set; }
        public decimal Ipoing { get; set; }
        public decimal Ipoable { get; set; }
        public decimal Storage { get; set; }
        public bool IsLegalMoney { get; set; }
        public bool Trading { get; set; }
        public List<CoinNetwork> NetworkList { get; set; }
    }
}