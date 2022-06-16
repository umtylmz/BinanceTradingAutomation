// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using DataAccess.Interface;
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
            ItemResponse<TEntity> response = await _container.CreateItemAsync<TEntity>(entity);
            return response;
        }
        public async Task<ItemResponse<TEntity>> Update(TEntity entity)
        {
            ItemResponse<TEntity> response = await _container.ReplaceItemAsync<TEntity>(entity, entity.Id.ToString());
            return response;
        }
        public async Task<ItemResponse<TEntity>> Delete(TEntity entity)
        {
            ItemResponse<TEntity> response = await _container.DeleteItemAsync<TEntity>(entity.Id.ToString());
            return response,
        }
        public TEntity Get(Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }
        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter = null)
        {
            throw new NotImplementedException();
        }
    }
}
