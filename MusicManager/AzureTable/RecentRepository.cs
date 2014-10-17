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
	public class RecentRepository
	{
        CloudTableClient cloudTableClient = null;

		public RecentRepository()
        {
            cloudTableClient = AzureTableService.GetCloudTableClient();
        }

		public void Add(DynamicTableEntity entity, string entityType)
		{
			var cloudTable = cloudTableClient.GetTableReference("recent");
			entity.PartitionKey = entityType;
			entity.RowKey = entity.Timestamp.UtcTicks.ToString();
			var to = TableOperation.InsertOrReplace(entity);
			TableResult tr = cloudTable.Execute(to);
		}

		public void DeleteByEntityType(string entityType)
		{
			var cloudTable = cloudTableClient.GetTableReference("recent");

			var tableQuery = new TableQuery();
			tableQuery.FilterString = string.Format("PartitionKey eq '{0}'", entityType);
			var results = cloudTable.ExecuteQuery(tableQuery);

			foreach (var result in results)
			{
				TableResult tr = cloudTable.Execute(TableOperation.Delete(result));
			}

		}
	}
}
