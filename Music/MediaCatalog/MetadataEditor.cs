using System;
using System.Collections;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace Microsoft.Samples.MediaCatalog
{
	public class MetadataEditor : IDisposable, IEnumerable
	{
		private IWMHeaderInfo3 header;
		bool isDisposed = false;
		ushort attrCount;

		// Constructor
		public MetadataEditor(string filename)
		{
			IWMMetadataEditor2 metadataEditor = null;

			try
			{
				WindowsMediaWrapper.CreateEditor(out metadataEditor);
				metadataEditor.Open(filename);

				header = (IWMHeaderInfo3) metadataEditor;
				header.GetAttributeCount(0xFFFF, out attrCount);

			}
			catch (COMException cex)
			{
				if ((ComError)cex.ErrorCode == ComError.NS_E_FILE_OPEN_FAILED)
					throw new FileNotFoundException
						("Failed to open the file into memory. The file may " + 
						" be missing or you may not have permission to open it.", 
						filename);
				else
					throw cex;
			}
		}

		#region IDisposable Members

		protected virtual void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				if (disposing)
				{
					// No managed resources to clear up...
				}

                try
                {
                    // Clear up unmanaged resources
                    ((IWMMetadataEditor2)header).Close();
                }
                catch (Exception)
                {
                }
			}

			isDisposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~MetadataEditor()
		{	
			Dispose(false);
		}

		#endregion

		#region IEnumerator Nested Class
		public class MetadataEnumerator : IEnumerator
		{
			private int pos = -1;
			private MetadataEditor ed;

			public MetadataEnumerator(MetadataEditor editor)
			{
				ed = editor;
			}

			public void Reset()
			{
				pos = -1;
			}

			public object Current
			{
				get
				{
					return ed[pos];
				}
			}

			public bool MoveNext()
			{
				pos++;
				return (pos < ed.Count);
			}

		}
		#endregion

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			return new MetadataEnumerator(this);
		}

		#endregion

		#region Indexers
		
		public Attribute this[int index]
		{
			get 
			{
				// Check indexer
				if (index < 0 || index >= attrCount)
				{
					throw new IndexOutOfRangeException();
				}

				ushort globalStream = 0xFFFF;
				StringBuilder attrName = null;
				ushort attrNameLen = 0;
				WMT_ATTR_DATATYPE attrDataType;
				byte[] attrValue = null;
				ushort attrValueLen = 0;

				header.GetAttributeByIndex(
					(ushort)index, 
					ref globalStream, 
					attrName,
					ref attrNameLen,
					out attrDataType,
					attrValue,
					ref attrValueLen);

				attrName = new StringBuilder(attrNameLen);
				attrValue = new byte[attrValueLen];

				header.GetAttributeByIndex(
					(ushort)index, 
					ref globalStream, 
					attrName,
					ref attrNameLen,
					out attrDataType,
					attrValue,
					ref attrValueLen);

				Attribute a = new Attribute(
					attrName.ToString(), 
					UnpackByteArray(attrValue, attrDataType));

				return a;
			}
		}


		public object this[MediaMetadata mediaMetadata]
		{
			get
			{
				switch(mediaMetadata)
				{
					case MediaMetadata.AlbumArtist:
						return GetAttributeByName("WM/AlbumArtist");
					case MediaMetadata.AlbumSortOrder:
						return GetAttributeByName("WM/AlbumSortOrder");
					case MediaMetadata.AlbumTitle:
						return GetAttributeByName("WM/AlbumTitle");
					case MediaMetadata.AudioFileUrl:
						return GetAttributeByName("WM/AudioFileURL");
					case MediaMetadata.Author:
						return GetAttributeByName("Author");
					case MediaMetadata.BeatsPerMinute:
						return GetAttributeByName("WM/BeatsPerMinute");
					case MediaMetadata.BitRate:
						return GetAttributeByName("Bitrate");
					case MediaMetadata.ContentDistributor:
						return GetAttributeByName("WM/ContentDistributor");
					case MediaMetadata.Copyright:
						return GetAttributeByName("Copyright");
					case MediaMetadata.CopyrightUrl:
						return GetAttributeByName("CopyrightURL");
					case MediaMetadata.Description:
						return GetAttributeByName("Description");
					case MediaMetadata.Duration:
						return GetAttributeByName("Duration");
					case MediaMetadata.FileSize:
						return GetAttributeByName("FileSize");
					case MediaMetadata.Genre:
						return GetAttributeByName("WM/Genre");
					case MediaMetadata.IsProtected:
						return GetAttributeByName("Is_Protected");
					case MediaMetadata.Provider:
						return GetAttributeByName("WM/Provider");
					case MediaMetadata.Publisher:
						return GetAttributeByName("WM/Publisher");
					case MediaMetadata.Title:
						return GetAttributeByName("Title");
					case MediaMetadata.TrackNumber:
						return GetAttributeByName("WM/TrackNumber");
					default:
						return null;
				}
			}
		}
		#endregion

		public int Count
		{
			get
			{
				return attrCount;
			}
		}

		public object GetAttributeByName(string AttributeName)
		{
			WMT_ATTR_DATATYPE attrDataType;
			byte[] attrValue = null;
			ushort attrValueLen = 0;
			ushort globalStream = 0xFFFF;

			// We call this API twice: the first time to retrieve the length 
			// of attrValue; the second time to populate it with a value
			// having newed it up to the appropriate size.
			try
			{
				header.GetAttributeByName(ref globalStream,
					AttributeName,
					out attrDataType,
					attrValue,
					ref attrValueLen);
			}
			catch (COMException cex)
			{
				if ((ComError)cex.ErrorCode == ComError.ASF_E_NOTFOUND)
				{
					// If we can't find the attribute, we'll simply return a null
					return null;
				}
				else throw cex;
			}

			attrValue = new byte[attrValueLen];
				
			header.GetAttributeByName(ref globalStream,
				AttributeName,
				out attrDataType,
				attrValue,
				ref attrValueLen);

			return UnpackByteArray(attrValue, attrDataType);
				
		}

		private object UnpackByteArray(byte[] attrValue, WMT_ATTR_DATATYPE attrType)
		{
			// Now we convert the resultant data into something
			// that can be returned, using the attributeDataType
			// value to determine its underlying type.
			switch (attrType)
			{
				case WMT_ATTR_DATATYPE.WMT_TYPE_STRING:
                
					if (attrValue.Length < 2)
					{
						return null;
					}
					else
					{
						StringBuilder sb = new StringBuilder(attrValue.Length / 2);

						for (int i = 0; i < attrValue.Length - 2; i += 2)
						{
							sb.Append(BitConverter.ToChar(attrValue, i));
						}
						return sb.ToString();
					}

				case WMT_ATTR_DATATYPE.WMT_TYPE_BINARY:
					return attrValue;

				case WMT_ATTR_DATATYPE.WMT_TYPE_BOOL:
					return BitConverter.ToBoolean( attrValue, 0 );

					// DWORD
				case WMT_ATTR_DATATYPE.WMT_TYPE_DWORD:    
					return BitConverter.ToUInt32( attrValue, 0 );

					// QWORD
				case WMT_ATTR_DATATYPE.WMT_TYPE_QWORD:
					return BitConverter.ToUInt64( attrValue, 0 );

					// WORD
				case WMT_ATTR_DATATYPE.WMT_TYPE_WORD:
					return BitConverter.ToUInt16( attrValue, 0 );

					// GUID
				case WMT_ATTR_DATATYPE.WMT_TYPE_GUID:
					return BitConverter.ToString( attrValue, 0, attrValue.Length );

				default:
					return null;
			}
		}
	}
}
