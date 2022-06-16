// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Core.DataAccess.EntityFramework
{
    public class EFRepositoryBase<TEntity, TContext> : IRepository<TEntity>
           where TEntity : BaseEntity
           where TContext : DbContext, new()
    {
        private readonly TContext _context;
        public EFRepositoryBase()
        {
            _context = EFContext<TContext>.GetInstance();
        }

        public int Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            int result = _context.SaveChanges();

            return result;
        }

        public int Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            int result = _context.SaveChanges();

            return result;
        }

        public int Delete(TEntity entity)
        {
            entity.IsActive = false;
            int result = Update(entity);

            return result;
        }

        public TEntity Get(Expression<Func<TEntity, bool>> filter)
        {
            return _context.Set<TEntity>().Where(a => a.IsActive).FirstOrDefault(filter);
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null)
        {
            return filter is null ? _context.Set<TEntity>().Where(a => a.IsActive) : _context.Set<TEntity>().Where(a => a.IsActive).Where(filter);
        }

        public TEntity GetById(int id)
        {
            return _context.Set<TEntity>().Where(a => a.IsActive).FirstOrDefault(a => a.Id == id);
        }
    }
}
