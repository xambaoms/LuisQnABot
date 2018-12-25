using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace CoreBotLuisQna.Table.Luis
{
    public class LuisEntity: TableEntity
    {

        public static string STR_PARTITION_KEY = "luis";
        public static string HOST_REGION = @"westus.api.cognitive.microsoft.com";
        public LuisEntity()
        {
            this.PartitionKey = STR_PARTITION_KEY;
            HostRegion = HOST_REGION;
        }

        public LuisEntity(string appID)
        {
            this.PartitionKey = STR_PARTITION_KEY;
            this.RowKey = appID;

            AppId = appID;

            HostRegion = HOST_REGION;
        }

        public string AppId { get; set; }

        public string Nome { get; set; }

        public string ApiKey { get; set; }

        public string HostRegion { get; set; }


    }
}
