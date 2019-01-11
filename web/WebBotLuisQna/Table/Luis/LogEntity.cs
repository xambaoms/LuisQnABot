using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WebBotLuisQna.Table.Luis
{
    [Serializable]
    public class LogEntity : TableEntity
    {
        public LogEntity() { }

        public LogEntity(string luisAppID)
        {
            PartitionKey = luisAppID;
            RowKey = Guid.NewGuid().ToString();

            Data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,ffff");

        }

        public string Id { get; set; }

        public string Data { get; set; }

        public string Context { get; set; }

        public string LuisResult { get; set; }

        public string QnAResult { get; set; }
    }
}
