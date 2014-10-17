using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using com.andrewlinton.AzureTable;
using Microsoft.WindowsAzure.Storage.Table;

namespace TestProject1
{
	/// <summary>
	/// Summary description for MaintenanceTest
	/// </summary>
	[TestClass]
	public class MaintenanceTest
	{
		public MaintenanceTest()
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

		public void GetModificationDate(CloudTable table, DynamicTableEntity entity)
		{
			testContextInstance.WriteLine("{0}", entity.Timestamp);
			for(int i = 0; i< recentSongs.Length;i++)
			{
				if (recentSongs[i]==null)
				{
					recentSongs[i] = entity;
					return;
				}
				else if (recentSongs[i].Timestamp.CompareTo(entity.Timestamp)<0)
				{
					for (int x = recentSongs.Length - 2; x > i; x--)
					{
						recentSongs[x + 1] = recentSongs[x];
					}
					recentSongs[i] = entity;
					return;
				}
			}
		}

		DynamicTableEntity[] recentSongs;

		[TestMethod]
		public void RecentSongs()
		{
			int? maxSongsToProcess = null;
			recentSongs = new DynamicTableEntity[100];
			var songRepository = new SongRepository(new Repository("log"));
			songRepository.Process(null, GetModificationDate, maxSongsToProcess);

			string entityType = "song";
			var recentRepository = new RecentRepository();
			recentRepository.DeleteByEntityType(entityType);
			foreach (DynamicTableEntity entity in recentSongs)
			{
				testContextInstance.WriteLine("{0}", entity.Timestamp);
				recentRepository.Add(entity, entityType);
			}
		}
	}
}
