using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Controllers
{
    public class SongController : ApiController
    {
        IMusicRepository musicRepository;

        public SongController()
        {
            musicRepository = new MusicRepository();
        }

        public IList<SONG> Get(string genre, string sequence, int skip, string artist, string location)
        {
            var songs = musicRepository.GetSongs(genre, sequence, skip, artist, location);
            return songs;
        }

        public void Post(int id, SONG song)
        {
            musicRepository.Update(song);
        }

/*        public void Put(SONG song)
        {
            musicRepository.Update(song);
        } */

    }
}
