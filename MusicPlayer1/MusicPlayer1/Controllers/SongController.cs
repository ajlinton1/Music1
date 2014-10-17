using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using com.andrewlinton.AzureTable;

namespace MusicPlayer1.Controllers
{
    public class SongController : ApiController
    {
		MusicRepository musicRepository;

		public SongController()
		{
			musicRepository = new MusicRepository();
		}

		public IList<Song> Get(string skip, string artist, string genre)
		{
			int skipInt = 0;
			if (skip!=null)
			{
				skipInt = Convert.ToInt32(skip);
			}
			var songs = musicRepository.GetSongs(skipInt, artist, genre);
			return songs;
		}

    }
}
