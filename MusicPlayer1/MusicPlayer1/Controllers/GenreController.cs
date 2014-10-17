using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using com.andrewlinton.AzureTable;

namespace MusicPlayer1.Controllers
{
    public class GenreController : ApiController
    {
		MusicRepository musicRepository;

		public GenreController()
		{
			musicRepository = new MusicRepository();
		}

		public IList<Genre> Get()
		{
			return musicRepository.GetGenres();
		}

    }
}
