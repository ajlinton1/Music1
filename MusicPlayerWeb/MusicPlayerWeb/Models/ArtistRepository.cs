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
    public class ArtistRepository
    {
        const int maxReturn = 100;

        public List<Tuple<string, string>> Get(int take, int skip)
        {
            var artists = new List<Tuple<string, string>>();

            if (take > maxReturn)
            {
                take = maxReturn;
            }

            var cloudTableClient = AzureService.GetCloudTableClient();
            var cloudTable = cloudTableClient.GetTableReference("artists");

            var query = new TableQuery();
//            query.TakeCount = take;
//            query.Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Ludacris"));
            var results = cloudTable.ExecuteQuery(query);

            results = results.Skip(skip);
            results = results.Take(take);

            foreach (DynamicTableEntity result in results)
            {
                string artistName = result.RowKey;
                string songCount = result.Properties["numSongs"].StringValue;
                var artist = new Tuple<string, string>(artistName, songCount);
                artists.Add(artist);
            }

            return artists;
        }

    }
}