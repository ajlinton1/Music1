using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer8.MusicService;

namespace MusicPlayer8
{
    public class SongView : SONG
    {
        static string musicBlobUrl = "http://voidingthevoid.blob.core.windows.net/public";
        string baseUrl = "http://localhost";

        public string Location
        {
            get;
            set;
        }

        public SongView(SONG song)
        {
            this.ALBUM = song.ALBUM;
            this.ARTIST = song.ARTIST;
            this.COMMENTS = song.COMMENTS;
            this.GENRE = song.GENRE;
            this.ID = song.ID;
            this.LOCATION = song.LOCATION;
            this.TITLE = song.TITLE;
            this.RATING = song.RATING;

            this.Location = musicBlobUrl + "/" + song.ID + ".mp3";
        }

        public SongView(MellowSongs song)
        {
            this.ALBUM = song.ALBUM;
            this.ARTIST = song.ARTIST;
            this.COMMENTS = song.COMMENTS;
            this.GENRE = song.GENRE;
            this.ID = song.ID;
            this.LOCATION = song.LOCATION;
            this.TITLE = song.TITLE;
            this.RATING = song.RATING.Value;

            int start = song.LOCATION.ToLower().IndexOf("music");
            this.Location = song.LOCATION.Substring(start + 5);
            this.Location = this.Location.Replace('\\', '/');
            this.Location = baseUrl + "/" + this.Location;
        }

        public SongView(MellowSongs1 song)
        {
            this.ALBUM = song.ALBUM;
            this.ARTIST = song.ARTIST;
            this.COMMENTS = song.COMMENTS;
            this.GENRE = song.GENRE;
            this.ID = song.ID;
            this.LOCATION = song.LOCATION;
            this.TITLE = song.TITLE;
            this.RATING = song.RATING.Value;

            int start = song.LOCATION.ToLower().IndexOf("music");
            this.Location = song.LOCATION.Substring(start + 5);
            this.Location = this.Location.Replace('\\', '/');
            this.Location = baseUrl + "/" + this.Location;
        }

        public SongView(RandomSongs song)
        {
            this.ALBUM = song.ALBUM;
            this.ARTIST = song.ARTIST;
            this.COMMENTS = song.COMMENTS;
            this.GENRE = song.GENRE;
            this.ID = song.ID;
            this.LOCATION = song.LOCATION;
            this.TITLE = song.TITLE;
            this.RATING = song.RATING;

            int start = song.LOCATION.ToLower().IndexOf("music");
            this.Location = song.LOCATION.Substring(start + 5);
            this.Location = this.Location.Replace('\\', '/');
            this.Location = baseUrl + "/" + this.Location;
        }

        public SongView(RecentSong song)
        {
            this.ALBUM = song.ALBUM;
            this.ARTIST = song.ARTIST;
            this.COMMENTS = song.COMMENTS;
            this.GENRE = song.GENRE;
            this.ID = song.ID;
            this.LOCATION = song.LOCATION;
            this.TITLE = song.TITLE;
            this.RATING = song.RATING;

            int start = song.LOCATION.ToLower().IndexOf("music");
            this.Location = song.LOCATION.Substring(start + 5);
            this.Location = this.Location.Replace('\\', '/');
            this.Location = baseUrl + "/" + this.Location;
        }

        public SongView(SongsV song)
        {
            this.ALBUM = song.ALBUM;
            this.ARTIST = song.ARTIST;
            this.COMMENTS = song.COMMENTS;
            this.GENRE = song.GENRE;
            this.ID = song.ID;
            this.LOCATION = song.LOCATION;
            this.TITLE = song.TITLE;
            this.RATING = song.RATING.Value;

            int start = song.LOCATION.ToLower().IndexOf("music");
            this.Location = song.LOCATION.Substring(start + 5);
            this.Location = this.Location.Replace('\\', '/');
            this.Location = baseUrl + "/" + this.Location;
        }

        public SONG ConvertToSong()
        {
            SONG song = new SONG();
            song.ALBUM = this.ALBUM;
            song.ARTIST = this.ARTIST;
            song.COMMENTS = this.COMMENTS;
            song.GENRE = this.GENRE;
            song.ID = this.ID;
            song.LOCATION = this.LOCATION;
            song.TITLE = this.TITLE;
            song.RATING = this.RATING;
            return song;
        }

        public String Title
        {
            get
            {
                if (String.IsNullOrEmpty(TITLE))
                {
                    return "?";
                }
                else if (TITLE.Length < 30)
                {
                    return TITLE;
                }
                else
                {
                    return TITLE.Substring(0, 30);
                }
            }
        }

        public String Artist
        {
            get
            {
                if (String.IsNullOrEmpty(ARTIST))
                {
                    return "?";
                }
                else if (ARTIST.Length < 30)
                {
                    return ARTIST;
                }
                else
                {
                    return ARTIST.Substring(0, 30);
                }
            }
        }

        public String Album
        {
            get
            {
                if (String.IsNullOrEmpty(ALBUM))
                {
                    return "?";
                }
                else if (ALBUM.Length < 30)
                {
                    return ALBUM;
                }
                else
                {
                    return ALBUM.Substring(0, 30);
                }
            }
        }

        public String Genre
        {
            get
            {
                if (String.IsNullOrEmpty(GENRE))
                {
                    return "?";
                }
                else if (GENRE.Length < 30)
                {
                    return GENRE;
                }
                else
                {
                    return GENRE.Substring(0, 30);
                }
            }
        }

        public int Rating
        {
            get
            {
                return RATING;
            }
        }

    }
}
