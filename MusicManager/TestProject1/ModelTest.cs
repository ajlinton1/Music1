using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicManager;
using Music;
using Azure;

namespace TestProject1
{
    /// <summary>
    ///This is a test class for ModelTest and is intended
    ///to contain all ModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ModelTest
    {
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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        public void WriteOutput(String inString)
        {
            testContextInstance.WriteLine(inString);
        }

        /// <summary>
        ///A test for Reindex
        ///</summary>
        [TestMethod()]
        public void ReindexTest1()
        {
            Model target = new Model(); 
            Action<string> errorAction = WriteOutput; 
            Action<string> statusAction = WriteOutput; 
            Action<string> newSongAction = WriteOutput; 
            Action<string> newArtistAction = WriteOutput; 
            int threadsNum = 1;

            var azureService = new AzureService(Properties.Settings.Default.AzureAccountName, Properties.Settings.Default.AzureAccountKey);
            UploadProcessor uploadProcessor = new UploadProcessor(Properties.Settings.Default.AzureContainerName, Properties.Settings.Default.AzureAccountName, Properties.Settings.Default.AzureAccountKey);
            IMusicDao musicDao = new MusicDao(Properties.Settings.Default.ConnectionString, new string[] { Properties.Settings.Default.MusicFolder }, uploadProcessor, azureService);
            target.Reindex(threadsNum, new string[] { Properties.Settings.Default.MusicFolder }, musicDao);
        }
    }
}
