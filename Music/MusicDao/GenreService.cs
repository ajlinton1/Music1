using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.andrewlinton.AzureTable;

namespace com.andrewlinton.music
{
	public class GenreService
	{
		GenreRepository genreRepository;

		public GenreService()
		{
			genreRepository = new GenreRepository();
		}

		public void SaveGenres(Dictionary<string,int> genres)
		{
			foreach (KeyValuePair<string, int> kvp in genres)
			{
				string genre = kvp.Key;
				int count = kvp.Value;
				genreRepository.SetGenre(genre, count.ToString());
			}
		}

		public void Populate()
		{
			genreRepository.DeleteAll();

			var songRepository = new SongRepository(new Repository("log"));
			Dictionary<string, int> genres = songRepository.ProcessGenres(SaveGenres);
		}
	}
}
