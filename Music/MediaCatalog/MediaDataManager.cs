using System;
using System.IO;

namespace Microsoft.Samples.MediaCatalog
{
	/// <summary>
	/// Acts as a high-level API for retrieving information from one or more 
	/// media files. You can use this class to create a DataSet containing
	/// metadata attributes from a tree of directories accessed recursively.
	/// </summary>
	public class MediaDataManager
	{
		/// <summary>
		/// Used to fire TrackAdded events.
		/// </summary>
		public delegate void TrackAddedEventHandler(object sender, TrackInfoEventArgs e);

		/// <summary>
		/// An event that is triggered whenever a new track is added. Useful
		/// when you're running a long recursive operation.
		/// </summary>
		public event TrackAddedEventHandler TrackAdded;

		/// <summary>
		/// Retrieves structured property information for the given media file.
		/// </summary>
		/// <param name="filename">Windows Media File (.asf, .wma, .wmv file)</param>
		/// <returns>TrackInfo object containing commonly-used fields</returns>
		public TrackInfo RetrieveTrackInfo(string filename)
		{
			TrackInfo ti = new TrackInfo();
			
			using (MetadataEditor md = new MetadataEditor(filename))
			{
				ti.title = md[MediaMetadata.Title] as string;
				ti.author = md[MediaMetadata.Author] as string;
				ti.albumTitle = md[MediaMetadata.AlbumTitle] as string;
				ti.albumArtist = md[MediaMetadata.AlbumArtist] as string;
				ti.publisher = md[MediaMetadata.Publisher] as string;
				ti.genre = md[MediaMetadata.Genre] as string;
				
				object o = md[MediaMetadata.TrackNumber];
				ti.track = (o == null ? 0 : (uint) o);

				o = md[MediaMetadata.Duration];
				ti.duration = new TimeSpan((o == null ? 0 : (long)(ulong) o));

				o = md[MediaMetadata.BitRate];
				ti.bitRate = (o == null ? 0 : (uint) o);

				o = md[MediaMetadata.IsProtected];
				ti.isProtected = (o == null ? false : (bool) o);

				o = md[MediaMetadata.FileSize];
				ti.fileSize = (o == null ? 0 : (ulong) o);
			}

			return ti;
		}


		/// <summary>
		/// Used internally to fill a row of a DataSet with metadata for the given media file.
		/// </summary>
		/// <param name="filename">Windows Media File (.wma file)</param></param>
		/// <param name="row">The TrackRow to be filled with data</param>
		private void RetrieveTrackRow(string filename, ref MediaData.TrackRow row)
		{
			MediaData.TrackDataTable tab = new MediaData.TrackDataTable();

			using (MetadataEditor md = new MetadataEditor(filename))
			{
				row.Title = md[MediaMetadata.Title] as string;
				row.Author = md[MediaMetadata.Author] as string;
				row.AlbumTitle = md[MediaMetadata.AlbumTitle] as string;
				row.AlbumArtist = md[MediaMetadata.AlbumArtist] as string;
				row.Publisher = md[MediaMetadata.Publisher] as string;
				row.Genre = md[MediaMetadata.Genre] as string;
				
				object o = md[MediaMetadata.TrackNumber];
				if (o == null)
					row.SetTrackNumberNull();
				else
					row.TrackNumber = (uint) o;

				o = md[MediaMetadata.Duration];
				if (o == null) 
					row.SetDurationNull();
				else
				{
					row.Duration = new TimeSpan((long)(ulong) o);
				}

				o = md[MediaMetadata.BitRate];
				if (o == null) 
					row.SetBitRateNull();
				else
					row.BitRate = (uint) o;

				o = md[MediaMetadata.IsProtected];
				if (o == null) 
					row.SetIsProtectedNull();
				else
					row.IsProtected = (bool) o;

				o = md[MediaMetadata.FileSize];
				if (o == null) 
					row.SetFileSizeNull();
				else
					row.FileSize = (ulong) o;

				row.FileName = filename;
			}
		}


		/// <summary>
		/// Retrieves media information for a single directory as a DataSet.
		/// </summary>
		/// <param name="directory">Directory to be used for data retrieval</param>
		/// <returns>MediaData object (a strongly-typed DataSet)</returns>
		public MediaData RetrieveSingleDirectoryInfo(string directory)
		{
			MediaData md = new MediaData();

			DirectoryInfo dirInfo = new DirectoryInfo(directory);
			FileInfo[] fileInfos = dirInfo.GetFiles("*.wma");
			
			for (int i=0; i < fileInfos.Length; i++)
			{
				MediaData.TrackRow row = md.Track.NewTrackRow();
				RetrieveTrackRow(dirInfo + "\\" + fileInfos[i].Name, ref row);
				md.Track.AddTrackRow(row);

				NotifyEventSubscribers(row);
			}

			return md;
		}

		/// <summary>
		/// Recursively trawls through a directory structure for media files,
		/// using them to build a DataSet of media metadata.
		/// </summary>
		/// <param name="directory">Starting point for the recursive search</param>
		/// <returns>MediaData object (a strongly-typed DataSet)</returns>
		public MediaData RetrieveRecursiveDirectoryInfo(string directory)
		{
			MediaData md = new MediaData();

			RecurseDirectories(directory, md);

			return md;
		}

		/// <summary>
		/// Internal method used to fire the TrackAdded event.
		/// </summary>
		/// <param name="row">Row that's just been added</param>
		private void NotifyEventSubscribers(MediaData.TrackRow row)
		{
			// notify any event subscribers
			if (TrackAdded != null)
			{
				TrackAdded(this, new TrackInfoEventArgs(
					(row.IsTitleNull()  ? "" : row.Title), 
					(row.IsAuthorNull() ? "" : row.Author)));
			}
		}

		/// <summary>
		/// Recursive function used by RetrieveRecursiveDirectoryInfo to drill down
		/// the directory tree.
		/// </summary>
		/// <param name="directory">Current directory level</param>
		/// <param name="md">MediaData structure in current form</param>
		private void RecurseDirectories(string directory, MediaData md)
		{
			DirectoryInfo parent = new DirectoryInfo(directory);

			DirectoryInfo[] children = parent.GetDirectories();
			foreach (DirectoryInfo folder in children)
			{
				FileInfo[] fileInfos = folder.GetFiles("*.wma");
			
				for (int i=0; i < fileInfos.Length; i++)
				{
					MediaData.TrackRow row = md.Track.NewTrackRow();
					RetrieveTrackRow(folder.FullName + "\\" + fileInfos[i].Name, ref row);
					md.Track.AddTrackRow(row);

					NotifyEventSubscribers(row);
				}

				RecurseDirectories(folder.FullName, md);
			}
		}
	}
}
