using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using Music;
using Azure;
using com.andrewlinton.music;
using com.andrewlinton.music.Properties;

namespace Music
{
    public class UploadProcessor : IUploadProcessor
    {
        bool running;
        String containerName;
        MusicDao musicDao = null;
        ConcurrentQueue<Tuple<String, String>> queue;
        AzureService azureService;
        public event Action<String> OnStatus;
        public event Action<String> OnError;

        public int FilesUploaded { get; set; }

        private UploadProcessor()
        {
        }

        public UploadProcessor(string containerName, string accountName, string accountKey)
        {
            this.containerName = containerName;
            azureService = new AzureService(accountName, accountKey);
            queue = new ConcurrentQueue<Tuple<String, String>>();
            running = true;
        }

        public void Enqueue(Tuple<String, String> fileinfo)
        {
            queue.Enqueue(fileinfo);
        }

        public bool UploadFile(Tuple<String, String> fileinfo)
        {
            OnStatus(String.Format("Uploading: {0}", fileinfo.Item1));
            string filename = fileinfo.Item1;
            string key = fileinfo.Item2;
            String key1 = key + ".mp3";
            var uploadStatus = azureService.StoreFile(filename, containerName, key1);
            if (uploadStatus)
            {
                OnStatus(String.Format("Uploaded: {0}", fileinfo.Item1));
            }
            else
            {
                OnStatus(String.Format("Upload failed: {0}", fileinfo.Item1));
            }
            return uploadStatus;
        }

        public void DrainQueue()
        {
            Tuple<String, String> fileInfo;

            while ((running == true) && (queue.Count > 0))
            {
                if (queue.TryDequeue(out fileInfo))
                {
                    try
                    {
                        string filename = fileInfo.Item1;
                        string key = fileInfo.Item2;
                        FilesUploaded++;
                        Debug.WriteLine("Uploading: " + filename);
                        azureService.StoreFile(filename, containerName, key);
                        var song = new SONG() { ID = Convert.ToInt32(key), UPLOADED = true };
                        musicDao.UpdateSongUploadStatus(song);
                        Debug.WriteLine("Upload succeeded: " + filename);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(String.Format("{0}:{1}", ex.Message, ex.StackTrace));
                    }
                }
                else if (running)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    return;
                }
            }
        } 
    }
}
