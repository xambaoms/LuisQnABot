using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace CoreBotLuisQna.Table.Luis
{
    public class QnAKBEntity: TableEntity
    {
        public QnAKBEntity() { }

        public QnAKBEntity(string luisAppId, string qnAKVID)
        {
            this.PartitionKey = luisAppId;
            this.RowKey = qnAKVID;

            QnAKBID = qnAKVID;
        }

        public string QnADomain { get; set; }

        public string QnAEndPointKey { get; set; }

        public string QnAKBID { get; set; }

        public string QnAKBNome { get; set; }

        public string LuisIntent { get; set; }
    }
}
