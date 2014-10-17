using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using MusicPlayerWeb.Properties;

namespace MusicPlayerWeb.Models
{
    public class GenreRepository
    {
        public List<Tuple<string,string>> Get()
        {
            var genres = new List<Tuple<string, string>>();

            var cloudTableClient = AzureService.GetCloudTableClient();
            var cloudTable = cloudTableClient.GetTableReference("genres");
            TableQuery query = new TableQuery();
            var results = cloudTable.ExecuteQuery(query);
            foreach (DynamicTableEntity result in results)
            {
                string genre = result.RowKey;
                string songCount = result.Properties["numSongs"].StringValue;
                var genreTuple = new Tuple<string, string>(genre, songCount);
                genres.Add(genreTuple);
            }

            return genres;
        }

    }
}