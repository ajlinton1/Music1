using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace com.andrewlinton.AzureTable
{
    public class GenreRepository
    {
        CloudTableClient cloudTableClient = null;

        public GenreRepository()
        {
            cloudTableClient = AzureTableService.GetCloudTableClient();
        }

        public void SetGenre(string genre, string numSongs)
        {
            var cloudTable = cloudTableClient.GetTableReference("genres");
            DynamicTableEntity dynamicTableEntity = new DynamicTableEntity();
            dynamicTableEntity.PartitionKey = "genres";
            dynamicTableEntity.RowKey = genre;
            dynamicTableEntity.Properties["numSongs"] = new EntityProperty(numSongs);
            var to = TableOperation.InsertOrReplace(dynamicTableEntity);
            TableResult tr = cloudTable.Execute(to);
        }

        public void ResetGenres()
        {
            var cloudTable = cloudTableClient.GetTableReference("genres");
            var query = new TableQuery();
            var genres = cloudTable.ExecuteQuery(query);

            foreach (DynamicTableEntity genre in genres)
            {
                genre.Properties["numSongs"] = new EntityProperty("0");
                var to = TableOperation.InsertOrReplace(genre);
                TableResult tr = cloudTable.Execute(to);
            }
        }

		public void DeleteAll()
		{
			Debug.WriteLine("Delete all");
			var cloudTable = cloudTableClient.GetTableReference("genres");
			var tableQuery = new TableQuery();
			var results = cloudTable.ExecuteQuery(tableQuery);
			foreach (var result in results)
			{
				cloudTable.Execute(TableOperation.Delete(result));
			}
		}

    }
}
