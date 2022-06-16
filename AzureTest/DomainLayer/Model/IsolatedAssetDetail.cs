// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Domain.Model
{
    public class IsolatedAssetDetail
    {
        public string Asset { get; set; }
        public bool BorrowEnabled { get; set; }
        public decimal Borrowed { get; set; }
        public decimal Free { get; set; }
        public decimal Interest { get; set; }
        public decimal Locked { get; set; }
        public decimal NetAsset { get; set; }
        public decimal NetAssetOfBtc { get; set; }
        public bool repayEnabled { get; set; }
        public decimal totalAsset { get; set; }
    }
}
