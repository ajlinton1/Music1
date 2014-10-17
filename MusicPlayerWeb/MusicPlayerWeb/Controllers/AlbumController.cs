using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Controllers
{
    public class AlbumController : ApiController
    {
        IMusicRepository musicRepository;

        public AlbumController()
        {
            musicRepository = new MusicRepository();
        }

        public IList<string> Get(int skip)
        {
            return musicRepository.GetAlbums(skip);
        }

    }
}
