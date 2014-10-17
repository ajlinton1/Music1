using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Controllers
{
    public class ArtistController : ApiController
    {
        IMusicRepository musicRepository;

        public ArtistController()
        {
            musicRepository = new MusicRepository();
        }

        public List<Tuple<string, string>> Get(int skip)
        {
            int maxReturn = 100;
            var artistRepository = new ArtistRepository();
            return artistRepository.Get(maxReturn, skip);
        }

    }
}
