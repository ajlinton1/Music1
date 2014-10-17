using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Azure;
using Music;

namespace MusicManager
{
    public class Model
    {
        public void Reindex(int threadsNum, string[] musicFolders, IMusicDao musicDao)
        {
            var jobLogRepository = new JobLogRepository();
            var lastIndexRun = jobLogRepository.GetLatest();
            int songsInserted = 0;

            var allSongFiles = musicDao.GetSongFiles(musicFolders[0]);
            var musicFiles = from musicFile in allSongFiles.AsParallel()
                             where musicFile.IsMusicFile()
                             select musicFile;

/*            musicFiles = from musicFile in musicFiles.AsParallel()
                         where musicFile.LOCATION.IndexOf("Xbox Music") == -1
                         select musicFile;*/

/*            musicFiles = from musicFile in musicFiles.AsParallel()
                         where musicFile.LOCATION.IndexOf("KEXP") == -1
                         select musicFile; */

            musicFiles = from musicFile in musicFiles.AsParallel()
                         select musicFile.NormalizeFilename();

            var musicFilesModifiedDate = from musicFile in musicFiles.AsParallel()
                                         select new
                                         {
                                             song = musicFile,
                                             modificationDate = musicFile.GetModificationDate()
                                         };

            var modifiedFiles = from musicFile in musicFilesModifiedDate.AsParallel()
                                where (musicFile.modificationDate > lastIndexRun.DateRun)
                                select musicFile.song;

            modifiedFiles = from musicFile in modifiedFiles.AsParallel()
                            select musicFile.Populate();

            var incompleteFiles = from musicFile in modifiedFiles.AsParallel()
                                  where !musicFile.TagsComplete()
                                  select musicFile;

            foreach (var file in incompleteFiles)
            {
				Debug.WriteLine("Incomplete tags: {0} ", file.LOCATION);
            }  

            modifiedFiles = from musicFile in modifiedFiles.AsParallel()
                            where musicFile.TagsComplete()
                            select musicFile;

            var songsWithDbInfo = from song in modifiedFiles.AsParallel()
                                  select new
                                  {
                                      song = song,
                                      existsInDb = song.InDb(musicDao)
                                  };

            var updatedFiles = from song in songsWithDbInfo.AsParallel()
                               where song.existsInDb
                               select song;

            var newFiles = from song in songsWithDbInfo.AsParallel()
                           where !song.existsInDb
                           select song;

            var insertedSongs = from song in newFiles
                                select musicDao.InsertSong(song.song);

            songsInserted = insertedSongs.ToArray().Length;

            foreach (var song in updatedFiles)
            {
                Debug.WriteLine("Updating: {0}", song.song.LOCATION);
                musicDao.UpdateSong(song.song.TITLE, song.song.ARTIST, song.song.ALBUM, song.song.GENRE, 0, 0, 0, song.song.LOCATION);
            }

            jobLogRepository.Add(songsInserted.ToString());

            musicDao.UpdateArtists();
        }
        public void AddNew(string musicFolder, IMusicDao musicDao)
		{
			int songsInserted = 0;


			var allSongFiles = musicDao.GetSongFiles(musicFolder);
			var musicFiles = from musicFile in allSongFiles
							 where musicFile.IsMusicFile()
							 //                             where musicFile.IsMp3File()
							 select musicFile;

			musicFiles = from musicFile in musicFiles
						 select musicFile.NormalizeFilename();

			musicFiles = from musicFile in musicFiles
							select musicFile.Populate();

			musicFiles = from musicFile in musicFiles
							where musicFile.TagsComplete()
							select musicFile;

			var songsWithDbInfo = from song in musicFiles
								  select new
								  {
									  song = song,
									  existsInDb = song.InDb(musicDao)
								  };

			var newFiles = from song in songsWithDbInfo
						   where !song.existsInDb
						   select song;

			var insertedSongs = from song in newFiles
								select musicDao.InsertSong(song.song);

			songsInserted = insertedSongs.ToArray().Count();

		}
    }
}
