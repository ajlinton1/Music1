using System;
using System.IO;
using System.Diagnostics;
using Music;
using System.Text.RegularExpressions;
using IdSharp.Tagging.ID3v2;
using IdSharp.Tagging.ID3v1;
using com.andrewlinton.music;

namespace MusicManager
{
    public static class SongExt
    {
        public static bool IsMusicFile(this SONG song)
        {
            return Regex.IsMatch(song.LOCATION, ".mp3|.wma", RegexOptions.IgnoreCase);
        }

        public static bool IsMp3File(this SONG song)
        {
            return Regex.IsMatch(song.LOCATION, ".mp3", RegexOptions.IgnoreCase);
        }

        public static bool HasIdInFilename(this SONG song)
        {
            bool ret = false;
            var re = new Regex(@"\\s\d*s");
            var matches = re.Matches(song.LOCATION);
            if (matches.Count > 0)
            {
                ret = true;
                string matchValue = matches[0].Value;
            }

            return ret;
        }

        public static int GetFilenameId(this SONG song)
        {
            int ret = -1;
            var re = new Regex(@"\\s\d*s");
            var matches = re.Matches(song.LOCATION);
            if (matches.Count > 0)
            {
                ret = Convert.ToInt32(matches[0].Value);
            }

            return ret;
        }

        public static SONG NormalizeFilename(this SONG song)
        {
            try
            {
            int commaLocation = song.LOCATION.IndexOf(",");
            if (commaLocation > -1)
            {
                // Separate filename from folder name
                FileInfo fileInfo = new FileInfo(song.LOCATION);
                string directoryName = fileInfo.DirectoryName;
                string newFilename = fileInfo.Name;

                // Remove commas from file name
                do
                {
                    commaLocation = newFilename.IndexOf(",");
                    if (commaLocation > -1)
                    {
                        newFilename = newFilename.Remove(commaLocation, 1);
                    }
                }
                while (commaLocation > -1);

                // Join new filename with folder name
                newFilename = directoryName + "\\" + newFilename;

                // Move old file to new file
                if (!File.Exists(newFilename))
                {
                    File.Move(song.LOCATION, newFilename);
                    File.Delete(song.LOCATION);
                }

                return song;
            }

            // Clean up filename
            string rName = "Rhapsody  Subscription Music Service Listen All You Want Millions of Songs";
            if (Regex.IsMatch(song.LOCATION, rName))
            {
                // Separate filename from folder name
                FileInfo fileInfo = new FileInfo(song.LOCATION);
                string directoryName = fileInfo.DirectoryName;
                string newFilename = fileInfo.Name;

                newFilename = newFilename.Substring(rName.Length);
                newFilename = "Track" + newFilename;
                // Join new filename with folder name
                newFilename = directoryName + "\\" + newFilename;

                // Move old file to new file
                if (!File.Exists(newFilename))
                {
                    File.Move(song.LOCATION, newFilename);
                    File.Delete(song.LOCATION);
                }
                    song.LOCATION = newFilename;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error: {0} : {1}", song.LOCATION, ex.Message));
            }
            return song;
        }

        public static SONG AddIdToFilename(this SONG song)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(song.LOCATION);
                song.FILESIZE = fileInfo.Length;
                string newFilename = string.Format("{0}\\s{1}s {2}", fileInfo.DirectoryName, song.ID, fileInfo.Name);
                fileInfo.CopyTo(newFilename, true);
                File.Delete(song.LOCATION);
                song.LOCATION = newFilename;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Error: {0}", ex.Message));
            }
            return song;
        }
                
        public static SONG Populate(this SONG song)
        {
            string title, artist, album, genre;

            title = null;
            artist = null;
            album = null;
            genre = null;
			string trackNumber = null;

            try
            {

				try
				{
					IID3v2 iD3v2 = ID3v2Helper.CreateID3v2(song.LOCATION);
					artist = iD3v2.Artist;
					title = iD3v2.Title;
					album = iD3v2.Album;
					genre = iD3v2.Genre;
					trackNumber = iD3v2.TrackNumber;
					if ((!string.IsNullOrWhiteSpace(genre)) && (genre.Contains("(")))
					{
						genre = genre.Replace('(', ' ');
						genre = genre.Replace(')', ' ');
						genre = genre.Trim();
						int genreIndex = Convert.ToInt16(genre);
						genre = GenreHelper.GenreByIndex[genreIndex];
					}
				}
				catch (Exception)
				{
					Debug.WriteLine(String.Format("Warning: Unable to extract ID3v2 tags from {0}", song.LOCATION));
				}

				try
				{
					IID3v1 iD3v1 = ID3v1Helper.CreateID3v1(song.LOCATION);
					if ((string.IsNullOrWhiteSpace(genre)) || (genre.Contains("(")))
					{
						genre = GenreHelper.GenreByIndex[iD3v1.GenreIndex];
					}
					if (string.IsNullOrWhiteSpace(artist))
					{
						artist = iD3v1.Artist;
					}
					if (string.IsNullOrWhiteSpace(title))
					{
						title = iD3v1.Title;
					}
					if (string.IsNullOrWhiteSpace(album))
					{
						album = iD3v1.Album;
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine(string.Format("Warning: Unable to extract ID3v1 tags from {0} {1}", song.LOCATION, ex.Message));
				}

				if (String.IsNullOrWhiteSpace(title))
				{
					String[] s = song.LOCATION.Split(new char[] { '\\', '.' });
					title = s[s.Length - 2];
				}

				if (string.IsNullOrEmpty(artist))
				{
					int loc = title.IndexOf("by");
					if ((loc>-1) && (title.Length>3))
					{
						artist = title.Substring(loc + 3);
						if (loc > 1)
						{
							title = title.Substring(0, loc - 1);
						}

						loc = artist.IndexOf("from");
							if ((loc > -1) && (artist.Length > 5))
						{
							album = artist.Substring(loc + 5);
							artist = artist.Substring(0, loc - 1);
						}
					}
				}

				song.TITLE = title;
				song.ARTIST = artist;
				song.GENRE = genre;
				song.ALBUM = album;
				song.TrackNumber = trackNumber;

				song.RATING = 1;

				try
				{
					song.HASH = MusicDao.GenerateHashCode(song.LOCATION);
				}
				catch (Exception)
				{
					Debug.WriteLine(String.Format("Warning: Unable to generate hash for {0}", song.LOCATION));
				}

				try
				{
					FileInfo fileInfo = new FileInfo(song.LOCATION);
					song.FILESIZE = fileInfo.Length;
				}
				catch (Exception)
				{
					Debug.WriteLine(String.Format("Warning: Unable to get file length {0}", song.LOCATION));
				}
			}
			catch (Exception e1)
			{
				Debug.WriteLine(e1.Message);
			}

            return song;
        }

        public static SONG CalculateModificationDate(this SONG song)
        {
            var fileInfo = new FileInfo(song.LOCATION);
            song.UPDATED = fileInfo.LastWriteTime;

            return song;
        }

        public static DateTime GetModificationDate(this SONG song)
        {
            var fileInfo = new FileInfo(song.LOCATION);
            return fileInfo.LastWriteTime;
        }

        public static SONG CalculateHash(this SONG song)
        {
            song.HASH = MusicDao.GenerateHashCode(song.LOCATION);
            return song;
        }
        public static bool HashExists(this SONG song, IMusicDao musicDao)
        {
            return musicDao.HashExists(song);
        }
        public static bool InDb(this SONG song, IMusicDao musicDao)
        {
            return musicDao.InDb(song);
        }
        public static bool TagsComplete(this SONG song)
        {
            bool ret = true;

			try
			{

            if (string.IsNullOrEmpty(song.ALBUM))
            {
                ret = false;
            }
            if (string.IsNullOrEmpty(song.ARTIST))
            {
                ret = false;
            }
            if (string.IsNullOrEmpty(song.GENRE))
            {
                ret = false;
            }
            if (string.IsNullOrEmpty(song.LOCATION))
            {
                ret = false;
            }
            if (string.IsNullOrEmpty(song.TITLE))
            {
                ret = false;
            }
			}
			catch (Exception ex)
			{
				Debug.Write(ex.Message);
			}
            return ret;
        }

    }
}
