using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Samples.MediaCatalog;
using IdSharp.Tagging.ID3v2;
using IdSharp.Tagging.ID3v1;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Auth;
using Azure;
using com.andrewlinton.music;

namespace Music
{
    public class MusicDao : IMusicDao
    {
        String connString = null; 
        static String destinationFolder = null; // @"I:\Music";
        int songCount = -1;
        String lastSuccess = null;
        Random random = new Random();
        public event Action<String> OnError;
        public event Action<String> OnStatus;
        public event Action<String> OnNewSong;
        public event Action<String> OnNewArtist;
        public event Action<String> OnProcessFile;
        private ConcurrentBag<string> filenameList = new ConcurrentBag<string>();
        private Object filenameQueueLock = new Object();
        private Object insertSongLock = new Object();
        IUploadProcessor uploadProcessor = null;
        AzureService azureService;
        CloudQueueClient cloudQueueClient;
        CloudQueue[] newSongQueue;
        string[] clientNames = { "coolmaster", "alintonaspire", "alien2" };

        private static List<DelegateResultPair> ProcessFileDelgates { get; set; }

 //       public delegate void InsertSongDelegate(String title, String artist, String album, String genre, long? duration, long? bitrate, long? filesize, String location, byte[] hash);
 //       public InsertSongDelegate insertSong;
        public Func<SONG, SONG> insertSong;

//        public delegate void UpdateSongUploadStatusDelegate(int songId, bool uploadStatus);
//        public UpdateSongUploadStatusDelegate updateSongUploadStatus;
        public Func<SONG, SONG> updateSongUploadStatus;

//        public delegate void EnqueueNewSongDelegate(int id, string location);
//       public EnqueueNewSongDelegate enqueueNewSong;
        public Func<SONG, SONG> enqueueNewSong;

        public ConcurrentQueue<String> FilenameQueue
        {
            get;
            set;
        }

        public IEnumerable<string> FilenameList
        {
            get
            {
                return filenameList;
            }
        }

        private class DelegateResultPair
        {
            public Action<String> Delegate { get; set; }
            public IAsyncResult Result { get; set; }
        }

        public int SongsInserted { get; set; }

        public MusicDao()
        { 
        }

        public MusicDao(string connString, string[] musicFolders, IUploadProcessor uploadProcessor, AzureService azureService)
        {
            insertSong = this.InsertSong;
            updateSongUploadStatus = this.UpdateSongUploadStatus;
            enqueueNewSong = this.EnqueueNewSong;
            this.uploadProcessor = uploadProcessor;
            uploadProcessor.OnStatus += this.OnStatus;
            uploadProcessor.OnError += this.OnError;
            this.azureService = azureService;
            cloudQueueClient = azureService.GetCloudQueueClient();

            ProcessFileDelgates = new List<DelegateResultPair>();
            this.connString = connString;
            FilenameQueue = new ConcurrentQueue<string>();

            newSongQueue = new CloudQueue[clientNames.Length];

            int i = 0;
            foreach (var clientName in clientNames)
            {
                newSongQueue[i] = cloudQueueClient.GetQueueReference(clientName);

                //Check whether the queue exists, and create it if it does not.
                if (!newSongQueue[i].Exists())
                {
                    newSongQueue[i].Create();
                }
                i++;
            }
        }

        public void Initialize()
        {
            try
            {
                using (MusicEntities1 dataContext = new MusicEntities1())
                {
/*                    Table<SONG> songs = dataContext.SONGs.ToArray().Count();
                    songCount = songs.Count(); */
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                throw;
            }
        }

        bool DesiredGenre(IEnumerable<String> desiredGenres, String currentGenre)
        {
            bool desired = false;
            foreach (String desiredGenre in desiredGenres)
            {
                if (currentGenre.ToUpper().Contains(desiredGenre))
                {
                    desired = true;
                }
            }
            return desired;
        }

        public static bool SelectMethod(String input, IEnumerable<String> desiredGenres)
        {
            foreach (String desiredGenre in desiredGenres)
            {
                if (desiredGenre.ToUpper().Equals(input.ToUpper()))
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<SONG> GetSelectedSongs(IEnumerable<String> desiredGenres)
        {
            using (var dataContext = new MusicEntities1())
            {

                var query = from song in dataContext.SONG
                            join artist in dataContext.ARTIST
                            on song.ARTIST equals artist.NAME into artists
                            where (song.LOCATION.ToUpper().Contains(".WMA") ||
                            song.LOCATION.ToUpper().Contains(".MP3"))
                            select new { Song = song, Artist = artists };

                List<SONG> ret = new List<SONG>();

                foreach (var q in query)
                {
                    int? rating = 0;
                    rating = q.Song.RATING;
                    foreach (ARTIST artist in q.Artist)
                    {
                        if ((artist.RATING != null) && (artist.RATING != 0))
                        {
                            rating = artist.RATING.Value;
                        }
                    }

                    for (int i = 0; i <= rating; i++)
                    {
                        ret.Add(q.Song);
                    }
                }

                var songQuery = from song in ret
                                where SelectMethod(song.GENRE, desiredGenres)
                                select song;

                return songQuery;
            }
        }

        public SONG UploadSong(SONG song)
        {
            song.UPLOADED = uploadProcessor.UploadFile(new Tuple<string, string>(song.LOCATION, song.ID.ToString()));
            return song;
        }

        public SONG EnqueueSong(SONG song)
        {
            OnStatus(String.Format("Enqueueing: {0}", song.LOCATION));
            enqueueNewSong(song);
            OnStatus(String.Format("Enqueued: {0}", song.LOCATION));
            return song;
        }

        public SONG InsertSong(SONG song)
        {
            int id = -1;

            try
            {
                OnStatus(String.Format("Inserting: {0}", song.LOCATION));

                // Check song attributes
                if (String.IsNullOrEmpty(song.TITLE))
                {
                    OnError(String.Format("Invalid title {0}", song.LOCATION));
                }
                if (String.IsNullOrEmpty(song.ARTIST))
                {
                    OnError(String.Format("Invalid artist {0}", song.LOCATION));
                }
                if (song.ALBUM == null)
                {
                    OnError(String.Format("Invalid album {0}", song.LOCATION));
                }
                if (song.GENRE == null)
                {
                    OnError(String.Format("Invalid genre {0}", song.LOCATION));
                }

//                Debug.WriteLine("Inserting: " + location);

                using (SqlConnection con = new SqlConnection(connString))
                {
                    con.Open();
                    String sql = "InsertSong";
                    using (SqlCommand command = new SqlCommand(sql, con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@title",SqlDbType.VarChar).Value = song.TITLE;
                        command.Parameters.Add("@artist",SqlDbType.VarChar).Value = song.ARTIST;
                        command.Parameters.Add("@album",SqlDbType.VarChar).Value = song.ALBUM;
                        command.Parameters.Add("@genre",SqlDbType.VarChar).Value = song.GENRE;
                        command.Parameters.Add("@duration", SqlDbType.BigInt).Value = 0; // song.DURATION ? song.DURATION : 0;
                        command.Parameters.Add("@filesize", SqlDbType.BigInt).Value = song.FILESIZE; 
                        command.Parameters.Add("@rating",SqlDbType.Int).Value = 1;
                        command.Parameters.Add("@comments", SqlDbType.VarChar).Value = String.Empty;
                        command.Parameters.Add("@location", SqlDbType.VarChar).Value = song.LOCATION;
                        byte[] hash = song.HASH.ToArray();
                        command.Parameters.Add("@hash",SqlDbType.Binary).Value = hash;

                        SqlParameter idParameter = new SqlParameter("@id", SqlDbType.Int);
                        idParameter.Value = -1;
                        idParameter.Direction = ParameterDirection.Output;
                        command.Parameters.Add(idParameter);

                        command.ExecuteNonQuery();

                        id = (int)idParameter.Value;
                        song.ID = id;
                    }
                }

                String songKey = id.ToString();
                SongsInserted++;

                OnNewSong(song.LOCATION);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                OnError(ex.Message);
//                throw;
            }
            return song;
        }

        private SONG EnqueueNewSong(SONG song)
        {
            OnStatus(string.Format("Enqueueing: {0}", song.LOCATION));
            song.UPLOADED = true;
            UpdateSongUploadStatus(song);

            foreach (var queue in newSongQueue)
            {
                string messageString = string.Format("{0}---{1}", song.ID, song.LOCATION);
                CloudQueueMessage message = new CloudQueueMessage(messageString);
                queue.AddMessage(message);
            }
            OnStatus(string.Format("Enqueued: {0}", song.LOCATION));
            return song;
        }

        public bool UpdateSong(String title, String artist, String album, String genre, long? duration, long? bitrate, long? filesize, String location)
        {
            bool updated = false;

            int start = location.ToLower().IndexOf("\\music\\");
            location = location.Substring(start);

            using (var dataContext = new MusicEntities1())
            {
                // Check if song already in db
                var results = from s in dataContext.SONG
                                         where (s.LOCATION.Contains(location))
                                         select s;

                var songs = results.ToArray<SONG>();

                foreach (SONG song in songs)
                {
                    updated = true;
                    song.TITLE = title;
                    song.ARTIST = artist;
                    song.ALBUM = album;
                    song.GENRE = genre;
                    song.DURATION = duration;
                    song.BITRATE = bitrate;
                    song.FILESIZE = filesize;
                    song.UPDATED = DateTime.Now;
                    dataContext.SaveChanges();
                    break;
                }
            }
            return updated;
        }

        public void InsertArtist(String name)
        {
            try
            {
                using (var dataContext = new MusicEntities1())
                {
                    var artists = dataContext.ARTIST;

                    ARTIST artist = new ARTIST();
                    artist.NAME = name;
                    dataContext.ARTIST.Add(artist);
                    dataContext.SaveChanges();
                    OnNewArtist(String.Format("Inserting new artist: {0}", name.ToString()));
                }
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                OnError(ex.StackTrace);
                throw;
            }
        }

        public SONG PickRandomSong(Func<SONG,bool> selectSong)
        {
            SONG song = null;

            try
            {
                using (var dataContext = new MusicEntities1())
                {
                    var songs = dataContext.SONG;

                    // Repeat until song is found
                    while (song == null)
                    {
                        int songId = random.Next(songCount);
                        song = songs.Single(s => s.ID == songId);
                        if (selectSong != null)
                        {
                            if (!selectSong(song))
                            {
                                song = null;
                                continue;
                            }
                        }
                    }
                }
            }
            catch (InvalidOperationException ioe)
            {
                Debug.WriteLine(ioe.Message);
                Debug.WriteLine(ioe.StackTrace);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                throw;
            }
            return song;
        }

        public bool CopySong(String source)
        {
            try
            {
                String destination = destinationFolder + @"\" + Path.GetFileName(source);
                if (!File.Exists(destination))
                {
                    File.Copy(source, destination);
                }
                lastSuccess = destination;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                if (ex.Message.Contains("There is not enough space on the disk."))
                {
                    Debug.WriteLine("Device full");
                    // Delete last song
                    if (lastSuccess != null)
                    {
                        File.Delete(lastSuccess);
                    }
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }

        public void NoArtist()
        {
            using (var dataContext = new MusicEntities1())
            {
                // Get artists from songs
                IQueryable<String> songArtists = (from a in dataContext.SONG
                                                  select a.ARTIST).Distinct();

                IQueryable<String> artistNames = from artist in dataContext.ARTIST
                                                        select artist.NAME;

                IQueryable<String> noArtistSongs = songArtists.Except(artistNames);

                foreach (var songArtist in noArtistSongs)
                {
                    OnStatus(songArtist);
                }
            }
        }

        // Goes through each song in the db and adds the artists to the 
        // artist table if it does not exist there
        public void UpdateArtists()
        {
            try
            {
                using (var dataContext = new MusicEntities1())
                {
                    // Get artists from songs
                    IQueryable<String> songArtists = (from a in dataContext.SONG
                                                      select a.ARTIST).Distinct();

                    IQueryable<String> artistNames = from artist in dataContext.ARTIST
                                                     select artist.NAME;

                    IQueryable<String> noArtistSongs = songArtists.Except(artistNames);

                    // Check if each song artist is in artist table
                    foreach (var songArtist in noArtistSongs)
                    {
                        try
                        {
                            if (null == songArtist) continue;
                            String nSongArtist = songArtist;
                            nSongArtist = nSongArtist.Trim();
                            nSongArtist = nSongArtist.ToUpper();
                            if (nSongArtist.StartsWith("THE "))
                            {
                                nSongArtist = nSongArtist.Substring(3);
                                nSongArtist = nSongArtist.Trim();
                            }

                            // Skip bad data
                            if (String.IsNullOrEmpty(nSongArtist))
                            {
                                OnError(nSongArtist);
                                continue;
                            }
                            if (nSongArtist.Contains("\0"))
                            {
                                OnError(nSongArtist);
                                continue;
                            }
                            if (nSongArtist.StartsWith("("))
                            {
                                OnError(nSongArtist);
                                continue;
                            }
                            if (nSongArtist.StartsWith("?"))
                            {
                                OnError(nSongArtist);
                                continue;
                            }
                            if (nSongArtist.StartsWith("/"))
                            {
                                OnError(nSongArtist);
                                continue;
                            }

                            OnStatus(nSongArtist);

                            IQueryable<ARTIST> artists = from a in dataContext.ARTIST where a.NAME.ToUpper() == nSongArtist.ToUpper() select a;
                            if (artists.Count() == 0) // Artist not in artist table
                            {
                                InsertArtist(nSongArtist);
                                //OnStatus(nSongArtist);
//                                OnNewArtist(String.Format("Inserting: {0}",nSongArtist));
                            }
                        }
                        catch (Exception e1)
                        {
                            OnError(String.Format("{0} : {1} : {2}", songArtist, e1.Message, e1.StackTrace));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                throw;
            }
        }

        public void CrawlFoldersWMA(IEnumerable<String> musicFolders)
        {
            try
            {
                foreach (String musicFolder in musicFolders)
                {
                    SearchFolderWMA(musicFolder);
                    string[] directories = Directory.GetDirectories(musicFolder);
                    CrawlFoldersWMA(directories);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                OnError(ex.Message);
            }
        }

        private void GetSongFilesImp(List<SONG> songs, string folder)
        {
            var files = Directory.GetFiles(folder);

            foreach (String file in files)
            {
                var song = new SONG();
                song.LOCATION = file;
                songs.Add(song);
            }

            String[] subFolders = Directory.GetDirectories(folder);
            foreach (String subFolder in subFolders)
            {
                GetSongFilesImp(songs, subFolder);
            }
        }

        public IEnumerable<SONG> GetSongFiles(string folder)
        {
            var songsFiles = new List<SONG>();
            GetSongFilesImp(songsFiles, folder);
            return songsFiles;
        }

        void GatherSongs(List<SONG> songs, string folder)
        {
            try
            {
                var files = Directory.GetFiles(folder);

                foreach (String file in files)
                {
                    var song = new SONG();
                    song.LOCATION = file;
                    songs.Add(song);
                }

                String[] subFolders = Directory.GetDirectories(folder);
                foreach (String subFolder in subFolders)
                {
                    GatherSongs(songs, subFolder);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                OnError(ex.Message);
            }
        }

        SONG processSong(SONG song, Action<String> fileAction)
        {
            return song;
        }

        public void CrawlFolderS(String folder, Func<SONG, SONG> songFunc)
        {
            var songs = new List<SONG>();
            GatherSongs(songs, folder);

            var processed = songs.Select(song => songFunc(song));
            foreach (var p in processed)
            {
                var x = p.LOCATION;
            }
        }

        public void CrawlFolder(String folder, Action<String> fileAction)
        {
            try
            {
                var files = Directory.GetFiles(folder);

                

                foreach (String file in files)
                {
                    fileAction(file);
                }

                String[] subFolders = Directory.GetDirectories(folder);
                foreach (String subFolder in subFolders)
                {
                    CrawlFolder(subFolder, fileAction);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                OnError(ex.Message);
            }
        } 

        public void GetSongMetadata(String filename, out String title, out String artist, out String album, out String genre)
        {
            if (filename.ToUpper().EndsWith(".WMA"))
            {
                using (MetadataEditor md = new MetadataEditor(filename))
                {
                    title = md.GetAttributeByName("Title").ToString();
                    artist = md.GetAttributeByName("Author").ToString();
                    genre = "Unknown";
                    if (md.GetAttributeByName("WM/Genre") != null)
                    {
                        genre = md.GetAttributeByName("WM/Genre").ToString();
                    }
                    album = String.Empty;
                    if (md.GetAttributeByName("WM/AlbumTitle") != null)
                    {
                        album = md.GetAttributeByName("WM/AlbumTitle").ToString();
                    }
                    long duration = Convert.ToInt64(md.GetAttributeByName("Duration"));
                    long bitrate = Convert.ToInt64(md.GetAttributeByName("Bitrate"));
                    long filesize = Convert.ToInt64(md.GetAttributeByName("FileSize"));
                }
            }
            else
            {
                GetMp3Tags(filename, out title, out artist, out album, out genre);
            }
        }

        private void SearchFolderWMA(String folderName)
        {
            if (Directory.Exists(folderName))
            {
                String[] files = Directory.GetFileSystemEntries(folderName, "*.wma");
                foreach (String file in files)
                {
                    if (file != null)
                    {
                        try
                        {
                            String title, artist, album, genre;
                            long duration, bitrate, filesize;
                            GetWmaTags(file, out title, out artist, out album, out genre, out duration, out bitrate, out filesize);
                            SONG song = new SONG { LOCATION = file, TITLE = title, ARTIST = artist, GENRE = genre, DURATION = duration, BITRATE = bitrate, FILESIZE = filesize };
                            insertSong(song);
                        }
                        catch (Exception e1)
                        {
                            OnError(String.Format("Error reading {0}", file));
                            Debug.WriteLine("Error reading {0}", file);
                            Debug.WriteLine(e1.Message);
                        }
                    }
                }
            }
        }

        public void CrawlFoldersMp3(IEnumerable<String> musicFolders)
        {
            try
            {
                foreach (String musicFolder in musicFolders)
                {
                    SearchFolderMP3(musicFolder);
                    string[] directories = Directory.GetDirectories(musicFolder);
                    CrawlFoldersMp3(directories);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                OnError(ex.Message);
            }
        }

        private void SearchFolderMP3(String directory)
        {
            if (Directory.Exists(directory))
            {
                String[] files = Directory.GetFileSystemEntries(directory, "*.mp3");
                foreach (String file in files)
                {
                    try
                    {
                        String title;
                        String artist;
                        String album;
                        String genre;
                        GetMp3Tags(file, out title, out artist, out album,out genre);
                        var song = new SONG() { LOCATION = file, TITLE = title, ARTIST = artist, ALBUM = album, GENRE = genre };
                        insertSong(song);
                    }
                    catch (Exception e1)
                    {
                        Debug.WriteLine(e1.Message);
                        OnError(e1.Message);
                    }
                }
            }
        }

        public void GetTags(String filename, out String title, out String artist, out String album, out String genre)
        {
            if (filename.ToUpper().EndsWith(".MP3"))
            {
                GetMp3Tags(filename, out title, out artist, out album, out genre);
            }
            else if (filename.ToUpper().EndsWith(".WMA"))
            {
                long duration, bitrate, filesize;
                GetWmaTags(filename, out title, out artist, out album, out genre, out duration, out bitrate, out filesize);
            }
            else
            {
                throw new ArgumentException("Unknown file type");
            }
        }

        public void GetWmaTags(String filename, out String title, out String artist, out String album, out String genre, out long duration, out long bitrate, out long filesize)
        {
            using (MetadataEditor md = new MetadataEditor(filename))
            {
                title = md.GetAttributeByName("Title").ToString();
                artist = md.GetAttributeByName("Author").ToString();
                genre = "Unknown";
                if (md.GetAttributeByName("WM/Genre") != null)
                {
                    genre = md.GetAttributeByName("WM/Genre").ToString();
                }
                album = String.Empty;
                if (md.GetAttributeByName("WM/AlbumTitle") != null)
                {
                    album = md.GetAttributeByName("WM/AlbumTitle").ToString();
                }
                if (String.IsNullOrEmpty(title))
                {
                    String[] s = filename.Split(new char[] { '\\', '.' });
                    title = s[s.Length - 2] + " " + album;
                }
                duration = Convert.ToInt64(md.GetAttributeByName("Duration"));
                bitrate = Convert.ToInt64(md.GetAttributeByName("Bitrate"));
                filesize = Convert.ToInt64(md.GetAttributeByName("FileSize"));
            }
        }

        public void GetMp3Tags(String filename, out String title, out String artist, out String album, out String genre)
        {
            title = null;
            artist = null;
            album = null;
            genre = null;

            try
            {
                IID3v2 iD3v2 = ID3v2Helper.CreateID3v2(filename);
                artist = iD3v2.Artist;
                title = iD3v2.Title;
                album = iD3v2.Album;
                genre = iD3v2.Genre;
                if ((string.IsNullOrWhiteSpace(genre)) || (genre.Contains("(")))
                {
                    genre = genre.Replace('(', ' ');
                    genre = genre.Replace(')', ' ');
                    genre = genre.Trim();
                    int genreIndex = Convert.ToInt16(genre);
                    genre = GenreHelper.GenreByIndex[genreIndex];
                }
            }
            catch (Exception)
            {
                Debug.WriteLine(String.Format("Warning: Unable to extract ID3v2 tags from {0}", filename));
            }

            IID3v1 iD3v1 = ID3v1Helper.CreateID3v1(filename);
            if ((string.IsNullOrWhiteSpace(genre)) || (genre.Contains("(")))
            {
                genre = GenreHelper.GenreByIndex[iD3v1.GenreIndex];
            }
            if (string.IsNullOrWhiteSpace(artist))
            {
                artist = iD3v1.Artist;
            }
            if (string.IsNullOrWhiteSpace(title))
            {
                title = iD3v1.Title;
            }
            if (string.IsNullOrWhiteSpace(album))
            {
                album = iD3v1.Album;
            }
            if (String.IsNullOrWhiteSpace(title))
            {
                String[] s = filename.Split(new char[] {'\\','.'});
                title = s[s.Length - 2];
            }
        }

        public void QueryArtists()
        {
            try
            {
                using (var dataContext = new MusicEntities1())
                {
                    var artists = dataContext.ARTIST;

                    IQueryable<ARTIST> query = from a in artists
                                               where a.NAME.Contains("Kiss the Boys")
                                               orderby a.NAME
                                               select a;

                    foreach (ARTIST artist in query)
                    {
                        Trace.WriteLine(String.Format("{0}, {1}", artist.NAME, artist.COMMENTS));
                        artist.NAME = artist.NAME.Remove(0, "(KISS THE BOYS...) ".Length);
                    }
                    dataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public void QuerySongs()
        {
            try
            {
                using (var dataContext = new MusicEntities1())
                {
                    var songs = dataContext.SONG.ToArray();
                    int count = songs.Count();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public void RandomFill()
        {
            String lastSong = null;

            try
            {
                while (true)
                {
                    SONG song = PickRandomSong(PickDesiredSong);

                    if (song == null)
                    {
                        continue;
                    }

                    if (!File.Exists(song.LOCATION))
                    {
                        continue;
                    }

                    if (!CopySong(song.LOCATION))
                    {
                        break;
                    }
                    OnStatus(song.TITLE);
                    lastSong = song.LOCATION;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
            if (lastSong != null)
            {
                File.Delete(lastSong);
            }
        }

        public void InsertSongContent()
        {
            try
            {
                SONG song = PickRandomSong(null);
                using (SqlConnection con = new SqlConnection(connString))
                {
                    con.Open();
                    List<byte> l = new List<byte>();

                    FileStream fs = new FileStream(song.LOCATION, FileMode.Open, FileAccess.Read);
                    byte[] image = new Byte[fs.Length];
                    fs.Read(image, 0, image.Length);

                    String filename = song.LOCATION.Substring(song.LOCATION.LastIndexOf('\\'));
                    String sqlWrite = "INSERT INTO SONG_CONTENT (SONG_ID,CONTENT,FILENAME) VALUES (@Id,@BlobData,@Filename)";
                    SqlCommand cmdWrite = new SqlCommand(sqlWrite, con);

                    // Create parameter for insert command.
                    SqlParameter prm;
                    prm = new SqlParameter("@Id", song.ID);
                    cmdWrite.Parameters.Add(prm);

                    if (image != null)
                    {
                        // Add a parameter for the image binary data.
                        prm = new SqlParameter("@BlobData", SqlDbType.VarBinary,
                            image.Length, ParameterDirection.Input, false,
                            0, 0, null, DataRowVersion.Current, image);
                    }
                    else
                    {
                        // Add a parameter for a null image.
                        prm = new SqlParameter("@BlobData", SqlDbType.VarBinary, 0,
                            ParameterDirection.Input, false,
                            0, 0, null, DataRowVersion.Current,
                            System.DBNull.Value);
                    }

                    // Add the parameter to the command.
                    cmdWrite.Parameters.Add(prm);

                    prm = new SqlParameter("@Filename", filename);
                    cmdWrite.Parameters.Add(prm);

                    // Execute the command to update the image in the database.
                    cmdWrite.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public void GetSongContent()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    con.Open();

                    String sql = "SELECT TOP (1) [CONTENT],FILENAME FROM SONG_CONTENT";
                    
                    int bufferSize = 100;
                    byte[] outbyte = new byte[bufferSize];
                    long retVal = 0;
                    long startIndex = 0;

                    SqlCommand cmd = new SqlCommand(sql, con);

                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
                    dr.Read();

                    // Check to see if the field is DBNull.
                    if (!dr.IsDBNull(0))
                    {
                        String filename = dr[1].ToString();
                        // Create the memory stream to hold the output.
                        FileStream fs = new FileStream(filename, FileMode.CreateNew, FileAccess.Write, FileShare.None);

                        // Read the bytes into outbyte.
                        retVal = dr.GetBytes(0, startIndex, outbyte, 0, bufferSize);

                        // Keep reading while there are more bytes
                        // beyond the buffer.
                        while (retVal == bufferSize)
                        {
                            // Write the bytes to the memory stream.
                            fs.Write(outbyte, 0, outbyte.Length);

                            // Update the start index and
                            // fill the buffer again.
                            startIndex += bufferSize;
                            retVal = dr.GetBytes(0, startIndex, outbyte, 0,
                                bufferSize);
                        }

                        // Write the bytes remaining in the buffer.
                        fs.Write(outbyte, 0, (int)retVal - 1);
                        // Transfer the memory stream to the image.
                        fs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public void UploadSongs(int numSongs)
        {
            List<int> uploadedSongs = new List<int>();
 //           List<SONG> songs = GetProcSongsLinq();
            List<SONG> songs = null;
            Random random = new Random();
            for (int i = 0; i < numSongs; i++)
            {
                try
                {
                    int songIndex = random.Next(songs.Count);
                    if (uploadedSongs.Contains(songIndex))
                    {
                        continue;
                    }
                    SONG song = songs[songIndex];
                    FtpManager.Upload(song.LOCATION, song.ID);
                    song.UPLOADED = true;
                    UpdateSongUploadStatus(song);
                    uploadedSongs.Add(songIndex);
                    OnStatus(string.Format("Uploading: {0}", song.LOCATION));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(String.Format("Unable to upload {0}", ex.Message));
                }
            }
        }

        public void UpdateSongRating(String connectionString, int songId, int rating)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // Update song rating
                    con.Open();
                    String sql = "UPDATE SONG SET RATING = @Rating,UPDATED = GetDate() WHERE ID=@Id";
                    SqlCommand command = new SqlCommand(sql, con);
                    command.Parameters.AddWithValue("@Rating", rating);
                    command.Parameters.AddWithValue("@Id", songId);
                    command.ExecuteNonQuery();

                    // Select song artist
                    sql = "SELECT ARTIST FROM SONG WHERE ID=@ID";
                    command = new SqlCommand(sql, con);
                    command.Parameters.AddWithValue("@Id", songId);
                    String artistName = null;
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            artistName = (String)dataReader["ARTIST"];
                        }
                        else
                        {
                            return;
                        }
                    }

                    // Select artist rating
                    int artistRating = 0;
                    sql = "SELECT RATING FROM ARTIST WHERE NAME=@NAME";
                    command = new SqlCommand(sql, con);
                    command.Parameters.AddWithValue("@NAME", artistName);
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            if (dataReader["RATING"] != DBNull.Value)
                            {
                                artistRating = (int)dataReader["RATING"];
                            }
                        }
                    }

                    // Update artist rating
                    if (artistRating == 0)
                    {
                        sql = "UPDATE ARTIST SET RATING = @Rating WHERE NAME=@NAME";
                        command = new SqlCommand(sql, con);
                        command.Parameters.AddWithValue("@Rating", rating);
                        command.Parameters.AddWithValue("@NAME", artistName);
                        command.ExecuteNonQuery();
                    }

                    // Update Song Rating table
                    sql = "DELETE FROM SONG_RATED WHERE SONG_ID=@SongId";
                    command = new SqlCommand(sql, con);
                    command.Parameters.AddWithValue("@SongId", songId);
                    command.ExecuteNonQuery();

                    int inserts = 0;
                    inserts = inserts + rating;
                    sql = "INSERT INTO Song_Rated (Song_Id) VALUES (@SongId)";
                    command = new SqlCommand(sql, con);
                    command.Parameters.AddWithValue("@SongId", songId);
                    for (int i = 0; i < inserts; i++)
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                OnError(ex.StackTrace);
                throw;
            }
        }

        public void UpdateArtist(String connectionString, int artistId, String comments, int? rating, bool? radio)
        {
            String sql = null;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // Update song rating
                    con.Open();

                    if (!String.IsNullOrEmpty(comments))
                    {
                        sql = "UPDATE artist SET comments = @comments WHERE ID=@Id";
                        using (SqlCommand command = new SqlCommand(sql, con))
                        {
                            command.Parameters.AddWithValue("@comments", comments);
                            command.Parameters.AddWithValue("@Id", artistId);
                            command.ExecuteNonQuery();
                        }
                    }

                    if (rating.HasValue)
                    {
                        sql = "UPDATE artist SET rating = @rating WHERE ID=@Id";
                        using (SqlCommand command = new SqlCommand(sql, con))
                        {
                            command.Parameters.AddWithValue("@rating", rating);
                            command.Parameters.AddWithValue("@Id", artistId);
                            command.ExecuteNonQuery();
                        }
                    }

                    if (radio.HasValue)
                    {
                        sql = "UPDATE artist SET radio = @radio WHERE ID=@Id";
                        using (SqlCommand command = new SqlCommand(sql, con))
                        {
                            command.Parameters.AddWithValue("@radio", radio);
                            command.Parameters.AddWithValue("@Id", artistId);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                OnError(ex.StackTrace);
                throw;
            }
        }

        public SONG UpdateSongUploadStatus(SONG pSong)
        {
            using (var dataContext = new MusicEntities1())
            {
                SONG song = dataContext.SONG.Single(s => s.ID == pSong.ID);
                song.UPLOADED = pSong.UPLOADED;
                song.UPDATED = DateTime.Now;
                dataContext.SaveChanges();
            }
            return pSong;
        }

        public SONG GetSongById(int songId)
        {
            SONG song = null;
            using (var dataContext = new MusicEntities1())
            {
                song = dataContext.SONG.Single(s => s.ID == songId);
            }
            return song;
        }

        public SONG GetSongByLocation(string location)
        {
            SONG song = null;

            string location1 = location.ToLower();
            int start = location1.IndexOf("\\music\\");
            if (start > -1)
            {
                location = location.Substring(start);
            }

            using (var dataContext = new MusicEntities1())
            {
                song = dataContext.SONG.FirstOrDefault(s => s.LOCATION.IndexOf(location) > -1);
            }
            return song;
        }

        public bool UpdateSongByLocation(string location, Action<SONG> updateAction)
        {
            SONG song = null;
            bool updated = false;

            string location1 = location.ToLower();
            int start = location1.IndexOf("\\music\\");
            if (start > -1)
            {
                location = location.Substring(start);
            }

            using (var dataContext = new MusicEntities1())
            {
                song = dataContext.SONG.FirstOrDefault(s => s.LOCATION.IndexOf(location) > -1);
                if (song!=null)
                {
                    updateAction(song);
                    dataContext.SaveChanges();
                    updated = true;
                }
            }
            return updated;
        }

/*        public int DesiredArtist()
        {
            int ret = -1;
            using (var dataContext = new MusicEntities())
            {
                ArtistLoadRequest artistLoadRequest = dataContext.ArtistLoadRequests.Single(a => a.Loaded == false);
                if (artistLoadRequest != null)
                {
                    return artistLoadRequest.ArtistId;
                }
            }
            return ret;
        } */

/*        public void SetDesiredArtistLoaded()
        {
            using (var dataContext = new MusicEntities())
            {
                ArtistLoadRequest artistLoadRequest = dataContext.ArtistLoadRequests.Single(a => a.Loaded == false);
                if (artistLoadRequest != null)
                {
                    artistLoadRequest.Loaded = true;
                }
                dataContext.SubmitChanges();
            }
        } */

        public List<SONG> GetSongsByArtist(int artistId)
        {
            using (var dataContext = new MusicEntities1())
            {

                var artists = from row in dataContext.ARTIST
                              where (row.ID == artistId)
                              select row.NAME;

                List<SONG> ret = new List<SONG>();

                foreach (String artistName in artists)
                {
                    var songs = from row in dataContext.SONG
                                where (row.ARTIST.Contains(artistName))
                                && row.LOCATION.ToUpper().Contains(".WMA")
                                select row;

                    foreach (SONG song in songs)
                    {
                        ret.Add(song);
                    }
                }

                return ret;
            }
        }

        bool PickDesiredSong(SONG song)
        {
            bool ret = true;
            long maxFilesize = 100494241;

            if (song.FILESIZE > maxFilesize)
            {
                ret = false;
            }
            return ret;
        }

        public void GetProcSongs()
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                String sql = "SelectMusic";
                SqlCommand command = new SqlCommand(sql, connection);
                command.CommandType = CommandType.StoredProcedure;
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    Console.WriteLine(dataReader["Title"]);
                }
            }
        }

/*        public List<SONG> GetProcSongsLinq()
        {
            List<SONG> ret = new List<SONG>();
            using (var dataContext = new MusicEntities())
            {
                var songs = dataContext.s
                    .SelectMusic();
                foreach (var s in songs)
                {
                    SONG song = new SONG();
                    song.TITLE = s.TITLE;
                    song.ARTIST = s.ARTIST;
                    song.ID = s.SONG_ID;
                    song.GENRE = s.GENRE;
                    song.LOCATION = s.LOCATION;
                    int? rating = s.SONG_RATING;
                    if ((rating == null) || (rating == 0))
                    {
                        if ((s.ARTIST_RATING.HasValue) && (s.ARTIST_RATING != null) && (s.ARTIST_RATING != 0))
                        {
                            rating = s.ARTIST_RATING.Value;
                        }
                    }

                    if (rating == null)
                    {
                        rating = 0;
                    }
                    rating++;

                    for (int i = 0; i <= rating; i++)
                    {
                        ret.Add(song);
                    }
                }
            }
            return ret;
        } */

        public static byte[] GenerateHashCode(String filename)
        {
            byte[] result = null;
            HashAlgorithm sha = new SHA1CryptoServiceProvider();
            using (FileStream fileStream = new FileStream(filename, FileMode.Open))
            {
                result = sha.ComputeHash(fileStream);
            }
            return result;
        } 

        public void ProcessFile(String fileName)
        {
            Action<String> processFileDelegate = new Action<String>(ProcessFileWorker);
            IAsyncResult result = processFileDelegate.BeginInvoke(fileName, null, null);

            DelegateResultPair delegateResultPair = new DelegateResultPair();
            delegateResultPair.Delegate = processFileDelegate;
            delegateResultPair.Result = result;
            ProcessFileDelgates.Add(delegateResultPair);
        }

        public void ProcessQueueFilenames(int threadsNum)
        {
            for (int i = 0; i < threadsNum; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessQueuedFilenamesWorker));
            }
        }

        public void GetMetadataFromTitle(String filename)
        {
            try
            {
                String title, artist, album, genre;

                GetMp3Tags(filename, out title, out artist, out album, out genre);

                // Time: 10:00PM Artist: Dj Zinc feat. Ms Dynamite Song: 'Wyle Out' Album: 12" SIngle
                if (title.IndexOf("Artist:") == -1)
                {
                    return;
                }
                int artistStart = title.IndexOf("Artist:") + 8;
                int songStart = title.IndexOf("Song:") + 6;
                int albumStart = title.IndexOf("Album:") + 7;
                artist = title.Substring(artistStart, songStart - artistStart - 7);
                String song = title.Substring(songStart, albumStart - songStart - 8);
                song = song.Replace('\'', ' ');
                song = song.Trim();
                if (albumStart < title.Length)
                {
                    album = title.Substring(albumStart);
                }

                byte[] hash = null;
                InDb(filename);
                var song1 = new SONG() { TITLE = song, ARTIST = artist, GENRE = genre, LOCATION = filename, HASH = hash };
                insertSong(song1);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        public void ProcessQueuedFilenamesWorker(Object stateInfo)
        {
            String filename;
            while (FilenameQueue.TryDequeue(out filename))
            {
                ProcessFileWorker(filename);
            }
        }

        public void ProcessFilename(Object stateInfo)
        {
            String filename = (String)stateInfo;
            ProcessFileWorker(filename);
        }

        public void ProcessFileWorkerConcurrent(String filename)
        {
            Task task = Task.Factory.StartNew(() =>
            {
                ProcessFileWorker(filename);
            });
        }

        public void ProcessFileWorker(String filename)
        {
            String title, artist, album, genre;
            byte[] hash = null;
            int artistStart, artistStop;

            try
            {
                OnStatus(filename);

                if (!Regex.IsMatch(filename, ".mp3|.wma", RegexOptions.IgnoreCase))
                {
                    return;
                }

                int commaLocation = filename.IndexOf(",");
                if (commaLocation > -1)
                {
                    // Separate filename from folder name
                    FileInfo fileInfo = new FileInfo(filename);
                    string directoryName = fileInfo.DirectoryName;
                    string newFilename = fileInfo.Name;

                    // Remove commas from file name
                    do
                    {
                        commaLocation = newFilename.IndexOf(",");
                        if (commaLocation > -1)
                        {
                            newFilename = newFilename.Remove(commaLocation, 1);
                        }
                    }
                    while (commaLocation > -1);

                    // Join new filename with folder name
                    newFilename = directoryName + "\\" + newFilename;

                    // Move old file to new file
                    if (!File.Exists(newFilename))
                    {
                        File.Move(filename, newFilename);
                        File.Delete(filename);
                    }
                    filename = newFilename;
                }

                // Clean up filename
                string rName = "Rhapsody  Subscription Music Service Listen All You Want Millions of Songs";
                if (Regex.IsMatch(filename, rName))
                {
                    // Separate filename from folder name
                    FileInfo fileInfo = new FileInfo(filename);
                    string directoryName = fileInfo.DirectoryName;
                    string newFilename = fileInfo.Name;

                    newFilename = newFilename.Substring(rName.Length);
                    newFilename = "Track" + newFilename;
                    // Join new filename with folder name
                    newFilename = directoryName + "\\" + newFilename;

                    // Move old file to new file
                    if (!File.Exists(newFilename))
                    {
                        File.Move(filename, newFilename);
                        File.Delete(filename);
                    }
                    filename = newFilename;
                }

                if (Regex.IsMatch(filename, ".mp3", RegexOptions.IgnoreCase))
                {
                    if (!InDb(filename))
                    {
                        try
                        {
                            GetMp3Tags(filename, out title, out artist, out album, out genre);

                            // Extract tags from KEXP filenames
                            int endLoc = filename.IndexOf(".mp3");
                            int albumLoc = filename.IndexOf("Album;");
                            if (albumLoc > -1)
                            {
                                album = filename.Substring(albumLoc + 6, endLoc - albumLoc - 6);
                                album = album.Trim();
                            }

                            int titleLoc = filename.IndexOf("Song;");
                            if ((titleLoc > -1) && (albumLoc>-1))
                            {
                                title = filename.Substring(titleLoc + 5, albumLoc - titleLoc - 5);
                                title = title.Trim();
                                if ((title.Length > 1) && (title[0] == '\''))
                                {
                                    title = title.Substring(1);
                                }
                                if ((title.Length > 1) && (title[title.Length-1] == '\''))
                                {
                                    title = title.Substring(0, title.Length - 1);
                                }
                            }

                            int artistLoc = filename.IndexOf("Artist;");
                            if ((artistLoc > -1) && (titleLoc > -1))
                            {
                                artist = filename.Substring(artistLoc + 7, titleLoc - artistLoc - 7);
                                artist = artist.Trim();
                            }

                            if (title.IndexOf("Artist:") != -1)
                            {
                                artistStart = title.IndexOf("Artist:") + 8;
                                int songStart = title.IndexOf("Song:") + 6;
                                int albumStart = title.IndexOf("Album:") + 7;
                                if ((artistStart > 0) && ((songStart - artistStart - 7) > 0))
                                {
                                    artist = title.Substring(artistStart, songStart - artistStart - 7);
                                    String song;
                                    if ((albumStart - songStart - 8) > 0)
                                    {
                                        song = title.Substring(songStart, albumStart - songStart - 8);
                                    }
                                    else
                                    {
                                        song = title.Substring(songStart);
                                    }
                                    song = song.Replace('\'', ' ');
                                    song = song.Trim();
                                    if ((title.IndexOf("Album:") > -1) && (albumStart < title.Length))
                                    {
                                        album = title.Substring(albumStart);
                                    }
                                    title = song;
                                }
                            }

                            String rString = "Rhapsody Player - ";
                            int rPos = filename.IndexOf(rString);
                            if (rPos > -1)
                            {
                                int titleStart = rPos + rString.Length;
                                int titleEnd = filename.IndexOf(" by ", titleStart);
                                if ((titleEnd != -1) && (title.IndexOf(rString)>-1))
                                {
                                    title = filename.Substring(titleStart, titleEnd - titleStart);
                                    artistStart = titleEnd + 4;
                                    artistStop = filename.IndexOf(".mp3", artistStart);
                                    if ((artistStop > -1) && (String.IsNullOrEmpty(artist)))
                                    {
                                        artist = filename.Substring(artistStart, artistStop - artistStart);
                                    }
                                }
                                string newFilename = filename.Remove(rPos, rString.Length);
                                if (!File.Exists(newFilename))
                                {
                                    File.Move(filename, newFilename);
                                    File.Delete(filename);
                                    filename = newFilename;
                                }
                            }

                            if (null == title)
                            {
                                title = "?";
                            }
                            else
                            {
                                title = title.Replace('_', ' ');
                                title = title.Trim();
                            }

                            if (string.IsNullOrWhiteSpace(artist))
                            {
                                throw new Exception("Invalid arist");
                            }
                            if (string.IsNullOrWhiteSpace(genre))
                            {
                                throw new Exception("Invalid genre");
                            }
                            if (genre == "Other")
                            {
                                throw new Exception("Invalid genre");
                            }

                            //if (null == artist)
                            //{
                            //    artist = "?";
                            //}
                            //else
                            //{
                            //    artist = artist.Replace('_', ' ');
                            //    artist = artist.Trim();
                            //}
                            if (null == album)
                            {
                                album = "?";
                            }
                            else
                            {
                                album = album.Replace('_', ' ');
                                album = album.Trim();
                            }

                            // TODO: Update MP3 tags in file

                            if (!UpdateSong(title, artist, album, genre, 0, 0, 0, filename))
                            {
                                var song = new SONG() { TITLE = title, ARTIST = artist, ALBUM = album, GENRE = genre, LOCATION = filename, HASH = hash };
                                insertSong(song);
                            }
                        }
                        catch (Exception e1)
                        {
                            OnError(String.Format("{0} : {1} : {2}", filename, e1.Message, e1.StackTrace));
                        }
                    }
                }
                else if (Regex.IsMatch(filename, ".wma", RegexOptions.IgnoreCase))
                {
                    if (!InDb(filename))
                    {
                        try
                        {
                            long duration, bitrate, filesize;
                            GetWmaTags(filename, out title, out artist, out album, out genre, out duration, out bitrate, out filesize);

                            if (!UpdateSong(title, artist, album, genre, duration, bitrate, filesize, filename))
                            {
                                var song = new SONG() { TITLE = title, ARTIST = artist, ALBUM = album, GENRE = genre, DURATION = duration, BITRATE = bitrate, FILESIZE = filesize, HASH = hash };
                                insertSong(song);
                            }
                        }
                        catch (Exception e1)
                        {
                            OnError(String.Format("{0} : {1} : {2}", filename, e1.Message, e1.StackTrace));
                        }
                    }
                }
            }
            catch (Exception e2)
            {
                OnError(String.Format("{0} : {1} : {2}", filename, e2.Message, e2.StackTrace));
            }
        }

        public bool InDb(SONG song)
        {
            return InDb(song.LOCATION);
        }

        public bool InDb(String filename)
        {
            bool ret = false;

            try
            {
                using (var dataContext = new MusicEntities1())
                {
                    var songs = dataContext.SONG;

                    string filename1 = filename.ToLower();
                    int start = filename1.IndexOf("\\music\\");
                    if (start > -1)
                    {
                        filename = filename.Substring(start);
                    }

                    // Check if song already in db
                    SONG existingSong = dataContext.SONG.Select(s => s).Where(s => s.LOCATION.Contains(filename)).FirstOrDefault();
                    if (null != existingSong)
                    {
                        ret = true;
                    }
                }
            }
            catch (Exception ex)
            {
                OnError(String.Format("{0} : {1}", filename, ex.Message));
                ret = true;
                throw;
            }

            return ret;
        }

        public bool HashExists(SONG song)
        {
            bool ret = false;
            try
            {
                using (var dataContext = new MusicEntities1())
                {
                    var songs = dataContext.SONG;

                    string filename1 = song.LOCATION.Substring(2);

                    SONG existingSong = dataContext.SONG.Select(s => s).Where(s => s.HASH == song.HASH).FirstOrDefault();
                    if (null != existingSong)
                    {
                        ret = true;
                    }
                }
            }
            catch (Exception ex)
            {
                OnError(String.Format("{0}", ex.Message));
                throw;
            }

            return ret;
        }

        private bool FileInDb(String filename)
        {
            bool ret = false;

            try
            {
                using (var dataContext = new MusicEntities1())
                {
                    var songs = dataContext.SONG;

                    // Check if song already in db
                    IQueryable<SONG> query = from s in songs
                                             where (s.LOCATION == filename)
                                             select s;

                    if (query.Count() > 0)
                    {
                        ret = true;
                    }
                }
            }
            catch (Exception ex)
            {
                OnError(String.Format("{0} : {1}", filename, ex.Message));
                ret = true;
            }

            return ret;
        }

        public bool SongProcessingDone()
        {
            while (true)
            {
                foreach (DelegateResultPair delegateResultPair in ProcessFileDelgates)
                {
                    if (!delegateResultPair.Result.IsCompleted)
                    {
                        System.Diagnostics.Debug.WriteLine("Waiting for threads");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Thread completed.");
                        ProcessFileDelgates.Remove(delegateResultPair);
                        System.Diagnostics.Debug.WriteLine("Number of threads: " + ProcessFileDelgates.Count);
                        break;
                    }
                    delegateResultPair.Delegate.EndInvoke(delegateResultPair.Result);
                }
                if (ProcessFileDelgates.Count == 0)
                {
                    return true;
                }
            }
        }

    }
}

