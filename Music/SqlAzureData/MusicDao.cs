using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Data.Objects;
using System.Data.Common;

namespace SqlAzureData
{
    public class MusicDao
    {
        public static SONG[] GetMellowSongs(int num)
        {
            List<SONG> ret = new List<SONG>();
            using (var context = new MusicEntities())
            {
                var qStr = "SELECT TOP (@num) song_rated, NEWGUID() AS guid, song_rated.song " +
                "FROM song_rated " +
                "JOIN ARTIST as artist " +
                "ON artist.NAME = song_rated.song.artist " +
                "WHERE song_rated.song.GENRE IN {'Acid House'," +
            "'Acid Jazz'," +
            "'Ambient'," +
            "'Ambient Breakbeat'," +
            "'Ambient Downtempo Abstract'," +
            "'Ambient Downtempo'," +
            "'Ambient Dub'," +
            "'Ambient House'," +
            "'Ambient Space'," +
            "'Ambient Techno'," +
            "'Ambient / Chillout'," +
            "'Beats & Breaks'," +
            "'Beats and Breaks'," +
            "'Big Beat'," +
            "'Boom Bap'," +
            "'Breaks & Beats'," +
            "'Breaks and Beats'," +
            "'Chill Out'," +
            "'Chill-Out'," +
            "'Chillout'," +
            "'Cut & Paste'," +
            "'Dance'," +
            "'Deep House'," +
            "'Down Tempo'," +
            "'Downbeat'," +
            "'Downtemp'," +
            "'Downtempo'," +
            "'drum ''n'' bass'," +
            "'Dub'," +
            "'Dubstep'," +
            "'Electro-Funk'," +
            "'Electroclash'," +
            "'Electronic'," +
            "'Electronica'," +
            "'Electronica & Dance'," +
            "'ElectroPop'," +
            "'Environmental Music'," +
            "'Ethnic Fusion'," +
            "'Experimental'," +
            "'Experimental Hip-Hop'," +
            "'Experimental Rap'," +
            "'Film Score'," +
            "'Freestyle'," +
            "'Funky House'," +
            "'General Electronic'," +
            "'General Techno'," +
            "'Global House'," +
            "'Grime'," +
            "'Hardcore Electronic'," +
            "'House'," +
            "'Illbient'," +
            "'Instrumental Hip-Hop'," +
            "'Intelligent'," +
            "'Jungle'," +
            "'Laptronic'," +
            "'Laptronica'," +
            "'Leftfield'," +
            "'Leftfield/IDM'," +
            "'Minimal'," +
            "'Minimalism'," +
            "'New Age Electronic'," +
            "'Plunderphonic'," +
            "'Psytrance'," +
            "'Rave'," +
            "'Remix'," +
            "'Techno'," +
            "'Techno Tribal'," +
            "'Trance'," +
            "'Trip Hop'," +
            "'Trip-Hop'," +
            "'Turntabalism'," +
            "'Turntablism'," +
            "'World Beats'," +
            "'World Dance'," +
            "'World Reggae'," +
            "'Worldbeat'} " +

            "AND (artist.Radio is null)";

                qStr = qStr + " ORDER BY guid";

                var results = context.CreateQuery<DbDataRecord>(qStr);
                results.Parameters.Add(new ObjectParameter("num", num));

                foreach (var currentResult in results)
                {
                    SONG_RATED songRated = (SONG_RATED)currentResult[0];
                    SONG song = songRated.SONG;
                    ret.Add(song);
                }
            }

            return ret.ToArray();
        }

        public static SONG[] GetSongsByGenre(int num, string genre)
        {
            List<SONG> ret = new List<SONG>();
            using (var context = new MusicEntities())
            {
                var sql = "SELECT song_rated, NEWGUID() AS guid, song_rated.song " +
                "FROM song_rated " +
                "JOIN ARTIST as artist " +
                "ON artist.NAME = song_rated.song.artist " +
                "WHERE song_rated.song.GENRE LIKE 'Ambient%' ";

                sql = sql + " ORDER BY guid";

                var results = context.CreateQuery<DbDataRecord>(sql);
                results.Parameters.Add(new ObjectParameter("num", num));

                foreach (var currentResult in results)
                {
                    SONG_RATED songRated = (SONG_RATED)currentResult[0];
                    SONG song = songRated.SONG;
                    ret.Add(song);
                }
            }

            return ret.ToArray();
        }

    }
}
