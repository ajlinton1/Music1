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
using com.andrewlinton.AzureTable.Properties;

namespace com.andrewlinton.AzureTable
{
    public class AzureTableService
    {
//        const string accountName = "[Account]";
//        const string accountKey = "[Key]";

        public static CloudTableClient GetCloudTableClient()
        {
            var blobEndpoint = new Uri(String.Format("http://{0}.blob.core.windows.net/", Settings.Default.AccountName));
            var queueEndpoint = new Uri(String.Format("http://{0}.queue.core.windows.net/", Settings.Default.AccountName));
            var tableEndpoint = new Uri(String.Format("http://{0}.table.core.windows.net/", Settings.Default.AccountName));
			var fileEndpoint = new Uri(String.Format("http://{0}.table.core.windows.net/", Settings.Default.AccountName));
			var credentials = new StorageCredentials(Settings.Default.AccountName, Settings.Default.AccountKey);
			var storageAccountInfo = new CloudStorageAccount(credentials, blobEndpoint, queueEndpoint, tableEndpoint, fileEndpoint);
            var cloudTableClient = storageAccountInfo.CreateCloudTableClient();
            return cloudTableClient;
        }

/*        public static void AddSegment(string genre, string segementNorm, string segment)
        {
            var cloudTableClient = GetCloudTableClient();
            var cloudTable = cloudTableClient.GetTableReference("segments2");
            DynamicTableEntity dynamicTableEntity = new DynamicTableEntity();
            dynamicTableEntity.PartitionKey = genre;
            dynamicTableEntity.RowKey = segementNorm;
            char[] c = {'\\'};
            string[] s = segment.Split(c);
            if (s.Length>0)
            {
                dynamicTableEntity["segment0"] = new EntityProperty(s[0]);
            }
            if (s.Length > 1)
            {
                dynamicTableEntity["segment1"] = new EntityProperty(s[1]);
            }
            if (s.Length > 2)
            {
                dynamicTableEntity["segment2"] = new EntityProperty(s[2]);
            }
            if (s.Length > 3)
            {
                dynamicTableEntity["segment3"] = new EntityProperty(s[3]);
            }
            if (s.Length > 4)
            {
                dynamicTableEntity["segment4"] = new EntityProperty(s[4]);
            }
            var to = TableOperation.Insert(dynamicTableEntity);
            TableResult tr = cloudTable.Execute(to);
        } */

/*        public static void AddGenre(string genre, string numSongs)
        {
            var cloudTableClient = GetCloudTableClient();
            var cloudTable = cloudTableClient.GetTableReference("genres");
            DynamicTableEntity dynamicTableEntity = new DynamicTableEntity();
            dynamicTableEntity.PartitionKey = genre;
            dynamicTableEntity.RowKey = numSongs;
            var to = TableOperation.Insert(dynamicTableEntity);
            TableResult tr = cloudTable.Execute(to);
        } */

/*        public static void AddArtist(string artistName, string numSongs)
        {
            if (string.IsNullOrEmpty(artistName))
            {
                throw new ArgumentException("Invalid artist name");
            }
            var cloudTableClient = GetCloudTableClient();
            var cloudTable = cloudTableClient.GetTableReference("artists");
            DynamicTableEntity dynamicTableEntity = new DynamicTableEntity();
            dynamicTableEntity.PartitionKey = artistName;
            dynamicTableEntity.RowKey = numSongs;
            var to = TableOperation.Insert(dynamicTableEntity);
            TableResult tr = cloudTable.Execute(to);
        } */

    }
}
