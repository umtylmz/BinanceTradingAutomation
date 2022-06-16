// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using Domain.Entity;

namespace Core.DataAccess
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        int Add(TEntity entity);
        int Update(TEntity entity);
        int Delete(TEntity entity);
        TEntity Get(Expression<Func<TEntity, bool>> filter);
        TEntity GetById(int id);
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null);
    }
}
