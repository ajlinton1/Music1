using System;

namespace Microsoft.Samples.MediaCatalog
{
	public class TrackInfoEventArgs : EventArgs
	{
		public TrackInfoEventArgs(string title, string author)
		{
			this.title = title;
			this.author = author;
		}

		private string title;
		private string author;

		public string Title
		{
			get { return this.title; }
		}

		public string Author
		{
			get { return this.author; }
		}
	}
}
