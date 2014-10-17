using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicPlayerWeb.Models;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void GetSongs()
        {
            var musicRepository = new MusicRepository();
            var songs = musicRepository.GetSongs(null, null, 0, null, null);
            foreach (var s in songs)
            {
                testContextInstance.WriteLine("{0}, {1}, {2}", s.TITLE, s.LOCATION, s.ID);
            }
        }

        [TestMethod]
        public void GetSongsByGenre()
        {
            var musicRepository = new MusicRepository();
            var songs = musicRepository.GetSongs("Electronic", null, 0, null, null);
            foreach (var s in songs)
            {
                testContextInstance.WriteLine("{0} {1} {2}", s.GENRE, s.ID, s.LOCATION);
            }
        }

        [TestMethod]
        public void GetSongsByGenreAndSequence()
        {
            var musicRepository = new MusicRepository();
            var songs = musicRepository.GetSongs("Electronic", "Newest", 0, null, null);
            foreach (var s in songs)
            {
                testContextInstance.WriteLine("{0} {1} {2}", s.GENRE, s.ID, s.LOCATION);
            }
        }

        [TestMethod]
        public void GetOldestSongs()
        {
            var musicRepository = new MusicRepository();
            var songs = musicRepository.GetSongs(null, "Oldest", 0, null, null);
            foreach (var s in songs)
            {
                testContextInstance.WriteLine("{0} {1} {2}", s.GENRE, s.ID, s.LOCATION);
            }
        }

        [TestMethod]
        public void GetRandomSongs()
        {
            var musicRepository = new MusicRepository();
            var songs = musicRepository.GetSongs(null, "Random", 0, null, null);
            foreach (var s in songs)
            {
                testContextInstance.WriteLine("{0} {1} {2}", s.ID, s.LOCATION, s.TITLE);
            }
        }

        [TestMethod]
        public void GetRandomElectronic()
        {
            var musicRepository = new MusicRepository();
            var songs = musicRepository.GetSongs("Electronic", "Random", 0, null, null);
            foreach (var s in songs)
            {
                testContextInstance.WriteLine("{0} {1} {2}", s.ID, s.LOCATION, s.TITLE);
            }
        }

        [TestMethod]
        public void GetSongsByArtist()
        {
            var musicRepository = new MusicRepository();
            var songs = musicRepository.GetSongs(null, null, 0, "Blue", null);
            foreach (var s in songs)
            {
                testContextInstance.WriteLine("{0} {1} {2}", s.ID, s.LOCATION, s.TITLE);
            }
        }

        [TestMethod]
        public void GetSongsByLocation()
        {
            var musicRepository = new MusicRepository();
            var songs = musicRepository.GetSongs(null, null, 0, null, "KEXP");
            foreach (var s in songs)
            {
                testContextInstance.WriteLine("{0} {1} {2}", s.ID, s.LOCATION, s.TITLE);
            }
        }

/*        [TestMethod]
        public void GetGenres()
        {
            var musicRepository = new MusicRepository();
            musicRepository.GetGenres();
        } */

        [TestMethod]
        public void GetSongsGDrive()
        {
            var musicRepository = new MusicRepository();
            var songs = musicRepository.GetSongs(null, null, 0, null, null);
            foreach (var s in songs)
            {
                testContextInstance.WriteLine("{0} {1} {2}", s.ID, s.LOCATION, s.TITLE);
            }
        }

        [TestMethod]
        public void GetAlbums()
        {
            var musicRepository = new MusicRepository();
            var albums = musicRepository.GetAlbums(0);
            foreach (var album in albums)
            {
                testContextInstance.WriteLine("{0}", album);
            }
        }

        [TestMethod]
        public void GetAlbumsSkip()
        {
            int skip = 100;
            var musicRepository = new MusicRepository();
            var albums = musicRepository.GetAlbums(skip);
            foreach (var album in albums)
            {
                testContextInstance.WriteLine("{0}", album);
            }
        }

/*        [TestMethod]
        public void GetArtists()
        {
            int skip = 0;
            var musicRepository = new MusicRepository();
            var artists = musicRepository.GetArtists(skip);
            foreach (var artist in artists)
            {
                testContextInstance.WriteLine("{0}", artist);
            }
        } */

/*        [TestMethod]
        public void GetSegments()
        {
            int skip = 0;
            var musicRepository = new MusicRepository();
            var segments = musicRepository.GetSegments(skip);
            foreach (var segment in segments)
            {
                testContextInstance.WriteLine("{0}", segment);
            }
        } */

        [TestMethod]
        public void GetSegments()
        {
            int take = 10;
            int skip = 0;
            var segmentsService = new SegmentRepository();
            var segments = segmentsService.Get(take, skip);
            foreach (var segment in segments)
            {
                testContextInstance.WriteLine("{0} {1} {2} {3}", segment.Genre, segment.SegmentAll, segment.Segment0, segment.Segment1);
            }
        }

        [TestMethod]
        public void GetGenres()
        {
            var genreService = new GenreRepository();
            var genres = genreService.Get();
            foreach (var genre in genres)
            {
                testContextInstance.WriteLine("{0} {1}", genre.Item1, genre.Item2);
            }
        }

        [TestMethod]
        public void GetArtists()
        {
            int skip = 0;
            int take = 20;
            var artistRepository = new ArtistRepository();
            var artists = artistRepository.Get(take, skip);
            foreach (var artist in artists)
            {
                testContextInstance.WriteLine("{0} {1}", artist.Item1, artist.Item2);
            }
        }

    }
}
