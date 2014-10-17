using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.andrewlinton.AzureTable
{
	public class Genre
	{
		public string Name { get; set; }

		public int NumSongs { get; set; }

		public string ETag { get; set; }

		public string PartitionKey { get; set; }

		public string RowKey { get; set; }

		public DateTimeOffset Timestamp { get; set; }

	}
}