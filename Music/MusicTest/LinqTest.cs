using System;
using System.Text;
using System.Collections.Generic;
using Music;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MusicTest
{
    /// <summary>
    /// Summary description for LinqTest
    /// </summary>
    [TestClass]
    public class LinqTest
    {
        public LinqTest()
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
        public void QueryArtists()
        {
            try
            {
                MusicDao musicDao=new MusicDao();
                musicDao.QueryArtists();
            }
            catch (Exception ex)
            {
                testContextInstance.WriteLine(ex.Message);
                testContextInstance.WriteLine(ex.StackTrace);
                throw;
            }
        }

        [TestMethod]
        public void QuerySongs()
        {
            try
            {
                MusicDao musicDao = new MusicDao();
                musicDao.QuerySongs();
            }
            catch (Exception ex)
            {
                testContextInstance.WriteLine(ex.Message);
                testContextInstance.WriteLine(ex.StackTrace);
                throw;
            }
        }

    }
}
