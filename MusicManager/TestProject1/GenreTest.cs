using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using com.andrewlinton.music;
using com.andrewlinton.AzureTable;

namespace TestProject1
{
	/// <summary>
	/// Summary description for GenreTest
	/// </summary>
	[TestClass]
	public class GenreTest
	{
		public GenreTest()
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

/*		[TestMethod]
		public void PopulateGenres()
		{
			var cloudTableClient = GetCloudTableClient();
			int maxResults = 100;
			var cloudTable = cloudTableClient.GetTableReference("songs");
			var tableQuery = new TableQuery();
			tableQuery.TakeCount = maxResults;
			int numResults = 0;
			int skip = 0;

			do
			{
				numResults = 0;
				var results = cloudTable.ExecuteQuery(tableQuery).Skip(skip);
				foreach (var result in results)
				{
					string genre = null;
					if (result.Properties.ContainsKey("genre"))
					{
						genre = result["genre"].StringValue;
					}
				}

			}
			while (numResults > 0);
		}

		public static CloudTableClient GetCloudTableClient()
		{
			string accountName = "[Account]";
			string accountKey = "[Key]";
			var blobEndpoint = new Uri(String.Format("http://{0}.blob.core.windows.net/", accountName));
			var queueEndpoint = new Uri(String.Format("http://{0}.queue.core.windows.net/", accountName));
			var tableEndpoint = new Uri(String.Format("http://{0}.table.core.windows.net/", accountName));
			var fileEndpoint = new Uri(String.Format("http://{0}.table.core.windows.net/", accountName));
			var credentials = new StorageCredentials(accountName, accountKey);
			var storageAccountInfo = new CloudStorageAccount(credentials, blobEndpoint, queueEndpoint, tableEndpoint, fileEndpoint);
			var cloudTableClient = storageAccountInfo.CreateCloudTableClient();
			return cloudTableClient;
		} */

		[TestMethod]
		public void DeleteAllGenres()
		{
			var genreRepository = new GenreRepository();
			genreRepository.DeleteAll();
		}

		[TestMethod]
		public void PopulateGenres()
		{
			var genreService = new GenreService();
			genreService.Populate();
		}
	}
}
