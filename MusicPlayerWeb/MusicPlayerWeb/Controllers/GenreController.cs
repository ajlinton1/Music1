using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Controllers
{
    public class GenreController : ApiController
    {
        IMusicRepository musicRepository;

        public GenreController()
        {
            musicRepository = new MusicRepository();
        }

        public IList<Tuple<string,string>> Get()
        {
            var genres = new List<Tuple<string, string>>();
            var genreService = new GenreRepository();
            var genreTuples = genreService.Get();
            foreach (var genreTuple in genreTuples)
            {
                genres.Add(genreTuple);
            }
            return genres;
        }

    }
}
