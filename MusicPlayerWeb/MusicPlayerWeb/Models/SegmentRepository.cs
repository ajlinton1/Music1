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
    public class SegmentRepository
    {
        const int maxReturn = 100;

        public List<Segment> Get(int take, int skip)
        {
            var segements = new List<Segment>();

            if (take > maxReturn)
            {
                take = maxReturn;
            }

            var cloudTableClient = AzureService.GetCloudTableClient();
            var cloudTable = cloudTableClient.GetTableReference("segments");

            var query = new TableQuery();
            var results = cloudTable.ExecuteQuery(query);
            results = results.Skip(skip);
            results = results.Take(take);

            foreach (DynamicTableEntity result in results)
            {
                var segment = new Segment();
                segment.SegmentAll = result.RowKey;
                segment.Genre = result.Properties["genre"].StringValue;
                if (result.Properties.Keys.Contains("segement0"))
                {
                    segment.Segment0 = result.Properties["segement0"].StringValue;
                }
                if (result.Properties.Keys.Contains("segement1"))
                {
                    segment.Segment1 = result.Properties["segement1"].StringValue;
                }
                if (result.Properties.Keys.Contains("segement2"))
                {
                    segment.Segment2 = result.Properties["segement2"].StringValue;
                }

                segements.Add(segment);
            }

            return segements;
        }
    }
}