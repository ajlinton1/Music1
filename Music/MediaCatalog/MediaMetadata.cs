using System;

namespace Microsoft.Samples.MediaCatalog
{
	public enum MediaMetadata
	{
		// A selected list of media attributes. There are many more that aren't
		// included in this list; any additions here need to be added also to 
		// the MetadataEditor indexer this[MediaMetadata].

		// Tags in comments below come from the following URL:
		// http://msdn.com/library/en-us/wmform/htm/attributelist.asp

		AlbumArtist,		// WM/AlbumArtist
		AlbumSortOrder,		// WM/AlbumSortOrder
		AlbumTitle,			// WM/AlbumTitle
		AudioFileUrl,		// WM/AudioFileURL
		Author,				// Author
		BeatsPerMinute,		// WM/BeatsPerMinute
		BitRate,			// Bitrate
		ContentDistributor,	// WM/ContentDistributor
		Copyright,			// Copyright
		CopyrightUrl,		// CopyrightURL
		Description,		// Description
		Duration,			// Duration
		FileSize,			// FileSize
		Genre,				// WM/Genre
		IsProtected,		// Is_Protected
		Provider,			// WM/Provider
		Publisher,			// WM/Publisher
		Title,				// Title
		TrackNumber			// WM/TrackNumber
	}
}
