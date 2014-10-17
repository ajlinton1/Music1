using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicPlayerWeb.Models
{
    public interface IMusicRepository
    {
        IList<SONG> GetSongs(string genre, string sequence, int skip, string artist, string location);

        void Update(SONG song);

//        IList<string> GetGenres();

        IList<string> GetAlbums(int skip);

//        IList<string> GetArtists(int skip);

        IList<string> GetSegments(int skip);
    }
}