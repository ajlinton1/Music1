using System;
using System.Text;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Auth;

namespace TestProject1
{
    /// <summary>
    /// Summary description for AzureBlobTest
    /// </summary>
    [TestClass]
    public class AzureBlobTest
    {
        public AzureBlobTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void DownloadBlob()
        {
            string containerName = "public";
            string accountName = "[Account]";
            string accountKey = "[Key]";
            Uri blobEndpoint, queueEndpoint, tableEndpoint;
            blobEndpoint = new Uri(String.Format("http://{0}.blob.core.windows.net/", accountName));
            queueEndpoint = new Uri(String.Format("http://{0}.queue.core.windows.net/", accountName));
            tableEndpoint = new Uri(String.Format("http://{0}.table.core.windows.net/", accountName));
			Uri fileEndpoint = new Uri(String.Format("http://{0}.file.core.windows.net/", accountName));
			StorageCredentials credentials = new StorageCredentials(accountName, accountKey);

			CloudStorageAccount storageAccountInfo = new CloudStorageAccount(credentials, blobEndpoint, queueEndpoint, tableEndpoint, fileEndpoint);

            CloudBlobClient cloudBlobClient = storageAccountInfo.CreateCloudBlobClient();

            var containers = cloudBlobClient.ListContainers();
            foreach (var container in containers)
            {
                testContextInstance.WriteLine("{0}", container.Uri);
            }

            var songContainers = from container in containers
                                 where container.Name.IndexOf("public") > -1
                                 select container;
            foreach (var songContainer in songContainers)
            {
                testContextInstance.WriteLine("{0}", songContainer.Uri);
                var blobs = songContainer.ListBlobs();
                foreach (var blob in blobs)
                {
                    testContextInstance.WriteLine("{0}", blob.Uri);
                }
            }
        }

        [TestMethod]
        public void AzureTables()
        {
            string accountName = "[Account]";
            string accountKey = "[Key]";
            Uri blobEndpoint, queueEndpoint, tableEndpoint;
            blobEndpoint = new Uri(String.Format("http://{0}.blob.core.windows.net/", accountName));
            queueEndpoint = new Uri(String.Format("http://{0}.queue.core.windows.net/", accountName));
            tableEndpoint = new Uri(String.Format("http://{0}.table.core.windows.net/", accountName));
			Uri fileEndpoint = new Uri(String.Format("http://{0}.file.core.windows.net/", accountName));
			StorageCredentials credentials = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount storageAccountInfo = new CloudStorageAccount(credentials, blobEndpoint, queueEndpoint, tableEndpoint, fileEndpoint);
            var cloudTableClient = storageAccountInfo.CreateCloudTableClient();
            var tables = cloudTableClient.ListTables();
            foreach (var table in tables)
            {
                testContextInstance.WriteLine("{0}", table);
            }

        }

    }
}
