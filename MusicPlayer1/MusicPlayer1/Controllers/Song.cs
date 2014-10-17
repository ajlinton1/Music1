using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace com.andrewlinton.AzureTable
{
	public class Song
	{
		public string Id {get;set;}

		public string DownloadUrl {get;set;}

		public string OriginalFilename {get;set;}

		public string FileSize {get;set;}

		public string Title {get;set;}

		public string Artist {get;set;}

		public string Album {get;set;}

		public string Genre {get;set;}

		public int Rating { get; set; }

		public string GDriveId { get; set; }

		private string location;

		public string Location {
			get 
			{
				string ret = string.Format("{0} {1} {2}", Artist, Album, Title);
				return ret; 
			}
			set
			{
				location = value;
			}
		}

		public string ETag { get; set; }

		public string PartitionKey { get; set; }

		public string RowKey { get; set; }

		public DateTimeOffset Timestamp { get; set; }

	}
}