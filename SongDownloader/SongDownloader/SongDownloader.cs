using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.WindowsAzure.StorageClient;
using SongDownloader.Properties;
using Azure;
using SongDownloader.MusicService;

namespace SongDownloader
{
    class SongDownloader
    {
        static AzureService azureService;

        static void Main(string[] args)
        {
            CloudQueueClient cloudQueueClient;
            CloudQueue cloudQueue;
            string folderName;
            string machineName;
            string queueName;
            string musicFolder = null;
            MusicEntities musicEntities = null;

            try
            {
                Trace.WriteLine("SongDownloader started");

                musicFolder = args[0];
                azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
                Uri uri = new Uri(Settings.Default.MusicServiceUrl);
                musicEntities = new MusicEntities(uri);

                folderName = musicFolder;
                machineName = Environment.MachineName;
                queueName = machineName.ToLower();
                cloudQueueClient = azureService.GetCloudQueueClient();
                Debug.Assert(cloudQueueClient != null, "cloudQueueClient != null");
                cloudQueue = cloudQueueClient.GetQueueReference(queueName);
                Debug.Assert(cloudQueue != null, "cloudQueue != null");

                CloudQueueMessage message = null;
                do
                {
                    try
                    {
                        message = cloudQueue.GetMessage();
                    }
                    catch (Exception e3)
                    {
                        Trace.TraceError("Error getting cloudQueue: {0} {1}", e3.Message, e3.StackTrace);
                        throw;
                    }

                    if (message != null)
                    {
                        string messageStr = message.AsString;

                        Trace.WriteLine(string.Format("Dequeuing message: {0}", messageStr));

                        string idString;
                        string location = null;
                        int id = -1;
                        int loc = messageStr.IndexOf("---");
                        SONG song = new SONG();
                        if (loc > -1)
                        {
                            idString = messageStr.Substring(0, loc);
                            location = messageStr.Substring(loc + 3);
                            song.LOCATION = location;
                            id = Convert.ToInt32(idString);
                            song.ID = id;
                        }
                        else
                        {
                            idString = messageStr;
                            id = Convert.ToInt32(idString);
                            song = musicEntities.SONG
                            .Where(s => s.ID == id)
                            .FirstOrDefault();
                        }

                        Trace.WriteLine(string.Format("Processing song: {0}", id));

                        FileInfo fileInfo = new FileInfo(song.LOCATION);
                        String[] s1 = song.LOCATION.Split(new char[] { '\\' });
                        String artistFolder = null;
                        String name = fileInfo.Name;
                        String filename = null;
                        if (s1.Length > 3)
                        {
                            artistFolder = folderName;
                            for (int i = 2; i < s1.Length - 1; i++)
                            {
                                artistFolder = artistFolder + "\\" + s1[i];
                                DirectoryInfo directoryInfo = new DirectoryInfo(artistFolder);
                                if (!directoryInfo.Exists)
                                {
                                    Directory.CreateDirectory(artistFolder);
                                }
                            }
                            filename = String.Format("{0}\\{1}", artistFolder, name);
                        }
                        else
                        {
                            filename = String.Format("{0}\\{1}", folderName, name);
                        }

                        byte[] existingHash = null;
                        if (File.Exists(filename))
                        {
                            Trace.WriteLine(string.Format("Song exists: {0}", filename));

                            existingHash = GenerateHashCode(filename);

                            if (!Compare(existingHash, song.HASH))
                            {
                            }
                            else
                            {
                                Trace.WriteLine(String.Format("Song already exists:  {0}", filename));
                            }
                        }
                        else
                        {
                            DownloadSong(filename, song);
                        }
                    }
                    if (message != null)
                    {
                        cloudQueue.DeleteMessage(message);
                    }
                }
                while (message != null);
                Trace.WriteLine("SongDownloader finished");
            }
            catch (Exception ex)
            {
                try
                {
                    Trace.TraceError("Error: DownloadSongsQueue");
                    Trace.TraceError("Error: {0} {1}", ex.Message, ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        Trace.TraceError("Error: {0} {1} {2} {3}", ex.Message, ex.StackTrace, ex.InnerException.Message, ex.InnerException);
                    }
                }
                catch (Exception e1)
                {
                    Trace.TraceError("Error logging exception: {0} {1}", e1.Message, e1.StackTrace);
                }
            }
        }

        public static byte[] GenerateHashCode(String fileName)
        {
            byte[] result = null;
            HashAlgorithm sha = new SHA1CryptoServiceProvider();
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                result = sha.ComputeHash(fileStream);
            }
            return result;
        }

        public static bool Compare(byte[] a, byte[] b)
        {
            if ((null == a) || (null == b))
            {
                return false;
            }

            bool ret = true;

            for (int i = 0; i < a.Length; i++)
            {
                if (i == b.Length)
                {
                    return false;
                }
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return ret;
        }

        static void DownloadSong(string filename, SONG song)
        {
            Trace.WriteLine(String.Format("Downloading {0}", filename));
            try
            {
                String key = String.Format("{0}.mp3", song.ID);
                azureService.DownloadFile("public", filename, key);
            }
            catch (Exception)
            {
                Trace.TraceError("Unable to download blob {0} {1}", song.ID, filename);
            }
        }

    }


}
