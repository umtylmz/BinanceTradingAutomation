// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Domain.Interface;
using Domain.Model.BinanceResponse;

namespace Domain.Model
{
    public class IsolatedMarginWallet : IWallet
    {
        public IsolatedMarginWallet()
        {
            Assets = new();
        }

        public List<AssetContainer> Assets { get; set; }

    }
}
