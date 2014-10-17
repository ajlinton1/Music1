using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1
{
    /// <summary>
    /// Summary description for FtpTest
    /// </summary>
    [TestClass]
    public class FtpTest
    {
        public FtpTest()
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
        public void FtpUpload()
        {
            try
            {
                string uploadFile = @"C:\Music\Stars\Personal.WMA";
                Uri ftpSite = new Uri("ftp://65.182.208.175/[Account]com/html/Media/Track1.WMA");
                FileInfo fileInfo = new FileInfo(uploadFile);
                FtpWebRequest request =(FtpWebRequest)WebRequest.Create(ftpSite);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UseBinary = true;
                request.ContentLength = fileInfo.Length;
                request.Credentials = new NetworkCredential("210291", "mycYRs8a");
                byte[] byteBuffer = new byte[4096];
                using (Stream requestStream = request.GetRequestStream())
                {
                    using (FileStream fileStream = new FileStream(uploadFile, FileMode.Open))
                    {
                        int bytesRead = 0;
                        do
                        {
                            bytesRead = fileStream.Read(byteBuffer, 0, byteBuffer.Length);
                            if (bytesRead > 0)
                            {
                                requestStream.Write(byteBuffer, 0, bytesRead);
                            }
                        }
                        while (bytesRead > 0);
                    }
                }
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine(response.StatusDescription);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}
