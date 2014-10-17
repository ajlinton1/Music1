using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Net;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using GDriveConsole.Properties;
using com.andrewlinton.AzureTable;
using Music;
using GDrive;
using Azure;
using MusicManager;
using com.andrewlinton.music;

namespace GDriveConsole
{
    public class GDriveConsole
    {
		CloudQueue queue;
        AzureService azureService;
        CloudQueueClient cloudQueueClient;
		Repository logRepository;
		SongRepository songRepository;

		public GDriveConsole()
		{
			AzureService azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
			cloudQueueClient = azureService.GetCloudQueueClient();

            queue = cloudQueueClient.GetQueueReference("gdrive");

            //Check whether the queue exists, and create it if it does not.
            if (!queue.Exists())
            {
                queue.Create();
            }

			logRepository = new Repository("log");
			songRepository = new SongRepository(logRepository);
		}

        static void Main(string[] args)
        {
            Debug.WriteLine("Starting");
            var gDriveService = new GDriveService();
//            string search = "title contains '.mp3'";
            string search = "";
            gDriveService.ProcessFiles(search, SetGDriveUrl, null);
            Debug.WriteLine("Finished");
        }

		public static void SetGDriveUrl(Google.Apis.Drive.v2.Data.File file)
		{
			try
			{
				if ((file.OriginalFilename == null) || (file.OriginalFilename.IndexOf(".mp3") == -1))
				{
					return;
				}
				Debug.WriteLine("Processing: " + file.OriginalFilename);

				using (var context = new MusicEntities1())
				{
					bool found = false;
					var songs = from s in context.SONG
								//								where ((file.OriginalFilename.Contains(s.LOCATION)) && (s.FILESIZE == file.FileSize))
								where ((s.FILESIZE == file.FileSize) && (s.FILESIZE > 0))
								select s;
					var songArray = songs.ToArray();
					foreach (var song in songArray)
					{
						if (song.GDRIVE == null)
						{
							song.GDRIVE = file.WebContentLink;
							context.SaveChanges();
							Debug.WriteLine(string.Format("Updated: {0} {1}", file.OriginalFilename, file.WebContentLink));
						}
						found = true;
					}
					if (!found)
					{
						Debug.WriteLine(string.Format("Not found: {0} {1}", file.OriginalFilename, file.FileSize));
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(string.Format("Error: {0}", ex.Message));
			}
		}


		public static void PopulateArtists()
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
						Debug.WriteLine("{0}", ex.Message);
					}
				}
			}

		}

		public void PopulateGDriveQueue()
		{
			Debug.WriteLine("PopulateGDriveQueue starting");

			var gDriveService = new GDriveService();
			string search = "";
			gDriveService.ProcessFiles(search, PopulateQueue, null);
		}

		public void PopulateQueue(Google.Apis.Drive.v2.Data.File file)
		{
			MemoryStream memoryStream = new MemoryStream();

			// Create the JSON serializer.
			DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(file.GetType());
			jsonSerializer.WriteObject(memoryStream, file);

			// Get the JSON string from the memory stream.
			string messageString = Encoding.Default.GetString(memoryStream.ToArray());

			CloudQueueMessage message = new CloudQueueMessage(messageString);
			queue.AddMessage(message);
		}

		public void ProcessMessage(CloudQueueMessage message, Google.Apis.Drive.v2.Data.File file, DataContractJsonSerializer jsonSerializer)
		{
			try
			{
				MemoryStream memoryStream = new MemoryStream(Encoding.Default.GetBytes(message.AsString));
				file = jsonSerializer.ReadObject(memoryStream) as Google.Apis.Drive.v2.Data.File;
				ProcessGDriveFile(file, ProcessFile);
				queue.DeleteMessage(message);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("{0} : {1}", ex.Message, ex.StackTrace);
			}
		}

		public void DrainQueue()
		{
			Google.Apis.Drive.v2.Data.File file = new Google.Apis.Drive.v2.Data.File();
			DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(file.GetType());
			CloudQueueMessage message = null;
			do
			{
				try
				{
					message = queue.GetMessage();
					if (message != null)
					{
						try
						{
							MemoryStream memoryStream = new MemoryStream(Encoding.Default.GetBytes(message.AsString));
							file = jsonSerializer.ReadObject(memoryStream) as Google.Apis.Drive.v2.Data.File;
							ProcessGDriveFile(file, ProcessFile);  
						}
						finally
						{
							queue.DeleteMessage(message);
						} 
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Exception: {0} {1}", ex.Message.ToString(), ex.StackTrace.ToString());
				}
			}
			while (message != null);
		}


		public void ProcessGDriveFile(Google.Apis.Drive.v2.Data.File file, Action<string, Google.Apis.Drive.v2.Data.File, DynamicTableEntity> processFile)
		{
			if (file.WebContentLink==null)
			{
				Debug.WriteLine("File.WebContentLink == null");
				return;
			}

			if (file.MimeType != "audio/mpeg")
			{
				return;
			}

			DynamicTableEntity existingSong = songRepository.Get(file.Id);
			if ((existingSong == null) || (file.ModifiedDate.Value.CompareTo(existingSong.Timestamp.DateTime) > 0))
			{
				WebClient webClient = new WebClient();

				string filename = Path.GetTempFileName();
				string downloadUrl = string.Format("https://googledrive.com/host/{0}", file.Id);
				webClient.DownloadFile(downloadUrl, filename);

				try
				{
					System.IO.FileInfo fileInfo = new System.IO.FileInfo(filename);
					if (fileInfo.Length > 40000)
					{
						processFile(filename, file, existingSong);
					}
					else
					{
						Debug.WriteLine("Small file downloaded. Google may be throttling");
					}
				}
				finally
				{
					System.IO.File.Delete(filename);
				}
			}
		}

		public void DownloadAndInsert(Google.Apis.Drive.v2.Data.File file)
		{
			ProcessGDriveFile(file, ProcessFile);
		}

		public SONG GetSQLData(Google.Apis.Drive.v2.Data.File file)
		{
			if ((file.OriginalFilename == null) || (file.OriginalFilename.IndexOf(".mp3") == -1))
			{
				return null;
			}

			using (var context = new MusicEntities1())
			{
				var songs = from s in context.SONG
							where ((s.FILESIZE == file.FileSize) && (s.FILESIZE > 0))
							select s;
				var songArray = songs.ToArray();
				if (songArray.Length>0)
				{
					return songArray[0];
				}
			}
			return null;
		}

		public void ProcessFile(string filename, Google.Apis.Drive.v2.Data.File file, DynamicTableEntity existingFile)
		{
			SONG song = new SONG();
			song.LOCATION = filename;
			song.Populate();

			if (song.ARTIST == null)
			{
				Debug.WriteLine("song.ARTIST == null");
				return;
			}


			if (existingFile == null)
			{
				songRepository.Add(file.Id, file.WebContentLink, file.OriginalFilename, file.FileSize, song.TITLE, song.ARTIST, song.ALBUM, song.GENRE, song.LOCATION, song.RATING, song.TrackNumber);
			}
			else
			{
				songRepository.Update(file.Id, file.WebContentLink, file.OriginalFilename, file.FileSize, song.TITLE, song.ARTIST, song.ALBUM, song.GENRE, song.LOCATION, song.RATING, song.TrackNumber);
			}
		}
    }
}
