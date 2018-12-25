using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace CoreBotLuisQna.Table
{
    public class TableWrapper : ITableWrapper
    {

        public static string STR_TABLE_NAME = "luisqnabottable";

        private CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;
        private CloudTable table;

        public TableWrapper(string azureStorageConnectionString)
        {
            storageAccount = CloudStorageAccount.Parse(azureStorageConnectionString);

            tableClient = storageAccount.CreateCloudTableClient();

            table = tableClient.GetTableReference(STR_TABLE_NAME);

        }

        public Task<bool> CreateIfNotExists()
        {
            return table.CreateIfNotExistsAsync();
        }

        public Task<IList<TableResult>> Insert<T>(T entity) where T : TableEntity
        {
            TableBatchOperation batchOperation = new TableBatchOperation();
            batchOperation.Insert(entity);

            return table.ExecuteBatchAsync(batchOperation);
        }

        public async Task<T> Get<T>(string partitionKey, string rowKey) where T : TableEntity
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            TableResult result = await table.ExecuteAsync(retrieveOperation);

            return result.Result as T;
        }

        public async Task<List<T>> List<T>(string partitionKey) where T : TableEntity, new()
        {
            TableQuery<T> query =
                new TableQuery<T>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            List<T> entities = new List<T>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<T> resultSegment = await table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                foreach (T customer in resultSegment.Results)
                {
                    entities.Add(customer);
                }
            } while (token != null);

            return entities;
        }

        public Task<TableResult> Delete<T>(T entity) where T : TableEntity
        {
            entity.ETag = "*";

            TableOperation deleteOperation = TableOperation.Delete(entity);

            return table.ExecuteAsync(deleteOperation);
        }
    }
}
