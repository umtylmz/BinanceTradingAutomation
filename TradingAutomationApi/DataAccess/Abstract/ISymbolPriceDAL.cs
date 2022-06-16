// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Domain.Entity;
using TradingAutomationApi.Core.DataAccess;

namespace TradingAutomationApi.Domain.Interface.DataAccessAbstract
{
    public interface ISymbolPriceDAL : IRepository<SymbolPrice>
    {
    }
}
