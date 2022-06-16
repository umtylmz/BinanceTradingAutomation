// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Azure.Cosmos;

namespace DataAccess.AzureCosmosDb
{
    internal class CosmosDb
    {
        internal CosmosDb()
        {
            _client = new(_endpointUri, _primaryKey);
            _database = _client.CreateDatabaseIfNotExistsAsync(_databaseId).Result;
            _container = _database.CreateContainerIfNotExistsAsync(_containerId, "/TaskName").Result;
        }
        private readonly CosmosClient _client;
        private readonly Database _database;
        private readonly Container _container;
        private const string _endpointUri = "";
        private const string _primaryKey = "";
        private const string _databaseId = "TradingDatabase";
        private const string _containerId = "TradingContainer";
        internal Container ContainerInstance { get { return _container; } }
    }
}