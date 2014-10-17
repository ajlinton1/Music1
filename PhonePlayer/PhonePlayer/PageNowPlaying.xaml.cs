using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;

namespace PhonePlayer
{
    public partial class PageNowPlaying : PhoneApplicationPage
    {
        Song currentSong = null;
        GameTimer frameworkDispatchTimer;
        List<Song> songList = new List<Song>();
        int currentSongIndex = 0;
        SongCollection songCollection = null;

        public PageNowPlaying()
        {
            InitializeComponent();

            frameworkDispatchTimer = new GameTimer();
            frameworkDispatchTimer.FrameAction += frameworkDispatchTimer_FrameAction;
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
            MediaPlayer.ActiveSongChanged += MediaPlayer_ActiveSongChanged;
        }

        void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            if (songList.Count == 0)
            {
                return;
            }
            currentSongIndex++;
            if (currentSongIndex == songList.Count)
            {
                currentSongIndex = 0;
            }
            MediaPlayer.Play(songList[currentSongIndex]);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string select = null;
            string genre = null;
            if (this.NavigationContext.QueryString.ContainsKey("select"))
            {
                select = this.NavigationContext.QueryString["select"];
            }
            if (this.NavigationContext.QueryString.ContainsKey("genre"))
            {
                genre = this.NavigationContext.QueryString["genre"];
            }

            using (var library = new MediaLibrary())
            {
                var songResults = from song in library.Songs
                          where (song.Genre.ToString().ToLower().IndexOf(genre) == -1)
                          select song;


                var playlists = library.Playlists;
                var playlist = library.
                var songResults = library.Songs.Where(s => s.Genre.ToString().ToLower().IndexOf(genre) > -1);
                foreach (var song in songResults)
                {
                    songList.Add(song);
                }

                songCollection = library.Songs;
            }

            frameworkDispatchTimer.Start();
            FrameworkDispatcher.Update();
        }

        void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            if (songList.Count == 0)
            {
                return;
            }
            currentSongIndex++;
            if (currentSongIndex == songList.Count)
            {
                currentSongIndex = 0;
            }
            MediaPlayer.Play(songList[currentSongIndex]);
        }

        void frameworkDispatchTimer_FrameAction(object sender, EventArgs e)
        {
            if (songList.Count==0)
            {
                frameworkDispatchTimer.Stop();
                return;
            }
            if (MediaPlayer.State == MediaState.Playing)
            {
                return;
            }
            MediaPlayer.Play(songList[currentSongIndex]);
            currentSongIndex++;
            if (currentSongIndex == songList.Count)
            {
                currentSongIndex = 0;
            }
        }

    }
}