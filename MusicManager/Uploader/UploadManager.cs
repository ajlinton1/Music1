using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Music;
using MusicManager;
using MusicManager.Properties;
using Azure;
using com.andrewlinton.music;

namespace Uploader
{
    public class UploadManager
    {
        static void Main(string[] args)
        {
            Upload(null, null, null, null);
        }

        public static void Upload(string connectionString, string accountName, string accountKey, string containerName)
        {
            int numSongs = 100;
/*            String[] desiredGenres = { "trance", "techno", "electronic", "ambient" ,"house",
                                         "beats", "big beat", "chill", "dub", "Electro", "grime", "hard psyche",
                                         "illbient", "jungle", "leftfield", "plunderphonic", "trip", "turn"}; */

            try
            {
                Console.WriteLine("Starting");

                var azureService = new AzureService(accountName, accountKey);
                var uploadProcessor = new UploadProcessor(containerName, accountName, accountKey);
                var musicDao = new MusicDao(connectionString, null, uploadProcessor, azureService);
                IEnumerable<String> songs = FtpManager.List();

                foreach (String song in songs)
                {
                    if (song.Length > 0)
                    {
                        Console.WriteLine("Deleting: {0}", song);
                        FtpManager.Delete(song);
                        String songIdString = song.Substring(5);
                        songIdString = songIdString.Substring(0, songIdString.IndexOf('.'));
                        int songId = Convert.ToInt32(songIdString);
                        var song1 = new SONG() { ID = songId, UPLOADED = false };
                        musicDao.UpdateSongUploadStatus(song1);
                    }
                }

                musicDao.UploadSongs(numSongs);

                Console.WriteLine("Done");
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
