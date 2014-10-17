using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.andrewlinton.music
{
	public class Genre
	{
		public string Name { get; set; }

		public int Count { get; set; }

		public bool Mellow { get; set; }

		public string ETag { get; set; }

		public string PartitionKey { get; set; }

		public string RowKey { get; set; }

		public DateTimeOffset Timestamp { get; set; }

	}
}
