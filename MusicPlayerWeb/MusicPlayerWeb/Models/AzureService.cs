using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using MusicPlayerWeb.Properties;

namespace MusicPlayerWeb.Models
{
    public class AzureService
    {
        public static CloudTableClient GetCloudTableClient()
        {
            var blobEndpoint = new Uri(String.Format("http://{0}.blob.core.windows.net/", Settings.Default.AccountName));
            var queueEndpoint = new Uri(String.Format("http://{0}.queue.core.windows.net/", Settings.Default.AccountName));
            var tableEndpoint = new Uri(String.Format("http://{0}.table.core.windows.net/", Settings.Default.AccountName));
			var fileEndpoint = new Uri(String.Format("http://{0}.file.core.windows.net/", Settings.Default.AccountName));
			var credentials = new StorageCredentials(Settings.Default.AccountName, Settings.Default.AccountKey);
            var storageAccountInfo = new CloudStorageAccount(credentials, blobEndpoint, queueEndpoint, tableEndpoint, fileEndpoint);
            var cloudTableClient = storageAccountInfo.CreateCloudTableClient();
            return cloudTableClient;
        }
    }
}