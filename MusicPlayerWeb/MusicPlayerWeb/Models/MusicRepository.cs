using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicPlayerWeb.Models
{
    public class MusicRepository : IMusicRepository
    {
        int maxReturn = 100;
        List<string> genres = null;

        public MusicRepository()
        {
        }

        public IList<SONG> GetSongs(string genre, string sequence, int skip, string artist, string location)
        {
            SONG[] songs = null;
            IEnumerable<SONG> result;

            using (var musicEntities = new MusicEntities())
            {
                result = musicEntities.SONG.Where(s => (!string.IsNullOrEmpty(s.GDRIVE)));

                if (artist != null)
                {
                    result = result.Where(s => (s.ARTIST.IndexOf(artist) > -1));
                }

                if (location != null)
                {
                    result = result.Where(s => (s.LOCATION.IndexOf(location) > -1));
                }

                if (genre != null)
                {
                    genre = genre.ToLower();
                    result = result.Where(s => (s.GENRE.ToLower().IndexOf(genre) > -1));
                }

                if (!string.IsNullOrEmpty(sequence))
                {
                    if (sequence == "Newest")
                    {
                        result = result.OrderByDescending(s => s.ID);
                    }
                    else if (sequence == "Random")
                    {
                        List<SONG> ret = new List<SONG>();
                        songs = result.ToArray<SONG>();
                        var random = new Random();
                        var start = random.Next(songs.Length);
                        for (int i = start; i < songs.Length; i++)
                        {
                            ret.Add(songs[i]);
                            if (ret.Count >= maxReturn)
                            {
                                break;
                            }
                        }
                        return ret.ToArray<SONG>();
                    }
                    else
                    {
                        result = result.OrderBy(s => s.ID);
                    }

                    result = result.Skip(skip).Take(maxReturn);

                    result = from song in result
                             orderby song.LOCATION
                             select song;
                }
                else
                {
                    result = result.Skip(skip).Take(maxReturn);
                }

                songs = result.ToArray<SONG>();
            }
            return songs;
        }

        public IList<SONG> GetRandomSongs()
        {
            IEnumerable<SONG> result;
            List<SONG> ret = new List<SONG>();
            SONG[] songs = null;
            using (var musicEntities = new MusicEntities())
            {
                result = musicEntities.SONG.Where(s => (!string.IsNullOrEmpty(s.GDRIVE)));
                songs = result.ToArray<SONG>();
                var random = new Random();
                var start = random.Next(songs.Length);
                for (int i = start; i < songs.Length; i++ )
                {
                    ret.Add(songs[i]);
                    if (ret.Count > maxReturn)
                    {
                        break;
                    }
                }
                for (int i = 0; i < start; i++)
                {
                    ret.Add(songs[i]);
                    if (ret.Count > maxReturn)
                    {
                        break;
                    }
                }
                return ret.ToArray<SONG>();
            }
        }

        public void Update(SONG song)
        {
            using (var musicEntities = new MusicEntities())
            {
                var existingSong = musicEntities.SONG.Where(s => s.ID == song.ID).FirstOrDefault();
                if (existingSong == null)
                {
                    throw new ArgumentException("Song not found");
                }
                existingSong.ARTIST = song.ARTIST;
                existingSong.ALBUM = song.ALBUM;
                existingSong.GENRE = song.GENRE;
                existingSong.RATING = song.RATING;
                existingSong.TITLE = song.TITLE;
                existingSong.UPDATED = DateTime.Now;
                musicEntities.SaveChanges();
            }
        }

        public IList<string> GetAlbums(int skip)
        {
            var albums = new List<string>();
            using (var musicEntities = new MusicEntities())
            {
                var songs = from song in musicEntities.SONG
                            where !string.IsNullOrEmpty(song.GDRIVE)
                            select song;

                songs = from song in songs
                        where !string.IsNullOrEmpty(song.ALBUM)
                        select song;

                var groups = from song in songs
                             group song by song.ALBUM;

                groups = from g in groups
                         orderby g.Key
                         select g;

                groups = groups.Skip(skip).Take(maxReturn);

                foreach (IGrouping<string, SONG> grouping in groups)
                {
                    var group = grouping;
                    albums.Add(grouping.Key);
                }
            }
            return albums.ToArray<string>();
        }

/*        public IList<string> GetArtists(int skip)
        {
            using (var musicEntities = new MusicEntities())
            {
                var artists = from artist in musicEntities.ARTIST
                              orderby artist.NAME
                              select artist.NAME;

                artists = artists.Skip(skip).Take(maxReturn);

                return artists.ToArray();
            }
        } */

        public IList<string> GetSegments(int skip)
        {
            var segments = new List<string>();
            using (var musicEntities = new MusicEntities())
            {
                var songs = from song in musicEntities.SONG
                            where !string.IsNullOrEmpty(song.GDRIVE)
                            select song;

                songs = from song in songs
                        orderby song.LOCATION
                        select song;

                songs = songs.Skip(skip).Take(maxReturn);

                var songLocations = new List<string>();
                foreach (var song in songs)
                {
                    songLocations.Add(song.LOCATION);
                }

                var shortSegments = new List<string>();
                foreach (var segment in songLocations)
                {
                    int start = segment.IndexOf("\\Music\\") + 7;
                    int end = segment.LastIndexOf('\\');
                    if (end > start)
                    {
                        var shortSegment = segment.Substring(start, end - start);
                        shortSegments.Add(shortSegment);
                    }
                }

                var groups = from songSegment in shortSegments
                             group songSegment by songSegment;

                groups = from g in groups
                         orderby g.Key
                         select g;

                foreach (var grouping in groups)
                {
                    var group = grouping;
                    segments.Add(grouping.Key);
                }
            }
            return segments.ToArray<string>();
        }

    }
}