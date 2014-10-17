using System;

namespace Microsoft.Samples.MediaCatalog
{
	public struct TrackInfo
	{
		internal string   title;
		internal string   author;
		internal string   albumTitle;
		internal string   albumArtist;
		internal string   publisher;
		internal string   genre;
		internal uint     track;
		internal TimeSpan duration;
		internal uint     bitRate;
		internal bool     isProtected;
		internal ulong    fileSize;

		public string   Title 
			{ get { return title; } }

		public string   Author 
			{ get { return author; } }

		public string   AlbumTitle 
			{ get { return albumTitle; } }

		public string   AlbumArtist 
			{ get { return albumArtist; } }

		public string   Publisher 
			{ get { return publisher; } }

		public string   Genre 
			{ get { return genre; } } 

		public uint     Track 
			{ get { return track; } }

		public TimeSpan Duration 
			{ get { return duration; } }

		public uint     BitRate 
			{ get { return bitRate; } }

		public bool     IsProtected
			{ get { return isProtected; } }

		public ulong    FileSize 
			{ get { return fileSize; } }	
	}
}
