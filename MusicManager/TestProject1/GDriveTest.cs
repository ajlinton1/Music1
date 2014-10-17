using System;
using System.Text;
using System.Collections.Generic;
using System.Web;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using GDrive;
using com.andrewlinton.AzureTable;
using Music;
using MusicManager;
using GDriveConsole;
using com.andrewlinton.music;

namespace TestProject1
{
    /// <summary>
    /// Summary description for GDriveTest
    /// </summary>
    [TestClass]
    public class GDriveTest
    {
        public GDriveTest()
        {
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
        public void GetGDriveFiles()
        {
            string search = "title='Ends.mp3'";
            var gDriveService = new GDriveService();
            var files = gDriveService.GetFiles(search);
            foreach (var file in files)
            {
                testContextInstance.WriteLine("{0}", file.DownloadUrl);

				string downloadUrl = string.Format("https://googledrive.com/host/{0}", file.Id);

				WebClient webClient = new WebClient();
				string filename = System.IO.Path.GetTempFileName();
				webClient.DownloadFile(downloadUrl, filename);
				System.IO.FileInfo fileInfo = new System.IO.FileInfo(filename);
				testContextInstance.WriteLine("{0}", fileInfo.Length);
            }
        }

        [TestMethod]
        public void PopulateSongTable()
        {
            var gDriveServiceContainer = new GDriveService();
            var gdriveService = gDriveServiceContainer.GetService();
			var songRepository = new SongRepository(new Repository("log"));

            FilesResource.ListRequest request = gdriveService.Files.List();
            var fileList = new List<File>();

            try
            {
                FileList files = request.Execute();
                fileList.AddRange(files.Items);
                request.PageToken = files.NextPageToken;
                foreach (var file in fileList)
                {
                    try
                    {
                        songRepository.Set(file.Id, file.DownloadUrl);
                    }
                    catch (Exception ex1)
                    {
                        testContextInstance.WriteLine("Error: {0}", ex1.Message);
                    }
                }
            }
            catch (Exception e)
            {
                testContextInstance.WriteLine("Error: {0}", e.Message);
            }

        }

		[TestMethod]
		public void DownloadGDriveFiles()
		{
			var songRepository = new SongRepository(new Repository("log"));
			string tempFolder = @"C:\temp2\";
			string search = "title='Sunspot.mp3'";
			//			string search = "";
			var gDriveService = new GDriveService();
			var files = gDriveService.GetFiles(search);
			foreach (var file in files)
			{
				try
				{
					testContextInstance.WriteLine("{0}", file.WebContentLink);
					WebClient webClient = new WebClient();
					string filename = tempFolder + file.OriginalFilename;
					webClient.DownloadFile(file.WebContentLink, filename);

					// Extract mp3 tags
					SONG song = new SONG();
					song.LOCATION = filename;
					if (song.IsMp3File())
					{
						song.Populate();
						
						testContextInstance.WriteLine(song.GENRE);
						songRepository.Add(file.Id, file.WebContentLink, file.OriginalFilename, file.FileSize, song.TITLE, song.ARTIST, song.ALBUM, song.GENRE, "", 0, song.TrackNumber);
					}

					System.IO.File.Delete(filename);
				}
				catch (Exception ex)
				{
					testContextInstance.WriteLine(ex.Message);
				}
			}
		}

		[TestMethod]
		public void PopulateSongTable1()
		{
			var gDriveServiceContainer = new GDriveService();
			var gdriveService = gDriveServiceContainer.GetService();
			var songRepository = new SongRepository(new Repository("log"));
			int maxSongs = 10;
			int songCount = 0;
			string tempFolder = @"C:\temp2\";

			FilesResource.ListRequest request = gdriveService.Files.List();
			var fileList = new List<File>();

			try
			{
				FileList files = request.Execute();
				fileList.AddRange(files.Items);
				request.PageToken = files.NextPageToken;
				foreach (var file in fileList)
				{
					try
					{
						testContextInstance.WriteLine("{0}", file.WebContentLink);
						WebClient webClient = new WebClient();
						string filename = tempFolder + file.OriginalFilename;
						webClient.DownloadFile(file.WebContentLink, filename);

						// Extract mp3 tags
						SONG song = new SONG();
						song.LOCATION = filename;
						if (song.IsMp3File())
						{
							song.Populate();

							testContextInstance.WriteLine(song.GENRE);
							songRepository.Add(file.Id, file.WebContentLink, file.OriginalFilename, file.FileSize, song.TITLE, song.ARTIST, song.ALBUM, song.GENRE, song.LOCATION, song.RATING, song.TrackNumber);
						}

						System.IO.File.Delete(filename);
					}
					catch (Exception ex1)
					{
						testContextInstance.WriteLine("Error: {0}", ex1.Message);
					}

/*					songCount++;
					if (songCount>maxSongs)
					{
						return;
					} */
				}
			}
			catch (Exception e)
			{
				testContextInstance.WriteLine("Error: {0}", e.Message);
			}

		}

		[TestMethod]
		public void PopulateSongTable2()
		{
			var gDriveServiceContainer = new GDriveService();
			var gdriveService = gDriveServiceContainer.GetService();
			List<File> fileList = new List<File>();
			FilesResource.ListRequest request = gdriveService.Files.List();
			string tempFolder = @"E:\MusicTemp\";
			var songRepository = new SongRepository(new Repository("log"));

			do
			{
				try
				{
					FileList files = request.Execute();
					fileList.AddRange(files.Items);
					request.PageToken = files.NextPageToken;
					foreach (var file in fileList)
					{
						try
						{
							testContextInstance.WriteLine("{0}", file.WebContentLink);
							WebClient webClient = new WebClient();
							string filename = tempFolder + file.OriginalFilename;
							webClient.DownloadFile(file.WebContentLink, filename);

							// Extract mp3 tags
							SONG song = new SONG();
							song.LOCATION = filename;
							if (song.IsMp3File())
							{
								song.Populate();

								testContextInstance.WriteLine(song.GENRE);
								songRepository.Add(file.Id, file.WebContentLink, file.OriginalFilename, file.FileSize, song.TITLE, song.ARTIST, song.ALBUM, song.GENRE, song.LOCATION, song.RATING, song.TrackNumber);
							}

							System.IO.File.Delete(filename);
						}
						catch (Exception ex1)
						{
							testContextInstance.WriteLine("Error: {0}", ex1.Message);
						}
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("An error occurred: " + e.Message);
					request.PageToken = null;
				}
			} while (!String.IsNullOrEmpty(request.PageToken));
		}

		[TestMethod]
		public void PopulateGDriveQueue()
		{
			GDriveConsole.GDriveConsole gDriveConsole = new GDriveConsole.GDriveConsole();
			gDriveConsole.PopulateGDriveQueue();
		}

		[TestMethod]
		public void DrainQueue()
		{
			GDriveConsole.GDriveConsole gDriveConsole = new GDriveConsole.GDriveConsole();
			gDriveConsole.DrainQueue();
		}

		[TestMethod]
		public void InsertRecentSongs()
		{
			var gDriveConsole = new GDriveConsole.GDriveConsole();
			string search = "mimeType = 'audio/mpeg'";
			var gDriveService = new GDriveService();
			gDriveService.ProcessFiles(search, gDriveConsole.DownloadAndInsert, 100);
		}

		[TestMethod]
		public void GetSong()
		{
			string id = "0B4dXRBkWJRuDME9MWXMtTklpLUk";
			var songRepository = new SongRepository(new Repository("log"));
			var song = songRepository.Get(id);
			Assert.IsNotNull(song);
		}
    }
}
