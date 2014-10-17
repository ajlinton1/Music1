using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.StorageClient;
using SqlAzureData;
using Azure;

namespace TestProject1
{
    /// <summary>
    /// Summary description for SqlAzureTest
    /// </summary>
    [TestClass]
    public class SqlAzureTest
    {
        public SqlAzureTest()
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

        /// <summary>
        ///A test for GetMellowSongs
        ///</summary>
        [TestMethod()]
        public void GetMellowSongsTest()
        {
            int num = 100; 
            SONG[] actual = MusicDao.GetMellowSongs(num);
        }

        [TestMethod()]
        public void GetSongsByGenre()
        {
            int num = 100;
            string genre = "Ambient";
            SONG[] actual = MusicDao.GetSongsByGenre(num, genre);
            foreach (SONG song in actual)
            {
                testContextInstance.WriteLine("{0} : {1}", song.GENRE, song.TITLE);
            }
        }

    }
}
