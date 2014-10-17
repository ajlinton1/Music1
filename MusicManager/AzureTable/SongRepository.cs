using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace com.andrewlinton.AzureTable
{
    public class SongRepository
    {
        CloudTableClient cloudTableClient = null;
		Repository logRepository = null;

		public SongRepository(Repository logRepository)
        {
            cloudTableClient = AzureTableService.GetCloudTableClient();
			this.logRepository = logRepository;
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

		public void Add(string id, string downloadUrl, string originalFilename, long? fileSize, string title, string artist, string album, string genre, string location, int rating, string trackNumber)
		{
			genre = genre.Replace('/', ' ');

			var cloudTable = cloudTableClient.GetTableReference("songs");
			DynamicTableEntity dynamicTableEntity = new DynamicTableEntity();
			dynamicTableEntity.PartitionKey = genre;
			dynamicTableEntity.RowKey = id;
			dynamicTableEntity["downloadUrl"] = new EntityProperty(downloadUrl);
			dynamicTableEntity["originalFilename"] = new EntityProperty(originalFilename);
			dynamicTableEntity["fileSize"] = new EntityProperty(fileSize.ToString());
			dynamicTableEntity["title"] = new EntityProperty(title);
			dynamicTableEntity["artist"] = new EntityProperty(artist);
			dynamicTableEntity["album"] = new EntityProperty(album);
			dynamicTableEntity["genre"] = new EntityProperty(genre);
			dynamicTableEntity["location"] = new EntityProperty(location);
			dynamicTableEntity["rating"] = new EntityProperty(rating);
			dynamicTableEntity["gdriveId"] = new EntityProperty(id);
			if (!string.IsNullOrEmpty(trackNumber))
			{
				dynamicTableEntity["trackNumber"] = new EntityProperty(trackNumber);
			}
			var to = TableOperation.InsertOrReplace(dynamicTableEntity);
			TableResult tr = cloudTable.Execute(to);

			logRepository.Add(dynamicTableEntity, "SongAdd", null);
		}

		public void Update(string id, string downloadUrl, string originalFilename, long? fileSize, string title, string artist, string album, string genre, string location, int rating, string trackNumber)
		{
			genre = genre.Replace('/', ' ');

			var cloudTable = cloudTableClient.GetTableReference("songs");
			DynamicTableEntity dynamicTableEntity = new DynamicTableEntity();
			dynamicTableEntity.PartitionKey = genre;
			dynamicTableEntity.RowKey = id;
			dynamicTableEntity["downloadUrl"] = new EntityProperty(downloadUrl);
			dynamicTableEntity["originalFilename"] = new EntityProperty(originalFilename);
			dynamicTableEntity["fileSize"] = new EntityProperty(fileSize.ToString());
			dynamicTableEntity["title"] = new EntityProperty(title);
			dynamicTableEntity["artist"] = new EntityProperty(artist);
			dynamicTableEntity["album"] = new EntityProperty(album);
			dynamicTableEntity["genre"] = new EntityProperty(genre);
			dynamicTableEntity["location"] = new EntityProperty(location);
			dynamicTableEntity["rating"] = new EntityProperty(rating);
			dynamicTableEntity["gdriveId"] = new EntityProperty(id);
			if (!string.IsNullOrEmpty(trackNumber))
			{
				dynamicTableEntity["trackNumber"] = new EntityProperty(trackNumber);
			}
			var to = TableOperation.InsertOrReplace(dynamicTableEntity);
			TableResult tr = cloudTable.Execute(to);

			logRepository.Add(dynamicTableEntity, "SongUpdate", null);
		}

		public DynamicTableEntity Get(string id)
		{
			var cloudTable = cloudTableClient.GetTableReference("songs");
			var tableQuery = new TableQuery();
			tableQuery.TakeCount = 1;
			tableQuery.FilterString = string.Format("RowKey eq '{0}'", id);
			var results = cloudTable.ExecuteQuery(tableQuery);

			foreach (var result in results)
			{
				return result;
			}
			return null;
		}

		public Dictionary<string, int> ProcessGenres(Action<Dictionary<string,int>> genreProcessor)
		{
			var genres = new Dictionary<string, int>();

			var cloudTable = cloudTableClient.GetTableReference("songs");
			var tableQuery = new TableQuery();
			TableContinuationToken tableContinuationToken = null; 

			do
			{
				var tableQuerySegment = cloudTable.ExecuteQuerySegmented(tableQuery, tableContinuationToken);
				tableContinuationToken = tableQuerySegment.ContinuationToken;
				foreach (var result in tableQuerySegment.Results)
				{
					if (result.Properties.ContainsKey("genre"))
					{
						string genre = result["genre"].StringValue;
						if (genres.Keys.Contains(genre))
						{
							genres[genre]++;
						}
						else
						{
							genres.Add(genre, 1);
						}
						Debug.WriteLine(genre);
					}
				}
				genreProcessor(genres);
			}
			while (tableContinuationToken != null);
			return genres;
		}

		public void Process(string filter, Action<CloudTable, DynamicTableEntity> songProcessor, int? maxSongsToProcess)
		{
			int songsProcessed = 0;
			var cloudTable = cloudTableClient.GetTableReference("songs");
			var tableQuery = new TableQuery();
			tableQuery.FilterString = filter;
			TableContinuationToken tableContinuationToken = null;

			do
			{
				var tableQuerySegment = cloudTable.ExecuteQuerySegmented(tableQuery, tableContinuationToken);
				tableContinuationToken = tableQuerySegment.ContinuationToken;
				foreach (var result in tableQuerySegment.Results)
				{
					songProcessor(cloudTable, result);
					if (maxSongsToProcess.HasValue)
					{
						songsProcessed++;
						if (songsProcessed>maxSongsToProcess.Value)
						{
							return;
						}
					}
				}
			}
			while (tableContinuationToken != null);
		}

		public void Delete(CloudTable cloudTable, DynamicTableEntity song)
		{
			TableResult tr = cloudTable.Execute(TableOperation.Delete(song));
		}
    }
}
