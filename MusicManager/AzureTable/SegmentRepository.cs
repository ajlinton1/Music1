using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.RetryPolicies;


namespace com.andrewlinton.AzureTable
{
    public class SegmentRepository
    {
        CloudTableClient cloudTableClient = null;

        public SegmentRepository()
        {
            cloudTableClient = AzureTableService.GetCloudTableClient();
        }

        public void Set(string genre, string segementNorm, string segment)
        {
            var cloudTable = cloudTableClient.GetTableReference("segments");
            DynamicTableEntity dynamicTableEntity = new DynamicTableEntity();
            dynamicTableEntity.PartitionKey = "segments";
            dynamicTableEntity.RowKey = segementNorm;
            dynamicTableEntity["genre"] = new EntityProperty(genre);
            char[] c = { '\\' };
            string[] s = segment.Split(c);
            if (s.Length > 0)
            {
                dynamicTableEntity["segment0"] = new EntityProperty(s[0]);
            }
            if (s.Length > 1)
            {
                dynamicTableEntity["segment1"] = new EntityProperty(s[1]);
            }
            if (s.Length > 2)
            {
                dynamicTableEntity["segment2"] = new EntityProperty(s[2]);
            }
            if (s.Length > 3)
            {
                dynamicTableEntity["segment3"] = new EntityProperty(s[3]);
            }
            if (s.Length > 4)
            {
                dynamicTableEntity["segment4"] = new EntityProperty(s[4]);
            }
            var to = TableOperation.Insert(dynamicTableEntity);
            TableResult tr = cloudTable.Execute(to);
        }

        public void Reset()
        {
            var cloudTable = cloudTableClient.GetTableReference("segments");
            var query = new TableQuery();
            var segments = cloudTable.ExecuteQuery(query);

            foreach (DynamicTableEntity segment in segments)
            {
                var to = TableOperation.Delete(segment);
                TableResult tr = cloudTable.Execute(to);
            }
        }

    }
}
