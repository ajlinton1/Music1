using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Auth;
using Music;
using HundredMilesSoftware.UltraID3Lib;
using Azure;
using TestProject1.Properties;
using MusicManager;
using GDrive;
using com.andrewlinton.AzureTable;
using com.andrewlinton.music;

namespace TestProject1
{
	/// <summary>
	///This is a test class for MusicDaoTest and is intended
	///to contain all MusicDaoTest Unit Tests
	///</summary>
	[TestClass()]
	public class MusicDaoTest
	{
		String containerName = "public";
		string targetFolder = "e:\\Music";

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


		/// <summary>
		///A test for GetSelectedSongs
		///</summary>
		[TestMethod()]
		public void GetSelectedSongs()
		{
			UploadProcessor uploadProcessor = new UploadProcessor(containerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
			MusicDao musicDao = GetMusicDao();
			String[] desiredGenres = { "trance", "techno", "electronic", "ambient" };
			IEnumerable<SONG> actual;
			actual = musicDao.GetSelectedSongs(desiredGenres);
			foreach (SONG song in actual)
			{
				Console.WriteLine(song.TITLE);
			}
		}

		/// <summary>
		///A test for UploadSongs
		///</summary>
		[TestMethod()]
		public void UploadSongsTest()
		{
			MusicDao musicDao = GetMusicDao();
			int numSongs = 10;
			musicDao.UploadSongs(numSongs);
		}

		[TestMethod()]
		public void IndexSongsNew()
		{
			var targetFolder = "c:\\Music";

			var azureService = new AzureService(Properties.Settings.Default.AzureAccountName, Properties.Settings.Default.AzureAccountKey);
			var musicDao = new MusicDao(Settings.Default.ConnectionString, new String[] { targetFolder }, new MockUploadProcessor(), azureService);
		}

		[TestMethod()]
		public void IndexSongsNoUpload()
		{
			var targetFolder = "c:\\Music";

			var azureService = new AzureService(Properties.Settings.Default.AzureAccountName, Properties.Settings.Default.AzureAccountKey);
			var musicDao = new MusicDao(Settings.Default.ConnectionString, new String[] { targetFolder }, new MockUploadProcessor(), azureService);

			musicDao.OnError += (s) => { testContextInstance.WriteLine("Error: {0}", s); };
			musicDao.OnStatus += (s) => { testContextInstance.WriteLine("{0}", s); };
			musicDao.OnNewSong += (s) =>
			{
				testContextInstance.WriteLine("New Song: {0}", s);
			};
			musicDao.OnNewArtist += (s) => { testContextInstance.WriteLine("New Artist: {0}", s); };
			/*          musicDao.insertSong = (SONG song) =>
						  {
							  testContextInstance.WriteLine("New Song: {0}", song.LOCATION);
							  return song;
						  }; */

			musicDao.updateSongUploadStatus = (SONG song) =>
			{
				testContextInstance.WriteLine("SongId: {0}", song.ID);
				return song;
			};
			musicDao.enqueueNewSong = (SONG song) =>
				{
					testContextInstance.WriteLine("Id: {0}", song.ID);
					return song;
				};

			musicDao.CrawlFolder(targetFolder, musicDao.ProcessFileWorker);
			//            musicDao.UpdateArtists();
		}

		[TestMethod()]
		public void IndexSongsActual()
		{
			var targetFolder = "f:\\Music";

			var musicDao = GetMusicDao();
			musicDao.OnError += (s) => { testContextInstance.WriteLine("Error: {0}", s); };
			musicDao.OnStatus += (s) => { testContextInstance.WriteLine("{0}", s); };
			musicDao.OnNewSong += (s) =>
			{
				testContextInstance.WriteLine("New Song: {0}", s);
			};
			musicDao.OnNewArtist += (s) => { testContextInstance.WriteLine("New Artist: {0}", s); };

			musicDao.CrawlFolder(targetFolder, musicDao.ProcessFileWorker);
			musicDao.UpdateArtists();
		}

		[TestMethod()]
		public void IndexSongsConcurrent()
		{
			MusicDao musicDao = GetMusicDao();
			musicDao.OnNewSong += (s) => { testContextInstance.WriteLine("New Song: " + s); };
			musicDao.CrawlFolder(targetFolder, musicDao.ProcessFileWorkerConcurrent);
			musicDao.OnNewArtist += AddArtist;
			musicDao.OnStatus += Status;
			musicDao.UpdateArtists();
		}

		[TestMethod()]
		public void EnqueueFiles()
		{
			DateTime start = DateTime.Now;
			MusicDao musicDao = GetMusicDao();
			musicDao.OnError += Error;
			musicDao.OnStatus += Status;
			musicDao.CrawlFolder(targetFolder, musicDao.FilenameQueue.Enqueue);
			musicDao.ProcessQueueFilenames(5);

			while (true)
			{
				int count = musicDao.FilenameQueue.Count();
				if (count == 0)
				{
					break;
				}
				Debug.WriteLine("Count: {0}", count);
				Thread.Sleep(1000);
			}

			TimeSpan elapsed = DateTime.Now - start;
			testContextInstance.WriteLine("Elapsed: {0}", elapsed);
			//            Queue<String> filenameQueue = musicDao.FilenameQueue;
			//            testContextInstance.WriteLine("Files: {0}", filenameQueue.Count);
			//            uploadProcessor.EndProcess();
		}

		[TestMethod()]
		public void UpdateArtists()
		{
			MusicDao musicDao = GetMusicDao();
			musicDao.OnNewArtist += AddArtist;
			musicDao.OnStatus += Status;
			musicDao.UpdateArtists();
		}

		[TestMethod()]
		public void NoArtist()
		{
			MusicDao musicDao = GetMusicDao();
			musicDao.OnNewArtist += AddArtist;
			musicDao.OnStatus += Status;
			musicDao.NoArtist();
		}

		public void DoNothing(String file)
		{
			testContextInstance.WriteLine("{0}", file);
		}

		//        [TestMethod()]
		//        public void MissingTitles()
		//        {
		//            MusicDao musicDao = GetMusicDao();
		//            musicDao.OnNewSong += NewSong;
		//            musicDao.OnError += Error;
		//            musicDao.OnStatus += Status;
		//            musicDao.CrawlFoldersWMA();
		////            musicDao.CrawlFoldersMp3(NewSong, Error, Status);
		////            musicDao.UpdateArtists(AddArtist, Status);
		//        }

		public void NewSong(String newSongName)
		{
			testContextInstance.WriteLine("New Song: {0}", newSongName);
		}

		public void Error(String errorInfo)
		{
			testContextInstance.WriteLine("Error: {0}", errorInfo);
			Debug.WriteLine(errorInfo);
		}

		public void Status(String statusInfo)
		{
			testContextInstance.WriteLine("Status: {0}", statusInfo);
		}

		public void AddArtist(String artistName)
		{
			testContextInstance.WriteLine("Adding Artist: {0}", artistName);
		}

		[TestMethod()]
		public void UltraID3Test()
		{
			UltraID3 ultraID3 = new UltraID3();
			ultraID3.Read(@"e:\Music\Antenne Bayern Radio\Mirror Dawn.mp3");
			testContextInstance.WriteLine(ultraID3.Artist);
			testContextInstance.WriteLine(ultraID3.Genre);

			//ultraID3.Genre = "Rock";
			//ultraID3.Write();

			MPEGFrameInfo mpegFrameInfo = ultraID3.FirstMPEGFrameInfo;
			if (ultraID3.ID3v1Tag.ExistsInFile)
			{

			}
			ID3v2Tag id3v2Tag = ultraID3.ID3v2Tag;
			ID3FrameCollection frames = id3v2Tag.Frames;
			for (int i = 0; i < frames.Count; i++)
			{
				ID3v2Frame frame = frames[i];
			}

			id3v2Tag.Genre = "Chillout";
			ultraID3.Write();
		}


		/// <summary>
		///A test for InsertSong
		///</summary>
		[TestMethod()]
		public void InsertSongTest()
		{
			MusicDao musicDao = GetMusicDao();
			string title = "Test title";
			string artist = "Test artist";
			string album = "Test album";
			string genre = "crap";
			Nullable<long> duration = new Nullable<long>(100);
			Nullable<long> bitrate = new Nullable<long>(5);
			Nullable<long> filesize = new Nullable<long>(200);
			string location = @"e:\music\test.mp3";
			byte[] hash = new byte[1];
			hash[0] = new byte();
			SONG song = new SONG() { TITLE = title, ARTIST = artist, ALBUM = album, GENRE = genre, DURATION = duration, BITRATE = bitrate, FILESIZE = filesize, LOCATION = location, HASH = hash };
			musicDao.InsertSong(song);
		}

		[TestMethod()]
		public void SyncSongUploadedStatusWithBlobs()
		{
			AzureService azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
			MusicDao musicDao = GetMusicDao();
			CloudBlobClient blobClient = azureService.GetCloudBlobContainer();
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);
			IEnumerable<IListBlobItem> blobs = container.ListBlobs();
			foreach (IListBlobItem blob in blobs)
			{
				testContextInstance.WriteLine("{0}", blob.Uri);
				String[] s = blob.Uri.ToString().Split(new char[] { '/' });
				String key = s[s.Length - 1];
				s = key.Split(new char[] { '.' });
				testContextInstance.WriteLine("{0}", s[0]);
				try
				{
					int id = Convert.ToInt32(s[0]);
					var song = new SONG() { ID = id, UPLOADED = true };
					musicDao.UpdateSongUploadStatus(song);
				}
				catch (Exception ex)
				{
					testContextInstance.WriteLine("{0} {1}", ex.Message, ex.StackTrace);
				}
			}
		}

		[TestMethod()]
		public void DownloadAllSongBlobs()
		{
			AzureService azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
			MusicDao musicDao = GetMusicDao();
			CloudBlobClient blobClient = azureService.GetCloudBlobContainer();
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);
			IEnumerable<IListBlobItem> blobs = container.ListBlobs();
			foreach (IListBlobItem blob in blobs)
			{
				testContextInstance.WriteLine("{0}", blob.Uri);
				String[] s = blob.Uri.ToString().Split(new char[] { '/' });
				String key = s[s.Length - 1];
				s = key.Split(new char[] { '.' });
				testContextInstance.WriteLine("{0}", s[0]);
				try
				{
					int id = Convert.ToInt32(s[0]);
					SONG song = musicDao.GetSongById(id);
					FileInfo fileInfo = new FileInfo(song.LOCATION);
					s = song.LOCATION.Split(new char[] { '\\' });
					String artistFolder = null;
					String name = fileInfo.Name;
					String fileName = null;
					if (s.Length > 3)
					{
						artistFolder = targetFolder;
						for (int i = 2; i < s.Length - 1; i++)
						{
							artistFolder = artistFolder + "\\" + s[i];
							DirectoryInfo directoryInfo = new DirectoryInfo(artistFolder);
							if (!directoryInfo.Exists)
							{
								Directory.CreateDirectory(artistFolder);
							}
						}
						fileName = String.Format("{0}\\{1}", artistFolder, name);
					}
					else
					{
						fileName = String.Format("{0}\\{1}", targetFolder, name);
					}

					if (!File.Exists(fileName))
					{
						string key1 = String.Format("{0}.mp3", song.ID);
						azureService.DownloadFile(containerName, fileName, key1);
					}
				}
				catch (Exception ex)
				{
					testContextInstance.WriteLine("{0} {1}", ex.Message, ex.StackTrace);
					throw;
				}
			}
		}

		[TestMethod()]
		public void DeleteSongBlobs()
		{
			AzureService azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
			CloudBlobClient blobClient = azureService.GetCloudBlobContainer();
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);
			IEnumerable<IListBlobItem> blobs = container.ListBlobs();
			foreach (IListBlobItem blob in blobs)
			{
				testContextInstance.WriteLine("{0}", blob.Uri);
				String[] s = blob.Uri.ToString().Split(new char[] { '/' });
				String key = s[s.Length - 1];
				testContextInstance.WriteLine("{0}", key);
				try
				{
					azureService.DeleteBlob(containerName, key);
				}
				catch (Exception ex)
				{
					testContextInstance.WriteLine("{0} {1}", ex.Message, ex.StackTrace);
				}
			}

			// TODO: update song set uploaded = 0 where uploaded = 1
		}

		[TestMethod()]
		public void SetUploadedAttributeOnSongs()
		{
			AzureService azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
			CloudBlobClient blobClient = azureService.GetCloudBlobContainer();
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);
			IEnumerable<IListBlobItem> blobs = container.ListBlobs();
			foreach (var blob in blobs)
			{
				testContextInstance.WriteLine("{0}", blob.Uri);
				String[] s = blob.Uri.ToString().Split(new char[] { '/' });
				String key = s[s.Length - 1];
				s = key.Split(new char[] { '.' });
				key = s[0];
				testContextInstance.WriteLine("{0}", key);
				try
				{
					int id = Convert.ToInt32(key);
					using (var dataContext = new MusicEntities1())
					{
						var songs = from song in dataContext.SONG where song.ID == id select song;
						foreach (var s1 in songs)
						{
							s1.UPLOADED = true;
							dataContext.SaveChanges();
						}
					}
				}
				catch (Exception ex)
				{
					testContextInstance.WriteLine("{0} {1}", ex.Message, ex.StackTrace);
				}
			}
		}

		[TestMethod()]
		public void QueueTest()
		{
			AzureService azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);

			CloudQueueClient cloudQueueClient = azureService.GetCloudQueueClient();

			IEnumerable<CloudQueue> queues = cloudQueueClient.ListQueues();
			foreach (var queue in queues)
			{
				testContextInstance.WriteLine("{0}", queue.Name);
			}

			CloudQueue queue1 = cloudQueueClient.GetQueueReference("testqueue");

			//Check whether the queue exists, and create it if it does not.
			if (!queue1.Exists())
			{
				queue1.Create();
			}

			CloudQueueMessage message = new CloudQueueMessage("This is a test");
			queue1.AddMessage(message);

			String songInfo = String.Format("{0},{1}", @"c:\music\song.mp3", 34);
			message = new CloudQueueMessage(songInfo);
			queue1.AddMessage(message);

			while (true)
			{
				CloudQueueMessage message1 = queue1.GetMessage();
				if (null == message1)
				{
					break;
				}
				testContextInstance.WriteLine("{0}", message1.AsString);
				queue1.DeleteMessage(message1);
			}
		}

//		[TestMethod()]
		public void DrainUploadQueue()
		{
			AzureService azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
			String uploadQueueName = "uploadqueue";
			CloudQueueClient cloudQueueClient = azureService.GetCloudQueueClient();
			CloudQueue uploadQueue = cloudQueueClient.GetQueueReference(uploadQueueName);

			while (true)
			{
				CloudQueueMessage message1 = uploadQueue.GetMessage();
				if (null == message1)
				{
					break;
				}
				testContextInstance.WriteLine("{0}", message1.AsString);
				uploadQueue.DeleteMessage(message1);
			}
		}

		[TestMethod]
		public void ListBlobs()
		{
			AzureService azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);

			CloudBlobClient blobClient = azureService.GetCloudBlobContainer();
			CloudBlobContainer container = blobClient.GetContainerReference(containerName);
			IEnumerable<IListBlobItem> blobs = container.ListBlobs();
			foreach (var blob in blobs)
			{
				testContextInstance.WriteLine("{0}", blob.Uri);
			}
		}

		[TestMethod]
		public void UpdateRecentSongs()
		{
			using (var dataContext = new MusicEntities1())
			{
				var songs = from song in dataContext.SONG where song.ID > 53520 select song;
				foreach (var s in songs)
				{
					testContextInstance.WriteLine("{0} : {1} : {2}", s.ARTIST, s.TITLE, s.UPLOADED);
				}
			}
		}

		/// <summary>
		/// Test file indexing
		///</summary>
		[TestMethod()]
		public void ProcessFileWorkerTest()
		{
			string pConnString = Settings.Default.ConnectionString;
			MusicDao musicDao = GetMusicDao();
			musicDao.OnNewSong += NewSong;
			musicDao.OnError += Error;
			musicDao.OnStatus += Status;
			string filename = @"E:\\Music\\Audioslave\\Audioslave - Cochise.mp3";
			musicDao.ProcessFileWorker(filename);
		}

		[TestMethod()]
		public void CrawlFolderSTest()
		{
			var folder = "e:\\Music\\KEXP";
			Func<SONG, SONG> songFunc = (SONG song) =>
			{
				Console.WriteLine(song.LOCATION);
				return song;
			};
			var musicDao = GetMusicDao();
			musicDao.CrawlFolderS(folder, songFunc);
		}

		[TestMethod()]
		public void ProcessSongFiles()
		{
			int maxSongsToInsert = 10;
			//            var folder = "e:\\Music";
			var folder = @"E:\Gdrive\Music";
			//var folder = @"d:\Music";

			var jobLogRepository = new JobLogRepository();
			var lastIndexRun = jobLogRepository.GetLatest();
			int songsInserted = 0;

			//            lastIndexRun.DateRun = new DateTime(2014, 2, 20);

			/*            Func<SONG, SONG> songFunc = (SONG song) =>
						{
							Console.WriteLine(song.LOCATION);
							return song;
						}; */

			var musicDao = GetMusicDao();

			var allSongFiles = musicDao.GetSongFiles(folder);
			var musicFiles = from musicFile in allSongFiles
							 where musicFile.IsMusicFile()
							 //                             where musicFile.IsMp3File()
							 select musicFile;

			/*            var targetFilename = @"Pale Green Ghosts";
						musicFiles = from musicFile in musicFiles
									 where musicFile.LOCATION.IndexOf(targetFilename) > -1
									 select musicFile; */

			musicFiles = from musicFile in musicFiles
						 select musicFile.NormalizeFilename();

			var musicFilesModifiedDate = from musicFile in musicFiles
										 select new
										 {
											 song = musicFile,
											 modificationDate = musicFile.GetModificationDate()
										 };

			//                         musicFile.CalculateModificationDate();

			var modifiedFiles = from musicFile in musicFilesModifiedDate
								where (musicFile.modificationDate > lastIndexRun.DateRun)
								select musicFile.song;

			/*            foreach (var musicFile in modifiedFiles)
						{
							testContextInstance.WriteLine("{0} {1}", musicFile.UPDATED, musicFile.LOCATION);
						} */

			modifiedFiles = from musicFile in modifiedFiles
							select musicFile.Populate();

			foreach (var modifiedFile in modifiedFiles)
			{
				if (modifiedFile.FILESIZE==null || modifiedFile.FILESIZE == 0)
				{
					testContextInstance.WriteLine("{0}", "Error");
				}
				testContextInstance.WriteLine("{0}", modifiedFile.FILESIZE);
			}

/*			var incompleteFiles = from musicFile in modifiedFiles
								  where !musicFile.TagsComplete()
								  select musicFile;*/

			modifiedFiles = from musicFile in modifiedFiles
							where musicFile.TagsComplete()
							select musicFile;

			var songsWithDbInfo = from song in modifiedFiles
								  select new
								  {
									  song = song,
									  existsInDb = song.InDb(musicDao)
								  };

			var newFiles = from song in songsWithDbInfo
						   where !song.existsInDb
						   select song;

/*			            foreach (var newFile in newFiles)
						{
							testContextInstance.WriteLine("{0}", newFile.song.LOCATION);
						} */

			var insertedSongs = from song in newFiles
								select musicDao.InsertSong(song.song);

			songsInserted = insertedSongs.ToArray().Count();

/*			var updatedFiles = from song in songsWithDbInfo
							   where song.existsInDb
							   select musicDao.UpdateSong(song.song.TITLE, song.song.ARTIST, song.song.ALBUM, song.song.GENRE, 0, 0, song.song.FILESIZE, song.song.LOCATION);

			int numUpdatedFiles = updatedFiles.ToArray().Count();
			songsInserted = insertedSongs.ToArray().Count(); */

			/*            foreach (var song in updatedFiles)
						{
							testContextInstance.WriteLine("Updated: {0}", song.song.LOCATION);
			//                musicDao.UpdateSong(song.song.TITLE, song.song.ARTIST, song.song.ALBUM, song.song.GENRE, 0, 0, 0, song.song.LOCATION, song.song.HASH.ToArray());
						} 

						foreach (var file in incompleteFiles)
						{
							testContextInstance.WriteLine("Incomplete tags: {0} ", file.LOCATION);
						} */

			/*            jobLogRepository.Add(songsInserted.ToString()); */
		}

		[TestMethod()]
		public void GetSongsSimple()
		{
			using (var context = new MusicEntities1())
			{
				var results = from song in context.SONG
							  where song.ARTIST.IndexOf("Lucius") > -1
							  select song;

				foreach (var song in results)
				{
					testContextInstance.WriteLine("{0}", song.LOCATION);
				}
			}
		}

		[TestMethod()]
		public void AddJobLog()
		{
			var jobLogRepository = new JobLogRepository();
			jobLogRepository.Add(DateTime.Now.ToLongTimeString());
			var jobLog = jobLogRepository.GetLatest();
		}

		[TestMethod()]
		public void DeleteUploadedSongs()
		{
			var azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);

			using (var dataContext = new MusicEntities1())
			{
				var uploadedSongs = from song in dataContext.SONG
									where song.UPLOADED
									select song;

				foreach (var song in uploadedSongs)
				{
					var key = song.ID + ".mp3";

					try
					{
						azureService.DeleteBlob("public", key);
					}
					catch (Exception ex)
					{
						if (ex.Message.IndexOf("The specified blob does not exist") == -1)
						{
							throw;
						}
					}

					song.UPLOADED = false;
				}
				dataContext.SaveChanges();
			}

		}

		[TestMethod()]
		public void PopulateTags()
		{
			string targetFilename = @"region008.mp3";
			string folder = @"E:\Music\KEXP\20140216\Street Sounds\";
			var musicDao = GetMusicDao();
			var allSongFiles = musicDao.GetSongFiles(folder);

			var musicFiles = from musicFile in allSongFiles
							 where musicFile.LOCATION.IndexOf(targetFilename) > -1
							 select musicFile;

			musicFiles = from musicFile in musicFiles
						 select musicFile.Populate();

			var results = musicFiles.ToArray();
		}

		[TestMethod()]
		public void UploadSongs()
		{
			string targetLocation = "Natasha Kmeto";
			var uploadProcessor = new UploadProcessor(containerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
			uploadProcessor.OnError += (s) => { testContextInstance.WriteLine("Error: {0}", s); };
			uploadProcessor.OnStatus += (s) => { testContextInstance.WriteLine("{0}", s); };

			using (var dataContext = new MusicEntities1())
			{
				var songs = from song in dataContext.SONG
							where song.LOCATION.IndexOf(targetLocation) > -1
							select song;

				songs = from song in songs
						where !song.UPLOADED
						select song;

				foreach (var song in songs)
				{
					// Change drive to C:
					if (song.LOCATION.ToLower()[0] != 'c')
					{
						song.LOCATION = song.LOCATION.Substring(1);
						song.LOCATION = "c" + song.LOCATION;
					}

					testContextInstance.WriteLine("{0}", song.LOCATION);
					var fileinfo = new Tuple<string, string>(song.LOCATION, song.ID.ToString());
					if (uploadProcessor.UploadFile(fileinfo))
					{
						song.UPLOADED = true;
					}
				}
				dataContext.SaveChanges();
			}
		}

		[TestMethod()]
		public void InDbTest()
		{
			var musicDao = GetMusicDao();
			string filename = @"E:\Gdrive\Music\KEXP\20130825\02_Web radio - Unknown track.mp3";
			var inDb = musicDao.InDb(filename);
		}

		[TestMethod]
		public void PopulateSegments()
		{
			var segments = new List<string>();
			var segmentRepository = new SegmentRepository();

			using (var context = new MusicEntities1())
			{
				var songs = from song in context.SONG
							where song.GDRIVE != null
							orderby song.LOCATION
							select song;

				foreach (var song in songs)
				{
					try
					{
						int start = song.LOCATION.IndexOf("\\Music\\") + 7;
						if (start > -1)
						{
							int end = song.LOCATION.LastIndexOf('\\');
							string segment = song.LOCATION.Substring(start, end - start);
							if (!segments.Contains(segment))
							{
								string rowKey = segment.Replace('\\', '_');
								segmentRepository.Set(song.GENRE, rowKey, segment);
								segments.Add(segment);
							}
						}
					}
					catch (Exception ex)
					{
						testContextInstance.WriteLine("{0}", ex.Message);
					}
				}
			}

		}

		[TestMethod]
		public void PopulateSegmentTest()
		{
			string segment = @"KEXP\2010.03.28";
			string rowKey = segment.Replace('\\', '_');
			//            AzureTableService.AddSegment(rowKey, segment, "", "");
		}

/*		[TestMethod]
		public void PopulateGenres()
		{
			var genres = new Dictionary<string, int>();
			var genreRepository = new GenreRepository();

			using (var context = new MusicEntities1())
			{
				var songs = from song in context.SONG
							where song.GDRIVE != null
							orderby song.GENRE
							select song;

				foreach (var song in songs)
				{
					if (!genres.ContainsKey(song.GENRE))
					{
						genres[song.GENRE] = 1;
					}
					else
					{
						genres[song.GENRE]++;
					}
				}

				foreach (KeyValuePair<string, int> kvp in genres)
				{
					try
					{
						genreRepository.SetGenre(kvp.Key, kvp.Value.ToString());
					}
					catch (Exception ex)
					{
						testContextInstance.WriteLine("{0}", ex.Message);
					}
				}
			}

		} */

		[TestMethod]
		public void ResetGenres()
		{
			var genreRepository = new GenreRepository();
			genreRepository.ResetGenres();
		}

		[TestMethod]
		public void AddIdToFilename()
		{
			SONG song = new SONG();
			song.LOCATION = @"E:\Gdrive\Music\Franz Ferdinand\Right Thoughts\Ulysses.mp3";
			song.ID = 99;
			song.AddIdToFilename();
		}

		[TestMethod]
		public void PopulateArtists()
		{
			var artists = new Dictionary<string, int>();
			var artistRepository = new ArtistRepository();

			artistRepository.Reset();

			using (var context = new MusicEntities1())
			{
				var songs = from song in context.SONG
							where song.GDRIVE != null
							orderby song.ARTIST
							select song;

				foreach (var song in songs)
				{
					if (!artists.ContainsKey(song.ARTIST))
					{
						artists[song.ARTIST] = 1;
					}
					else
					{
						artists[song.ARTIST]++;
					}
				}

				foreach (KeyValuePair<string, int> kvp in artists)
				{
					try
					{
						artistRepository.Set(kvp.Key, kvp.Value.ToString());
					}
					catch (Exception ex)
					{
						testContextInstance.WriteLine("{0}", ex.Message);
					}
				}
			}

		}

		[TestMethod]
		public void AddIdsToFilenames()
		{
			using (var context = new MusicEntities1())
			{
				var songs = from song in context.SONG
							where song.GDRIVE == null
							orderby song.ID descending
							select song;

				var songsArray = songs.ToArray().Take(200);

				foreach (var song in songsArray)
				{
					int id = song.GetFilenameId();
					testContextInstance.WriteLine("{0}", song.LOCATION);
				}
			}

		}

		[TestMethod]
		public void SetGDriveUrls()
		{
			string folder = @"E:\Gdrive\Music";
			string musicFolder = @"E:\Gdrive\Music";
			var musicDao = GetMusicDao();
			var allSongFiles = musicDao.GetSongFiles(folder);

			var musicFiles = from musicFile in allSongFiles
							 where musicFile.IsMp3File()
							 select musicFile;

			var songsWithDbInfo = from song in musicFiles
								  select musicDao.GetSongByLocation(song.LOCATION);

			var existingFiles = from song in songsWithDbInfo
								where song != null
								select song;

			existingFiles = from song in existingFiles
							where song.GDRIVE == null
							select song;

			var gDriveService = new GDriveService();

			foreach (var song in existingFiles)
			{
				char[] c = { '\\' };
				string[] s = song.LOCATION.Split(c);
				string filename = s[s.Length - 1];
				string search = string.Format("title='{0}'", filename);
				var files = gDriveService.GetFiles(search);
				foreach (var file in files)
				{
					testContextInstance.WriteLine("{0}", file.WebContentLink);

					try
					{
						FileInfo fileInfo = null;
						if (files.Count > 1)
						{
							int start = song.LOCATION.IndexOf("\\Music\\");
							string adjustedFilename = musicFolder + song.LOCATION.Substring(start + 6);
							fileInfo = new FileInfo(adjustedFilename);
						}
						if ((fileInfo == null) || (fileInfo.Length == file.FileSize))
						{
							testContextInstance.WriteLine("Found");
							using (var context = new MusicEntities1())
							{
								var currentSong = context.SONG.Single(s1 => s1.ID == song.ID);
								currentSong.GDRIVE = file.WebContentLink;
								context.SaveChanges();
								testContextInstance.WriteLine("{0} {1}", song.LOCATION, file.DownloadUrl);
							}
						}
					}
					catch (Exception ex)
					{
						testContextInstance.WriteLine("Error: {0} {1}", filename, ex.Message);
					}
				}

			}
		}

		public void SetGDriveUrl(Google.Apis.Drive.v2.Data.File file)
		{
			testContextInstance.WriteLine("{0}", file.OriginalFilename);

			using (var context = new MusicEntities1())
			{
				var songs = from s in context.SONG
							where ((s.LOCATION.Contains(file.OriginalFilename)) && (s.FILESIZE == file.FileSize))
							select s;
				var songArray = songs.ToArray();
				foreach (var song in songArray)
				{
					song.GDRIVE = file.WebContentLink;
					context.SaveChanges();
					testContextInstance.WriteLine("Updated: {0} {1}", song.LOCATION, file.WebContentLink);
				}

			}

		}

		[TestMethod]
		public void SetGDriveUrls1()
		{
			var gDriveService = new GDriveService();
			string search = "title contains '.mp3'";
			gDriveService.ProcessFiles(search, SetGDriveUrl, null);
		}

		int gDriveFileCount = 0;

		public void CountFile(Google.Apis.Drive.v2.Data.File file)
		{
			gDriveFileCount++;
			testContextInstance.WriteLine("Count: {0}", gDriveFileCount);
		}

		[TestMethod]
		public void CountGDriveFiles()
		{
			var gDriveService = new GDriveService();
			//            string search = "title contains '.mp3'";
			//            string search = "mimeType contains 'mp3'";
			string search = "";
			gDriveService.ProcessFiles(search, CountFile, null);
			testContextInstance.WriteLine("Count: {0}", gDriveFileCount);
		}

		[TestMethod]
		public void SetFilesize()
		{
			string folder = @"E:\Gdrive\Music";
			var musicDao = GetMusicDao();
			var allSongFiles = musicDao.GetSongFiles(folder);

			var musicFiles = from musicFile in allSongFiles
							 where musicFile.IsMp3File() && (musicFile.FILESIZE == 0 || musicFile.FILESIZE == null) && musicFile.GDRIVE == null
							 select musicFile;

			foreach (var song in musicFiles)
			{
				try
				{
					var fileInfo = new FileInfo(song.LOCATION);

					var updated = musicDao.UpdateSongByLocation(song.LOCATION, (SONG s) => { s.FILESIZE = fileInfo.Length; });
					if (!updated)
					{
						testContextInstance.WriteLine("File not found in db {0}", song.LOCATION);
					}
				}
				catch (Exception ex)
				{
					testContextInstance.WriteLine("{0}, {1}", song.LOCATION, ex.Message);
				}
			}
		}

		[TestMethod]
		public void CleanUpFilenames()
		{
			string folder = @"E:\Gdrive\Music";
			var musicDao = GetMusicDao();
			var allSongFiles = musicDao.GetSongFiles(folder);

			var musicFiles = from musicFile in allSongFiles
							 where musicFile.IsMp3File()
							 select musicFile;

			musicFiles = from musicFile in musicFiles
						 where (musicFile.LOCATION.IndexOf(",") > -1) || (musicFile.LOCATION.IndexOf("+") > -1) || (musicFile.LOCATION.IndexOf(";") > -1)
						 select musicFile;

			foreach (var song in musicFiles)
			{
				try
				{
					StringBuilder newFilename = new StringBuilder();
					for (int i = 0; i < song.LOCATION.Length; i++)
					{
						if ((song.LOCATION[i] != ',') && (song.LOCATION[i] != '+') && (song.LOCATION[i] != ';'))
						{
							newFilename.Append(song.LOCATION[i]);
						}
					}

					if (!File.Exists(newFilename.ToString()))
					{
						File.Copy(song.LOCATION, newFilename.ToString());
					}
					File.Delete(song.LOCATION);
				}
				catch (Exception ex)
				{
					testContextInstance.WriteLine("{0}, {1}", song.LOCATION, ex.Message);
				}
			}
		}

		public MusicDao GetMusicDao()
		{
			var uploadProcessor = new UploadProcessor(containerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
			uploadProcessor.OnError += (s) => { testContextInstance.WriteLine("Error: {0}", s); };
			uploadProcessor.OnStatus += (s) => { testContextInstance.WriteLine("{0}", s); };
			var azureService = new AzureService(Properties.Settings.Default.AzureAccountName, Properties.Settings.Default.AzureAccountKey);
			var musicDao = new MusicDao(Settings.Default.ConnectionString, new String[] { targetFolder }, uploadProcessor, azureService);
			musicDao.OnError += (s) => { testContextInstance.WriteLine("Error: {0}", s); };
			musicDao.OnStatus += (s) => { testContextInstance.WriteLine("{0}", s); };
			musicDao.OnNewSong += (s) =>
			{
				testContextInstance.WriteLine("New Song: {0}", s);
			};
			musicDao.OnNewArtist += (s) => { testContextInstance.WriteLine("New Artist: {0}", s); };

			return musicDao;
		}
	}
}
