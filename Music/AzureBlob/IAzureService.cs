using System;

namespace Azure
{
    public interface IAzureService
    {
        void DeleteBlob(string containerName, string key);
        void DownloadFile(string containerName, string fileName, string key);
        global::Microsoft.WindowsAzure.StorageClient.CloudBlobClient GetCloudBlobContainer();
        global::Microsoft.WindowsAzure.StorageClient.CloudQueueClient GetCloudQueueClient();
        global::System.Collections.Generic.IEnumerable<Uri> ListFiles(string containerName);
        void StoreFile(string fileName, string containerName, string key);
    }
}
