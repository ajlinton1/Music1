using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Music;
using IdSharp.Tagging.ID3v2;

namespace MusicTest
{
    /// <summary>
    /// Summary description for Mp3TagTest
    /// </summary>
    [TestClass]
    public class Mp3TagTest
    {
        public Mp3TagTest()
        {
            //
            // TODO: Add constructor logic here
            //
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
        public void Mp3Test()
        {
            try
            {
                MusicDao musicDao = new MusicDao();
                String fileName = @"F:\Music\Tom Middleton\Shinkansen.mp3";

                String title;
                String artist;
                String album;
                String genre;
                musicDao.GetMp3Tags(fileName, out title, out artist, out album, out genre);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
        
/*        [TestMethod]
        public void Mp3Tag()
        {
            try
            {
                String filename = @"E:\Music\Asia\Go.mp3";
                FileStream fs = File.OpenRead(filename);

                byte[] buffer=new byte[128];
                ASCIIEncoding ae = new ASCIIEncoding();
                fs.Seek(-128, SeekOrigin.End);
                fs.Read(buffer, 0, 128);
                String tag = ae.GetString(buffer);
                String hdr = tag.Substring(0, 3);
                if (hdr == "TAG")
                {
                    String trackTitle = tag.Substring(3, 30);
                    String artist = tag.Substring(33, 30);
                    String recording = tag.Substring(63, 30);
                    String year = tag.Substring(93, 4);
                    String comment = tag.Substring(97, 30);
                }

                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        } */
    }
}
