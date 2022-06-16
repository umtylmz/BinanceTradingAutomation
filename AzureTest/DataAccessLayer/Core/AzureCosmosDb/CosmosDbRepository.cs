// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using Domain.Entity;
using Microsoft.Azure.Cosmos;

namespace DataAccess.AzureCosmosDb
{
    internal class CosmosDbRepository<TEntity> where TEntity : BaseEntity
    {
        public CosmosDbRepository()
        {
            _cosmosDb = new CosmosDb();
            _container = _cosmosDb.ContainerInstance;
        }

        private readonly CosmosDb _cosmosDb;
        private readonly Container _container;

        public async Task<ItemResponse<TEntity>> Add(TEntity entity)
        {
            return await _container.CreateItemAsync(entity);
        }
        public async Task<ItemResponse<TEntity>> Update(TEntity entity)
        {
            return await _container.ReplaceItemAsync(entity, entity.Id.ToString());
        }
        public async Task<ItemResponse<TEntity>> Delete(TEntity entity)
        {
            return await _container.DeleteItemAsync<TEntity>(entity.Id.ToString(), new PartitionKey());
        }
        public async Task<TEntity> Get(Expression<Func<TEntity, bool>> filter)
        {
            //return _container.GetItemLinqQueryable<TEntity>(true).Where(filter).SingleOrDefault();
            return _container.GetItemLinqQueryable<TEntity>(true).Where(filter).AsEnumerable().FirstOrDefault(); //singleordefault kullanımında bug olduğundan firstordefault kullandım
        }
        public async Task<IQueryable<TEntity>> GetList(Expression<Func<TEntity, bool>> filter = null)
        {
            if (filter != null)
                return _container.GetItemLinqQueryable<TEntity>().Where(filter);
            else
                return _container.GetItemLinqQueryable<TEntity>();
        }
    }
}
