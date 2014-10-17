using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Auth;
using SqlAzureData;
using Azure;

namespace Music
{
    public class SongDownloader
    {
        String containerName = "songs";

        AzureService azureService;

        public event Action<SONG, String> OnSongFound;

        public event Action<String, String> OnSongNotFound;

        public SongDownloader(string accountName, string accountKey)
        {
            azureService = new AzureService(accountName, accountKey);
        }

        public void DownloadSongs(String folderName)
        {
            String tempFolderName = folderName + "\\Temp";

            CloudBlobClient blobClient = azureService.GetCloudBlobContainer();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            IEnumerable<IListBlobItem> blobs = container.ListBlobs();
            foreach (var blob in blobs)
            {
                Debug.WriteLine("{0}", blob.Uri);
                int id = Convert.ToInt32(blob.Uri.Segments[2]);

                SONG song = null;
                using (var context = new MusicEntities())
                {
                    var result = (from s in context.SONG
                                  where s.ID == id
                                  select s);
                    if (result.Count() > 0)
                    {
//                        song = result.First();
                        OnSongFound(song, folderName);
                    }
                    else
                    {
                        Debug.WriteLine("No song record for blob {0}", id);
                        OnSongNotFound(id.ToString(), tempFolderName);
                    }
                }
            }
        }

        public void DownloadBlob(String id, String folderName)
        {
            Debug.WriteLine(String.Format("Downloading blog {0}", id));

            String fileName = String.Format("{0}\\{1}.mp3", folderName, id);

            if (!File.Exists(fileName))
            {
                azureService.DownloadFile("songs", fileName, id);
            }

            azureService.DeleteBlob(containerName, id);
        }
        
        public void DownloadSong(SONG song, String folderName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(song.LOCATION);
                String[] s = song.LOCATION.Split(new char[] { '\\' });
                String artistFolder = null;
                String name = fileInfo.Name;
                String fileName = null;
                if (s.Length > 3)
                {
                    artistFolder = folderName;
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
                    fileName = String.Format("{0}\\{1}", folderName, name);
                }

                if (!File.Exists(fileName))
                {
                    Debug.WriteLine(String.Format("Downloading {0}", fileName));
                    azureService.DownloadFile("songs", fileName, song.ID.ToString());
                }
                else
                {
                    Debug.WriteLine(String.Format("Song already exists:  {0}", fileName));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
