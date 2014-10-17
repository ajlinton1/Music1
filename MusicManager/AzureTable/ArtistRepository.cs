using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace com.andrewlinton.AzureTable
{
    public class ArtistRepository
    {
        CloudTableClient cloudTableClient = null;

        public ArtistRepository()
        {
            cloudTableClient = AzureTableService.GetCloudTableClient();
        }

        public void Set(string artistName, string numSongs)
        {
            var cloudTable = cloudTableClient.GetTableReference("artists");
            DynamicTableEntity dynamicTableEntity = new DynamicTableEntity();
            dynamicTableEntity.PartitionKey = "artists";
            dynamicTableEntity.RowKey = artistName;
            dynamicTableEntity.Properties["numSongs"] = new EntityProperty(numSongs);
            var to = TableOperation.InsertOrReplace(dynamicTableEntity);
            TableResult tr = cloudTable.Execute(to);
        }

        public void Reset()
        {
            var cloudTable = cloudTableClient.GetTableReference("artists");
            var query = new TableQuery();
            var genres = cloudTable.ExecuteQuery(query);

            foreach (DynamicTableEntity genre in genres)
            {
                genre.Properties["numSongs"] = new EntityProperty("0");
                var to = TableOperation.InsertOrReplace(genre);
                TableResult tr = cloudTable.Execute(to);
            }
        }

    }
}

