// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using Domain.Entity;
using Domain.Response;

namespace  TradingAutomationApi.Core.DataAccess
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        GlobalResponse Add(TEntity entity);
        GlobalResponse Update(TEntity entity);
        GlobalResponse Delete(TEntity entity);
        GlobalResponse<TEntity> Get(Expression<Func<TEntity, bool>> filter);
        GlobalResponse<TEntity> GetById(int id);
        GlobalResponse<List<TEntity>> GetList(Expression<Func<TEntity, bool>> filter = null);
    }
}
