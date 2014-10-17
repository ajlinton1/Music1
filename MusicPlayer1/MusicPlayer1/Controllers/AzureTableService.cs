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
    public class AzureTableService
    {

        public static CloudTableClient GetCloudTableClient()
        {
			string accountName = "[AccountName]";
			string accountKey = "[Key]";
			var blobEndpoint = new Uri(String.Format("http://{0}.blob.core.windows.net/", accountName));
			var queueEndpoint = new Uri(String.Format("http://{0}.queue.core.windows.net/", accountName));
			var tableEndpoint = new Uri(String.Format("http://{0}.table.core.windows.net/", accountName));
			var fileEndpoint = new Uri(String.Format("http://{0}.table.core.windows.net/", accountName));
			var credentials = new StorageCredentials(accountName, accountKey);
			var storageAccountInfo = new CloudStorageAccount(credentials, blobEndpoint, queueEndpoint, tableEndpoint, fileEndpoint);
            var cloudTableClient = storageAccountInfo.CreateCloudTableClient();
            return cloudTableClient;
        }

    }
}
