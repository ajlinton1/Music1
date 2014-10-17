using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace com.andrewlinton.AzureTable
{
    public class MusicRepository
    {
        CloudTableClient cloudTableClient = null;

        public MusicRepository()
        {
            cloudTableClient = AzureTableService.GetCloudTableClient();
        }

        public void Set(string id, string downloadUrl)
        {
            var cloudTable = cloudTableClient.GetTableReference("songs");
            DynamicTableEntity dynamicTableEntity = new DynamicTableEntity();
            dynamicTableEntity.PartitionKey = "songs";
            dynamicTableEntity.RowKey = id;
            dynamicTableEntity["downloadUrl"] = new EntityProperty(downloadUrl);
            var to = TableOperation.InsertOrReplace(dynamicTableEntity);
            TableResult tr = cloudTable.Execute(to);
        }

		// file.Id, file.DownloadUrl, file.OriginalFilename, file.FileSize, song.TITLE, song.ARTIST, song.ALBUM, song.GENRE

		public void Set(string id, string downloadUrl, string originalFilename, long? fileSize, string title, string artist, string album, string genre)
		{
			var cloudTable = cloudTableClient.GetTableReference("songs");
			DynamicTableEntity dynamicTableEntity = new DynamicTableEntity();
			dynamicTableEntity.PartitionKey = "songs";
			dynamicTableEntity.RowKey = id;
			dynamicTableEntity["downloadUrl"] = new EntityProperty(downloadUrl);
			dynamicTableEntity["originalFilename"] = new EntityProperty(originalFilename);
			dynamicTableEntity["fileSize"] = new EntityProperty(fileSize.ToString());
			dynamicTableEntity["title"] = new EntityProperty(title);
			dynamicTableEntity["artist"] = new EntityProperty(artist);
			dynamicTableEntity["album"] = new EntityProperty(album);
			dynamicTableEntity["genre"] = new EntityProperty(genre);
			var to = TableOperation.InsertOrReplace(dynamicTableEntity);
			TableResult tr = cloudTable.Execute(to);
		}

		public IList<Song> GetSongs(int skip, string artist, string genre)
		{
			int maxResults = 100;

			maxResults = maxResults + skip;

			var songs = new List<Song>();
			var tableQuery = new TableQuery();
			tableQuery.TakeCount = maxResults;

			string filter = "";
			if (!string.IsNullOrEmpty(artist))
			{
				filter = string.Format("artist eq '{0}'", artist);
			}

			if (!string.IsNullOrEmpty(genre))
			{
				filter = string.Format("PartitionKey eq '{0}'", genre);
			}

			if (!string.IsNullOrEmpty(filter))
			{
				tableQuery.FilterString = filter;
			}

			string tableName;
			if (tableQuery.FilterString!=null)
			{
				tableName = "songs";
			}
			else
			{
				tableName = "recent";
			}
			var cloudTable = cloudTableClient.GetTableReference(tableName);

			var results = cloudTable.ExecuteQuery(tableQuery).Skip(skip).ToList();

			foreach (var result in results)
			{
					var song = new Song();
					song.ETag = result.ETag;
					song.PartitionKey = result.PartitionKey;
					song.RowKey = result.RowKey;
					song.Timestamp = result.Timestamp;
					if (result.Properties.ContainsKey("downloadUrl") )
					{
						song.DownloadUrl = result["downloadUrl"].StringValue;
					}
					if (result.Properties.ContainsKey("originalFilename"))
					{
						song.OriginalFilename = result["originalFilename"].StringValue;
					}
					if (result.Properties.ContainsKey("fileSize"))
					{
						song.FileSize = result["fileSize"].StringValue;
					}
					if (result.Properties.ContainsKey("title"))
					{
						song.Title = result["title"].StringValue;
					}
					if (result.Properties.ContainsKey("artist"))
					{
						song.Artist = result["artist"].StringValue;
					}
					if (result.Properties.ContainsKey("album"))
					{
						song.Album = result["album"].StringValue;
					}
					if (result.Properties.ContainsKey("genre"))
					{
						song.Genre = result["genre"].StringValue;
					}
					if (result.Properties.ContainsKey("rating"))
					{
						song.Rating = result["rating"].Int32Value.Value;
					}
					if (result.Properties.ContainsKey("gdriveId"))
					{
						song.GDriveId = result["gdriveId"].StringValue;
					}
					songs.Add(song);
			}
			return songs;
		}

		public IList<Genre> GetGenres()
		{
			var genres = new List<Genre>();
			var cloudTable = cloudTableClient.GetTableReference("genres");
			var tableQuery = new TableQuery();
			var results = cloudTable.ExecuteQuery(tableQuery);

			foreach (var result in results)
			{
				var genre = new Genre();
				genre.ETag = result.ETag;
				genre.PartitionKey = result.PartitionKey;
				genre.Name = result.RowKey.ToString();
				genre.Timestamp = result.Timestamp;
				if (result.Properties.ContainsKey("numSongs"))
				{
					genre.NumSongs = Convert.ToInt32(result["numSongs"].StringValue);
				}
				genres.Add(genre);
			}
			return genres;
		}

    }
}
