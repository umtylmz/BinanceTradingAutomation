// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using Domain.Entity;
using Domain.Response;
using TradingAutomationApi.Domain.Interface.BusinessAbstract;
using TradingAutomationApi.Domain.Interface.DataAccessAbstract;

namespace TradingAutomationApi.Business.Concrete
{
    public class SymbolPriceService : ISymbolPriceService
    {
        private readonly ISymbolPriceDAL _symbolPriceDAL;
        public SymbolPriceService(ISymbolPriceDAL symbolPriceDAL)
        {
            _symbolPriceDAL = symbolPriceDAL;
        }
        public GlobalResponse Add(SymbolPrice entity)
        {
            return _symbolPriceDAL.Add(entity);
        }

        public GlobalResponse Delete(SymbolPrice entity)
        {
            return _symbolPriceDAL.Delete(entity);
        }

        public GlobalResponse<SymbolPrice> Get(Expression<Func<SymbolPrice, bool>> filter)
        {
            return _symbolPriceDAL.Get(filter);
        }

        public GlobalResponse<SymbolPrice> GetById(int id)
        {
            return (_symbolPriceDAL.GetById(id));
        }

        public GlobalResponse<List<SymbolPrice>> GetList(Expression<Func<SymbolPrice, bool>> filter = null)
        {
            return _symbolPriceDAL.GetList(filter);
        }

        public GlobalResponse Update(SymbolPrice entity)
        {
            return _symbolPriceDAL.Update(entity);
        }
    }
}
