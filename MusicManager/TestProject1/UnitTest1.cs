using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Music;
using MusicManager;
using Azure;
using TestProject1.Properties;
using com.andrewlinton.music;

namespace TestProject1
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
//        String connString = "Data Source=76.12.221.204;Initial Catalog=mycruitercom;User Id=mycruitercom;Password=mycYRs8a;";
//        String connString = "Data Source=[Account];Initial Catalog=Music;User Id=SqlAzure;Password=[Password];";
        
        public UnitTest1()
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
        public void InsertSongContent()
        {
            var azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            var uploadProcessor = new UploadProcessor(Settings.Default.AzureContainerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            var musicDao = new MusicDao(Settings.Default.ConnectionString, null, uploadProcessor, azureService);
            musicDao.Initialize();
            musicDao.InsertSongContent();
        }

        [TestMethod]
        public void GetSongContent()
        {
            var azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            var uploadProcessor = new UploadProcessor(Settings.Default.AzureContainerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            var musicDao = new MusicDao(Settings.Default.ConnectionString, null, uploadProcessor, azureService);
            musicDao.Initialize();
            musicDao.GetSongContent();
        }

        [TestMethod]
        public void ChangeLocations()
        {
            try
            {
                var dataContext = new MusicEntities1();
                var songs = dataContext.SONG;

                foreach (SONG song in songs)
                {
                    Console.WriteLine(song.LOCATION);
                    song.LOCATION = song.LOCATION.ToLower();
                    if (song.LOCATION.IndexOf("d:") > -1)
                    {
                        song.LOCATION = song.LOCATION.Replace("d:", "c:");
                        dataContext.SaveChanges();
                    }
                    if (song.LOCATION.IndexOf("e:") > -1)
                    {
                        song.LOCATION = song.LOCATION.Replace("e:", "c:");
                        dataContext.SaveChanges();
                    }
                    if (song.LOCATION.IndexOf("f:") > -1)
                    {
                        song.LOCATION = song.LOCATION.Replace("f:", "c:");
                        dataContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        [TestMethod]
        public void DeleteMissingFiles()
        {
            var dataContext = new MusicEntities1();
            var songs = dataContext.SONG;

            foreach (SONG song in songs)
            {
                try
                {
                    if (!File.Exists(song.LOCATION))
                    {
                        testContextInstance.WriteLine(song.LOCATION);
                        dataContext.SONG.Remove(song);
                        dataContext.SaveChanges();
                    }
                    else
                    {
                        // TODO: Compare tags in db against file 
                        byte[] hash = MusicDao.GenerateHashCode(song.LOCATION);

                        bool same = true;
                        for (int i = 0; i < hash.Length; i++)
                        {
                            if (hash[i] != song.HASH.ToArray()[i])
                            {
                                same = false;
                            }
                        }

                        if (!same)
                        {
                            testContextInstance.WriteLine(song.LOCATION);
                            dataContext.SONG.Remove(song);
                            dataContext.SaveChanges();
                        }
                    }
                }
                catch (Exception e)
                {
                    testContextInstance.WriteLine(e.Message);
                }
            }
            dataContext.SaveChanges();
        }

        [TestMethod]
        public void UpdateSongMetadata()
        {
            try
            {
                var azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
                var uploadProcessor = new UploadProcessor(Settings.Default.AzureContainerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
                var musicDao = new MusicDao(Settings.Default.ConnectionString, null, uploadProcessor, azureService);
                var dataContext = new MusicEntities1();
                var songs = dataContext.SONG;

                foreach (SONG song in songs)
                {
                    try
                    {
                        if (song.LOCATION.ToUpper().StartsWith("C:"))
                        {
                            song.LOCATION = song.LOCATION.Remove(0, 2);
                            song.LOCATION = "D:" + song.LOCATION;
                        }
                        if (File.Exists(song.LOCATION))
                        {
                            String title, artist, album, genre;
                            musicDao.GetSongMetadata(song.LOCATION, out title, out artist, out album, out genre);
                            if ((String.IsNullOrEmpty(genre)) && (genre != song.GENRE))
                            {
                                song.GENRE = genre;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        testContextInstance.WriteLine(song.LOCATION);
                    }
                }
                dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                testContextInstance.WriteLine(ex.Message);
                testContextInstance.WriteLine(ex.StackTrace);
            }
        }

        [TestMethod]
        public void UploadSongs()
        {
            try
            {
                var azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
                var uploadProcessor = new UploadProcessor(Settings.Default.AzureContainerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
                var musicDao = new MusicDao(Settings.Default.ConnectionString, null, uploadProcessor, azureService);
                musicDao.Initialize();

                for (int i = 0; i < 10; i++)
                {
                    SONG song = musicDao.PickRandomSong(null);
                    FtpManager.Upload(song.LOCATION, i);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        [TestMethod]
        public void ListSongs()
        {
            IEnumerable<String> songs = FtpManager.List();

            foreach (String song in songs)
            {
                testContextInstance.WriteLine(song);
            }
        }

        [TestMethod]
        public void DeleteSongs()
        {
            try
            {
                IEnumerable<String> songs = FtpManager.List();

                foreach (String song in songs)
                {
                    if (song.Length > 0)
                    {
                        FtpManager.Delete(song);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public bool Ambient(SONG song)
        {
            if (song.GENRE == null)
            {
                return false;
            }
            return song.GENRE.ToUpper().Contains("AMBIENT");
        }

        [TestMethod]
        public void PickRandomSongs()
        {
            try
            {
                var azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
                var uploadProcessor = new UploadProcessor(Settings.Default.AzureContainerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
                var musicDao = new MusicDao(Settings.Default.ConnectionString, null, uploadProcessor, azureService);
                musicDao.Initialize();
                
                for (int i = 0; i < 10; i++)
                {
                    SONG song = musicDao.PickRandomSong((s) => { return s.GENRE.Equals("Ambient"); });
                    Console.WriteLine(song.LOCATION);
                    FtpManager.Upload(song.LOCATION, i);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

/*        [TestMethod]
        public void UploadArtistsSongs()
        {
            try
            {
                var azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
                var uploadProcessor = new UploadProcessor(Settings.Default.AzureContainerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
                var musicDao = new MusicDao(Settings.Default.ConnectionString, null, uploadProcessor, azureService);

                // Get ArtistId from ArtistLoadRequest
                int artistId = musicDao.DesiredArtist();

                // Get Songs by the requested artist
                List<SONG> songs = musicDao.GetSongsByArtist(artistId);

                // Delete existing Songs
                try
                {
                    FtpManager.DeleteAll();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                // Upload Songs
                foreach (SONG song in songs)
                {
                    FtpManager.Upload(song.LOCATION, song.ID);
                    song.UPLOADED = true;
                    musicDao.UpdateSongUploadStatus(song);
                }

                // Mark ArtistRequest as completed
                musicDao.SetDesiredArtistLoaded();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        } */

        [TestMethod]
        public void SelectSongs()
        {
            String[] desiredGenres = { "trance", "techno", "electronic", "ambient" ,"house",
                                     "beats", "big beat", "chill", "dub", "Electro", "grime", "hard psyche",
                                     "illbient", "jungle", "leftfield", "plunderphonic", "trip", "turn"};
            var azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            var uploadProcessor = new UploadProcessor(Settings.Default.AzureContainerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            var musicDao = new MusicDao(Settings.Default.ConnectionString, null, uploadProcessor, azureService);
            IEnumerable<SONG> songs = musicDao.GetSelectedSongs(desiredGenres);
            Assert.IsTrue(songs.GetEnumerator().MoveNext());
            foreach (SONG song in songs)
            {
                testContextInstance.WriteLine(song.TITLE);
            }
        }

        [TestMethod]
        public void TestUploadedSongs()
        {
            try
            {
                List<int> songIds = new List<int>();
                IEnumerable<String> uploadedSongs = FtpManager.List();
                foreach (String uploadedSong in uploadedSongs)
                {
                    String songIdString = uploadedSong.Substring(5);
                    songIdString = songIdString.Remove(songIdString.IndexOf("."));
                    int songId = Convert.ToInt32(songIdString);
                    songIds.Add(songId);
                }

                var dataContext = new MusicEntities1();
                var songs = from row in dataContext.SONG
                            where (row.UPLOADED==true)
                            select row;
                foreach (SONG song in songs)
                {
                    if (!songIds.Contains(song.ID))
                    {
                        Console.WriteLine(song.TITLE);
                        song.UPLOADED = false;
                    }
                }
                dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        [TestMethod]
        public void GetProcSongs()
        {
            try
            {
                var azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
                var uploadProcessor = new UploadProcessor(Settings.Default.AzureContainerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
                var musicDao = new MusicDao(Settings.Default.ConnectionString, null, uploadProcessor, azureService);
                musicDao.GetProcSongs();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

/*        [TestMethod]
        public void GetProcSongsLing()
        {
            var azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            var uploadProcessor = new UploadProcessor(Settings.Default.AzureContainerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            var musicDao = new MusicDao(Settings.Default.ConnectionString, null, uploadProcessor, azureService);
            List<SONG> songs = musicDao.GetProcSongsLinq();
            foreach (SONG song in songs)
            {
                Console.WriteLine(song.TITLE);
            }
        } */

        [TestMethod]
        public void GetSongMetadata()
        {
            String filename, title, artist, album, genre;

            filename = @"";
            var azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            var uploadProcessor = new UploadProcessor(Settings.Default.AzureContainerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            var musicDao = new MusicDao(Settings.Default.ConnectionString, null, uploadProcessor, azureService);
            musicDao.GetMp3Tags(filename, out title, out artist, out album, out genre);
            Assert.IsNotNull(title);
            Assert.IsNotNull(artist);
            Assert.IsNotNull(album);
            Assert.IsNotNull(genre);
        }

        [TestMethod]
        public void GetSongMetadata1()
        {
            String filename, title, artist, album, genre;

            filename = @"e:\Music\Apollo Four Fourty\Electro Glide In Blue\Carrera Rapida.mp3";
            var azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            var uploadProcessor = new UploadProcessor(Settings.Default.AzureContainerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            var musicDao = new MusicDao(Settings.Default.ConnectionString, null, uploadProcessor, azureService);
            musicDao.GetMp3Tags(filename, out title, out artist, out album, out genre);
            Assert.IsNotNull(title);
            Assert.IsNotNull(artist);
            Assert.IsNotNull(album);
            Assert.IsNotNull(genre);
        }

        [TestMethod]
        public void RatedSongs()
        {
            try
            {
                var dataContext = new MusicEntities1();
                var songs = dataContext.SONG;

                foreach (SONG song in songs)
                {
                    int inserts = 0;
                    inserts = inserts + song.RATING;

                    using (SqlConnection con = new SqlConnection(Settings.Default.ConnectionString))
                    {
                        con.Open();
                        String sql = "INSERT INTO Song_Rated (Song_Id) VALUES (@SongId)";
                        SqlCommand command = new SqlCommand(sql, con);
                        command.Parameters.AddWithValue("@SongId", song.ID);
                        for (int i = 1; i < inserts; i++)
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                testContextInstance.WriteLine(ex.Message);
                testContextInstance.WriteLine(ex.StackTrace);
                throw;
            }
        }

/*        [TestMethod]
        public void SongRatingUpdate()
        {
            var dataContext = new MusicEntities();
            var songs = dataContext.SONGs;

            foreach (SONG song in songs)
            {
                try
                {
                    String dbGenre = song.GENRE;
                    var azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);

                    var uploadProcessor = new UploadProcessor(Settings.Default.AzureContainerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);

                    String fileGenre = song.GetFileGenre(Settings.Default.ConnectionString, azureService, uploadProcessor);
                    if (String.Compare(dbGenre,fileGenre,true) != 0)
                    {
                        testContextInstance.WriteLine("{0} {1} {2}", song.LOCATION, song.GENRE, fileGenre);
                        song.GENRE = fileGenre;
                    }
                }
                catch (Exception ex)
                {
                    testContextInstance.WriteLine("{0} {1}", song.LOCATION, ex.Message);
                }
            }
            dataContext.SubmitChanges();
        } */

        [TestMethod()]
        public void GenerateHashCodes()
        {
            var dataContext = new MusicEntities1();

            IQueryable<SONG> songQuery = from s in dataContext.SONG select s;

            foreach (SONG song in songQuery)
            {
                try
                {
                    if ((File.Exists(song.LOCATION)) && (song.HASH == null))
                    {
                        byte[] hash = MusicDao.GenerateHashCode(song.LOCATION);
                        song.HASH = hash;
                        dataContext.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    testContextInstance.WriteLine(e.Message);
                }
            }
        }

        // Use SQL instead
        [TestMethod()]
        public void DeleteDuplicateSongs()
        {
            var dataContext = new MusicEntities1();

            // Delete songs with the same hash code
            IQueryable<SONG> songQuery = from s in dataContext.SONG 
                                         orderby s.HASH
                                         where s.HASH != null 
                                         select s;

            SONG previousSong = null;
            foreach (SONG song in songQuery)
            {
                if ((previousSong != null) && (previousSong.HASH == song.HASH))
                {
                    testContextInstance.WriteLine("{0} : {1}", previousSong.LOCATION, song.LOCATION);

                    if (previousSong.ID < song.ID)
                    {
                        dataContext.SONG.Remove(previousSong);
                        dataContext.SaveChanges();
                    }
                    else
                    {
                        dataContext.SONG.Remove(song);
                        dataContext.SaveChanges();
                    }
                }
                previousSong = song;
            }

            // Delete songs with the same location
            songQuery = from s in dataContext.SONG
                                         orderby s.LOCATION
                        where s.LOCATION != null
                                         select s;

            previousSong = null;
            foreach (SONG song in songQuery)
            {
                if ((previousSong != null) && (previousSong.LOCATION == song.LOCATION))
                {
                    testContextInstance.WriteLine("{0} : {1}", previousSong.LOCATION, song.LOCATION);
                    if (previousSong.ID < song.ID)
                    {
                        dataContext.SONG.Remove(previousSong);
                        dataContext.SaveChanges();
                    }
                    else
                    {
                        dataContext.SONG.Remove(song);
                        dataContext.SaveChanges();
                    }
                }
                previousSong = song;
            }
            dataContext.SaveChanges();
        }

        public void Log(String info)
        {
            testContextInstance.WriteLine("{0}", info);
        }

        [TestMethod()]
        public void UpdateSongs()
        {
            String container = "songs";
            var uploadProcessor = new UploadProcessor(container, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            var azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            var musicDao = new MusicDao(Settings.Default.ConnectionString, null, uploadProcessor, azureService);
            musicDao.OnNewSong += Log;
            var dataContext = new MusicEntities1();
            IQueryable<SONG> songQuery = from s in dataContext.SONG
                                         orderby s.HASH
                                         where s.HASH != null
                                         select s;

            foreach (SONG song in songQuery)
            {
                try
                {
                    byte[] hash = MusicDao.GenerateHashCode(song.LOCATION);
                    bool hashSame = true;
                    for (int i = 0; i < hash.Length; i++)
                    {
                        if (hash[i] != song.HASH.ToArray()[i])
                        {
                            hashSame = false;
                            break;
                        }
                    }

                    if (!hashSame)
                    {
                        testContextInstance.WriteLine("{0}", song.LOCATION);
                        musicDao.ProcessFile(song.LOCATION);
                        dataContext.SONG.Remove(song);
                    }
                    dataContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    testContextInstance.WriteLine("{0}", ex.Message);
                    testContextInstance.WriteLine("{0}", song.LOCATION);
                }
            }
//            uploadProcessor.EndProcess();
        }

        [TestMethod()]
        public void MigrateRatings()
        {
            String oldConString = "Data Source=76.12.221.204;Initial Catalog=mycruitercom;Persist Security Info=True;User ID=mycruitercom;Password=mycYRs8a";
            String sql = "select song_id, avg(total_rating) from " +
                        "(" +
                        "select sr1.*, (select count(sr2.song_id) c from song_rated sr2 " +
                        "where sr2.song_id=sr1.song_id) as total_rating from song_rated sr1 " +
                        ") as d " +
                        "group by song_id";

            String newConString = "Data Source=[Account];Initial Catalog=Music;User Id=SqlAzure;Password=[Password];";

            var musicDao = new MusicDao();
            using (SqlConnection con = new SqlConnection(oldConString))
            {
                con.Open();
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    SqlDataReader dataReader = com.ExecuteReader();
                    while (dataReader.Read())
                    {
                        int songId = (int)dataReader[0];
                        int rating = (int)dataReader[1];
                        if (rating > 1)
                        {
                            musicDao.UpdateSongRating(newConString, songId, rating);
                        }
                    }
                }
            }

        }

        [TestMethod()]
        public void MigrateArtists()
        {
            String oldConString = "Data Source=76.12.221.204;Initial Catalog=mycruitercom;Persist Security Info=True;User ID=mycruitercom;Password=mycYRs8a";
            String newConString = "Data Source=[Account];Initial Catalog=Music;User Id=SqlAzure;Password=[Password];";

            String sql = "SELECT * "+
                "FROM [mycruitercom].[dbo].[ARTIST] "+
                "where comments is not null "+
                "or rating is not null "+
                "or radio is not null";

            var musicDao = new MusicDao();
            using (SqlConnection con = new SqlConnection(oldConString))
            {
                con.Open();
                using (SqlCommand com = new SqlCommand(sql, con))
                {
                    SqlDataReader dataReader = com.ExecuteReader();
                    while (dataReader.Read())
                    {
                        int artistId = (int)dataReader["ID"];

                        int? rating = (dataReader["RATING"] != DBNull.Value) ? (int?)dataReader["RATING"] : null;
                        bool? radio = (dataReader["RADIO"] != DBNull.Value) ? (bool?)dataReader["RADIO"] : null;

                        String comments = (dataReader["COMMENTS"] != DBNull.Value) ? (String)dataReader["COMMENTS"] : null;

                        musicDao.UpdateArtist(newConString, artistId, comments, rating, radio);
                    }
                }
            }
        }

        [TestMethod()]
        public void SyncSongTags()
        {
            using (var dataContext = new MusicEntities1())
            {
                IQueryable<SONG> songs = from s in dataContext.SONG
                                             orderby s.HASH
                                             where s.HASH != null
                                             select s;
                foreach (var song in songs)
                {

                }
            }
        }
    }
}
