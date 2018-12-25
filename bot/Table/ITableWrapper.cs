using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace LuisBot.Table
{
   
    public interface ITableWrapper
    {
        Task<bool> CreateIfNotExists();
        Task<TableResult> Delete<T>(T entity) where T : TableEntity;
        Task<T> Get<T>(string partitionKey, string rowKey) where T : TableEntity;
        Task<IList<TableResult>> Insert<T>(T entity) where T : TableEntity;
        Task<List<T>> List<T>(string partitionKey) where T : TableEntity, new();
    }
}