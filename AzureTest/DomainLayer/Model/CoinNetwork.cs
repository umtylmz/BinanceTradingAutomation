// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Domain.Model
{
    public class CoinNetwork
    {
        public string Network { get; set; }
        public string Coin { get; set; }
        public decimal WithdrawIntegerMultiple { get; set; }
        public bool IsDefault { get; set; }
        public bool DepositEnable { get; set; }
        public bool WithdrawEnable { get; set; }
        public string DepositDesc { get; set; }
        public string WithdrawDesc { get; set; }
        public string SpecialTips { get; set; }
        public string SpecialWithdrawTips { get; set; }
        public string Name { get; set; }
        public bool ResetAddressStatus { get; set; }
        public string AddressRegex { get; set; }
        public string AddressRule { get; set; }
        public string MemoRegex { get; set; }
        public decimal WithdrawFee { get; set; }
        public decimal WithdrawMin { get; set; }
        public decimal WithdrawMax { get; set; }
        public decimal DepositDust { get; set; }
        public decimal MinConfirm { get; set; }
        public decimal UnLockConfirm { get; set; }
        public bool SameAddress { get; set; }
    }
}