using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicPlayerWeb.Models
{
    public class MusicRepositoryMock : IMusicRepository
    {
        public IList<SONG> GetSongs(string genre, string sequence, int skip, string artist, string location)
        {
            var songList = new List<SONG>();
            var song = new SONG();
            song.TITLE = "Test title";
            song.ARTIST = "Test artist";
            song.GENRE = "Test genre";
            song.LOCATION = "Test location";
            songList.Add(song);

            return songList;
        }

        public void Update(SONG song)
        {

        }

/*        public IList<string> GetGenres()
        {
            var genres = new List<string>();
            genres.Add("Rock");
            genres.Add("Electronic");
            return genres;
        } */

        public IList<string> GetAlbums(int skip)
        {
            var albums = new List<string>();
            return albums;
        }

/*        public IList<string> GetArtists(int skip)
        {
            var artists = new List<string>();
            return artists;
        }*/

        public IList<string> GetSegments(int skip)
        {
            var segments = new List<string>();
            return segments;
        }

    }
}