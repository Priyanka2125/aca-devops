﻿using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using ConfigurationProvider = DevOps.App.Configuration.ConfigurationProvider;

namespace DevOps.App.Repositories
{
    public class EncryptedTableStorageRepository
    {
        private const string EncryptionEntityPartitionKey = "DevOps";
        private const string EncryptionEntityRowKey = "Introduction";
    
      
        private readonly TableClient _tableClient;


        public EncryptedTableStorageRepository()
        {
            var connectionString = ConfigurationProvider.Get<string>("StorageAccount.ConnectionString");
            var tableName = ConfigurationProvider.Get<string>("StorageAccount.TableName");
            _tableClient = new TableClient(connectionString, tableName);
        }
        private TableEntity ToTableEntity(string key, string value)
        {
            TableEntity tableEntity;
            tableEntity = new TableEntity
            {
                PartitionKey = EncryptionEntityPartitionKey,
                RowKey = EncryptionEntityRowKey
            };
            tableEntity.Add(key, value);
            return tableEntity;
        }
        public async Task UpdateAsync(string entityKey, string entityValue)
        {
            await _tableClient.UpsertEntityAsync(ToTableEntity(entityKey, entityValue));
        }

        public async Task<TableEntity> GetAsync()
        {
            var result = await _tableClient.GetEntityAsync<TableEntity>(EncryptionEntityPartitionKey, EncryptionEntityRowKey);
            await _tableClient.CreateIfNotExistsAsync();

            if (result == null)
            {
                throw new InvalidOperationException("No entity was found. Please insert an entity first.");
            }
           
            return result;
        }
    }
}