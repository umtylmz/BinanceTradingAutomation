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
            _container = _database.CreateContainerIfNotExistsAsync(_containerId, "").Result;
        }
        private readonly CosmosClient _client;
        private readonly Database _database;
        private readonly Container _container;
        private const string _endpointUri = "https:////umitylmz.documents.azure.com:443//";
        private const string _primaryKey = "iFWONRxCVQWwyMi6rCjyhymh9oLCFa5ZIlJZHi5LNVTXbosmrPPdJ6YRJzJEXVBsN2IozZUZWmFtPhtmdJeEog==";
        private const string _databaseId = "FamilyDatabase";
        private const string _containerId = "FamilyContainer";
        internal Container ContainerInstance { get { return _container; } }
    }
}