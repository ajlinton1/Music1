using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using MusicManager;
using Music;
using Azure;

namespace MusicManagerConsole
{
    class MusicManager
    {
		Model model;
		IMusicDao musicDao;
		static string musicFolder = null;

        static void Main(string[] args)
        {
			if ((args!=null) && (args.Length>0) && (args[0])!=null)
			{
				musicFolder = args[0];
			}
			else
			{
				musicFolder = Properties.Settings.Default.MusicFolder;
			}
            int threadsNum = Environment.ProcessorCount;
            int maxSongsToProcess = Int32.MaxValue;

            if (args.Length > 0)
            {
                threadsNum = Convert.ToInt32(args[0]);
            }
            if (args.Length > 1)
            {
                maxSongsToProcess = Convert.ToInt32(args[1]);
            }

            Trace.WriteLine(String.Format("Starting MusicManager: Threads: {0} Max Songs {1}", threadsNum, maxSongsToProcess));

            var musicManager = new MusicManager(threadsNum, maxSongsToProcess, Properties.Settings.Default.ConnectionString, musicFolder);

			try
			{
				musicManager.Process();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
        }

        public MusicManager(int threadsNum, int maxSongsToProcess, string connectionString, string musicFolder)
        {
            model = new Model();

            var azureService = new AzureService(Properties.Settings.Default.AzureAccountName, Properties.Settings.Default.AzureAccountKey);
            var uploadProcessor = new UploadProcessor(Properties.Settings.Default.AzureContainerName, Properties.Settings.Default.AzureAccountName, Properties.Settings.Default.AzureAccountKey);
            uploadProcessor.OnError += Error;
            uploadProcessor.OnStatus += Status;
            musicDao = new MusicDao(connectionString, new string[] { Properties.Settings.Default.MusicFolder }, uploadProcessor, azureService);
            musicDao.OnError += Error;
            musicDao.OnNewArtist += AddArtist;
            musicDao.OnNewSong += NewSong;
            musicDao.OnProcessFile += Status;
            musicDao.OnStatus += Status;
        }

		public void Process()
		{
			// model.Reindex(threadsNum, new string[] { Properties.Settings.Default.MusicFolder }, musicDao);
			model.AddNew(musicFolder, musicDao);
		}

        public void NewSong(String newSongName)
        {
            Trace.WriteLine("New Song added: " + newSongName);
        }

        public void Error(String errorInfo)
        {
            Trace.TraceError("Error: " + errorInfo);
        }

        public void Status(String statusInfo)
        {
            Trace.WriteLine("Status: " + statusInfo.ToString());
        }

        public void AddArtist(String artistName)
        {
            Trace.WriteLine("Add artist: " + artistName);
        }

    }
}
