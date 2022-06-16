// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Domain.Enum;

namespace Domain.Model
{
    public class SymbolPrice
    {
        public SymbolTypeEnum Symbol { get; set; }
        public decimal Price { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
