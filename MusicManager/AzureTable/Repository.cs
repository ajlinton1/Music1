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
	public class Repository
	{
        CloudTableClient cloudTableClient = null;
		CloudTable cloudTable = null;

		public Repository(string name)
        {
            cloudTableClient = AzureTableService.GetCloudTableClient();
			cloudTable = cloudTableClient.GetTableReference(name);
		}

		public void Add(DynamicTableEntity entity, string partitionKey, string rowKey)
		{
			entity.PartitionKey = partitionKey;
			if (rowKey==null)
			{
				rowKey = DateTime.Now.Ticks.ToString();
			}
			entity.RowKey = rowKey;
			var to = TableOperation.InsertOrReplace(entity);
			TableResult tr = cloudTable.Execute(to);
		}

		public void DeleteByPartitionKey(string partitionKey)
		{
			var tableQuery = new TableQuery();
			tableQuery.FilterString = string.Format("PartitionKey eq '{0}'", partitionKey);
			var results = cloudTable.ExecuteQuery(tableQuery);

			foreach (var result in results)
			{
				TableResult tr = cloudTable.Execute(TableOperation.Delete(result));
			}

		}
	}
}
