using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace Azure
{
    public class AzureService
    {
        private string accountName, accountKey;
        Uri blobEndpoint, queueEndpoint, tableEndpoint, fileEndpoint;


        public AzureService(string accountName, string accountKey)
        {
            this.accountName = accountName;
            this.accountKey = accountKey;
            blobEndpoint = new Uri(String.Format("http://{0}.blob.core.windows.net/", accountName));
            queueEndpoint = new Uri(String.Format("http://{0}.queue.core.windows.net/", accountName));
            tableEndpoint = new Uri(String.Format("http://{0}.table.core.windows.net/", accountName));
			fileEndpoint = new Uri(String.Format("http://{0}.file.core.windows.net/", accountName));
		}

        public bool StoreFile(String fileName, String containerName, String key)
        {
            try
            {
                StorageCredentials credentials = new StorageCredentials(accountName, accountKey);
                CloudStorageAccount storageAccountInfo = new CloudStorageAccount(credentials, blobEndpoint, queueEndpoint, tableEndpoint, fileEndpoint);
                CloudBlobClient cloudBlobClient = storageAccountInfo.CreateCloudBlobClient();
//                cloudBlobClient.MaximumExecutionTime = new TimeSpan(0, 20, 0);
//                cloudBlobClient.SingleBlobUploadThresholdInBytes = cloudBlobClient.SingleBlobUploadThresholdInBytes / 2;

                //                IRetryPolicy linearRetryPolicy = new LinearRetry(TimeSpan.FromSeconds(2), 10);
                //                cloudBlobClient.RetryPolicy = linearRetryPolicy;

                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
                CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(key);

                using (var fileStream = System.IO.File.OpenRead(fileName))
                {
                    blockBlob.UploadFromStream(fileStream);
                }

                ICloudBlob cloudBlob = cloudBlobContainer.GetBlobReferenceFromServer(key);
                cloudBlob.FetchAttributes();
                cloudBlob.Properties.ContentType = "audio/mp3";
                cloudBlob.SetProperties();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public IEnumerable<Uri> ListFiles(String containerName)
        {
            List<Uri> ret = new List<Uri>();

            StorageCredentials credentials = new StorageCredentials(accountName, accountKey);

            CloudStorageAccount storageAccountInfo = new CloudStorageAccount(credentials, blobEndpoint, queueEndpoint, tableEndpoint, fileEndpoint);

            CloudBlobClient cloudBlobClient = storageAccountInfo.CreateCloudBlobClient();

            IEnumerable<IListBlobItem> blobs = cloudBlobClient.ListBlobs(containerName);
            foreach (IListBlobItem blob in blobs)
            {
                ret.Add(blob.Uri);
            }

            return ret;
        }

        public CloudBlobClient GetCloudBlobContainer()
        {
            StorageCredentials credentials = new StorageCredentials(accountName, accountKey);

			CloudStorageAccount storageAccountInfo = new CloudStorageAccount(credentials, blobEndpoint, queueEndpoint, tableEndpoint, fileEndpoint);

            CloudBlobClient cloudBlobClient = storageAccountInfo.CreateCloudBlobClient();
            return cloudBlobClient;
        }

        public CloudQueueClient GetCloudQueueClient()
        {
            StorageCredentials credentials = new StorageCredentials(accountName, accountKey);

			CloudStorageAccount storageAccountInfo = new CloudStorageAccount(credentials, blobEndpoint, queueEndpoint, tableEndpoint, fileEndpoint);

            CloudQueueClient cloudQueueClient = storageAccountInfo.CreateCloudQueueClient();
            return cloudQueueClient;
        }

        public void DownloadFile(String containerName, String fileName, String key)
        {
            StorageCredentials credentials = new StorageCredentials(accountName, accountKey);

			CloudStorageAccount storageAccountInfo = new CloudStorageAccount(credentials, blobEndpoint, queueEndpoint, tableEndpoint, fileEndpoint);

            CloudBlobClient cloudBlobClient = storageAccountInfo.CreateCloudBlobClient();

            cloudBlobClient.GetContainerReference(containerName).CreateIfNotExists();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

            cloudBlobContainer.GetBlobReferenceFromServer(key).DownloadToFile(fileName, System.IO.FileMode.OpenOrCreate);
        }

        public void DeleteBlob(String containerName, String key)
        {
            StorageCredentials credentials = new StorageCredentials(accountName, accountKey);

			CloudStorageAccount storageAccountInfo = new CloudStorageAccount(credentials, blobEndpoint, queueEndpoint, tableEndpoint, fileEndpoint);

            CloudBlobClient cloudBlobClient = storageAccountInfo.CreateCloudBlobClient();

            cloudBlobClient.GetContainerReference(containerName).CreateIfNotExists();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

            cloudBlobContainer.GetBlobReferenceFromServer(key).Delete();
        }


    }
}
