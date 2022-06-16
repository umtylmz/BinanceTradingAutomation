// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using DataAccess.Context.EntityFramework;
using Domain.Entity;
using TradingAutomationApi.Core.DataAccess.EntityFramework;
using TradingAutomationApi.Domain.Interface.DataAccessAbstract;

namespace TradingAutomationApi.DataAccess.Concrete
{
    public class SymbolPriceDAL : EFRepositoryBase<SymbolPrice, TAContext>, ISymbolPriceDAL
    {
    }
}
