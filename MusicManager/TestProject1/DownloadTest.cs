using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Music;
using SqlAzureData;

namespace TestProject1
{
    /// <summary>
    /// Summary description for DownloadTest
    /// </summary>
    [TestClass]
    public class DownloadTest
    {
        public DownloadTest()
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

        //[TestMethod]
        //public void DownloadBlobsWithNoSongRecords()
        //{
        //    String folderName = "e:\\music";

        //    SongDownloader songDownloader = new SongDownloader();
        //    songDownloader.OnSongFound += DownloadSong;
        //    songDownloader.OnSongNotFound += songDownloader.DownloadBlob;
        //    songDownloader.DownloadSongs(folderName);
        //}

        public void DownloadSong(SqlAzureData.SONG song, String folderName)
        {
        }
    }
}
