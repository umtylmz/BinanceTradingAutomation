// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using Domain.Entity;
using Domain.Response;
using Microsoft.EntityFrameworkCore;

namespace TradingAutomationApi.Core.DataAccess.EntityFramework
{
    public class EFRepositoryBase<TEntity, TContext> : IRepository<TEntity>
           where TEntity : BaseEntity
           where TContext : DbContext, new()
    {
        private readonly TContext _context = EFContext<TContext>.GetInstance();

        public GlobalResponse Add(TEntity entity)
        {
            try
            {
                _context.Set<TEntity>().Add(entity);
                _context.SaveChanges();
                return new();
            }
            catch (Exception)
            {
                return new() { IsSucceed = false };
            }
        }

        public GlobalResponse Update(TEntity entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                _context.SaveChanges();
                return new();
            }
            catch (Exception)
            {
                return new() { IsSucceed = false };
            }
        }

        public GlobalResponse Delete(TEntity entity)
        {
            try
            {
                entity.IsActive = false;
                Update(entity);
                return new();
            }
            catch (Exception)
            {
                return new() { IsSucceed = false };
            }
        }

        public GlobalResponse<TEntity> Get(Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                TEntity? data = _context.Set<TEntity>().Where(a => a.IsActive).FirstOrDefault(filter);
                GlobalResponse<TEntity> response = new()
                {
                    Data = data
                };
                return response;
            }
            catch (Exception)
            {
                return new() { IsSucceed = false };
            }
        }

        public GlobalResponse<TEntity> GetById(int id)
        {
            try
            {
                TEntity? data = _context.Set<TEntity>().Where(a => a.IsActive).FirstOrDefault(a => a.Id == id);
                return new() { Data = data };
            }
            catch (Exception)
            {
                return new() { IsSucceed = false };
            }
        }

        public GlobalResponse<List<TEntity>> GetList(Expression<Func<TEntity, bool>>? filter = null)
        {
            try
            {
                IQueryable<TEntity> data = _context.Set<TEntity>().Where(a => a.IsActive);
                data = filter is null ? data : data.Where(filter);
                return new() { Data = data.ToList() };
            }
            catch (Exception)
            {
                return new() { IsSucceed = false };
            }
        }
    }
}
