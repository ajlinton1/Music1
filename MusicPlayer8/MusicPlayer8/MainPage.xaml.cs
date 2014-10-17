using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Data.Services.Client;
using MusicPlayer8.MusicService;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicPlayer8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static string musicServiceUrl = "http://vvoidmusic.cloudapp.net/MusicService.svc";
        static MusicEntities musicEntities = new MusicEntities(new Uri(musicServiceUrl));
        public static DataServiceCollection<SONG> Songs = new DataServiceCollection<SONG>(musicEntities);
        public static DataServiceCollection<MellowSongs> MellowSongs = new DataServiceCollection<MellowSongs>(musicEntities);
        public static DataServiceCollection<RandomSongs> RandomSongs = new DataServiceCollection<RandomSongs>(musicEntities);
        int songIndex = 0;
        public CurrentSong currentSong = new CurrentSong();
        List<SongView> songViews = new List<SongView>();
        bool playing = false;

        public MainPage()
        {
            this.InitializeComponent();
            StackPanelCurrentSong.DataContext = currentSong;
            Windows.Media.MediaControl.PlayPauseTogglePressed += MediaControl_PlayPauseTogglePressed;
            Windows.Media.MediaControl.PlayPressed +=MediaControl_PlayPressed;
            Windows.Media.MediaControl.PausePressed += MediaControl_PausePressed;
            Windows.Media.MediaControl.StopPressed += MediaControl_StopPressed;
        }

        void MediaControl_StopPressed(object sender, object e)
        {
            MediaElement1.Stop();
            playing = false;
        }

        void MediaControl_PausePressed(object sender, object e)
        {
            MediaElement1.Stop();
            playing = false;
        }

        void MediaControl_PlayPressed(object sender, object e)
        {
            MediaElement1.Play();
            playing = true;
        }

        void MediaControl_PlayPauseTogglePressed(object sender, object e)
        {
            // TODO: Invoke on UI thread
            Debug.WriteLine("Pause");
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, PlayPauseToggle);
        }

        void PlayPauseToggle()
        {
            if (playing)
            {
                MediaElement1.Pause();
                playing = false;
            }
            else
            {
                MediaElement1.Play();
                playing = true;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void UploadedSongsLoadButton_Click(object sender, RoutedEventArgs e)
        {
            var query = from s in musicEntities.SONG where s.UPLOADED == true orderby s.ID descending select s;
            Songs.LoadCompleted += Songs_LoadCompleted;
            Songs.LoadAsync(query);
        }

        void Songs_LoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            songViews = new List<SongView>();
            DataServiceCollection<SONG> songs = (DataServiceCollection<SONG>)sender;
            foreach (SONG song in songs)
            {
                SongView songView = new SongView(song);
                songViews.Add(songView);
            }
            SongGrid.ItemsSource = songViews;
        }

        private void MellowSongsLoadButton_Click(object sender, RoutedEventArgs e)
        {
            var query = from s in musicEntities.MellowSongs select s;
            MellowSongs.LoadCompleted += MellowSongs_LoadCompleted;
            MellowSongs.LoadAsync(query);
        }

        void MellowSongs_LoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            songViews = new List<SongView>();
            DataServiceCollection<MellowSongs> songs = (DataServiceCollection<MellowSongs>)sender;
            foreach (MellowSongs song in songs)
            {
                SongView songView = new SongView(song);
                songViews.Add(songView);
            }
            SongGrid.ItemsSource = songViews;
        }

        private void RandomSongsLoadButton_Click(object sender, RoutedEventArgs e)
        {
            var query = from s in musicEntities.RandomSongs select s;
            RandomSongs.LoadCompleted += RandomSongs_LoadCompleted;
            RandomSongs.LoadAsync(query);
            SongGrid.ItemsSource = RandomSongs;
        }

        void RandomSongs_LoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            songViews = new List<SongView>();
            DataServiceCollection<RandomSongs> songs = (DataServiceCollection<RandomSongs>)sender;
            foreach (RandomSongs song in songs)
            {
                SongView songView = new SongView(song);
                songViews.Add(songView);
            }
            SongGrid.ItemsSource = songViews;
        }

        private void PlaySong()
        {
            if (songViews.Count == 0)
            {
                return;
            }

            string location = songViews[songIndex].Location;
            MediaElement1.Source = new Uri(location);
            MediaElement1.Play();
            Windows.Media.MediaControl.TrackName = songViews[songIndex].TITLE;
            Windows.Media.MediaControl.ArtistName = songViews[songIndex].ARTIST;
            currentSong.SetSong(songViews[songIndex]);
            StackPanelCurrentSong.DataContext = currentSong;
            playing = true;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            PlaySong();
        }        

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            MediaElement1.Stop();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (songViews.Count == 0)
            {
                return;
            }

            songIndex++;

            if (songIndex == songViews.Count)
            {
                songIndex = 0;
            }
            PlaySong();
        }

        private void MediaElement1_MediaEnded(object sender, RoutedEventArgs e)
        {
            songIndex++;
            if (songIndex == songViews.Count)
            {
                songIndex = 0;
            }
            PlaySong();
        }

        async void StackPanel_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;
            PopupMenu popupMenu = new PopupMenu();
            popupMenu.Commands.Add(new UICommand("Play", OnSongTapped, fe.DataContext));
            await popupMenu.ShowAsync(e.GetPosition(this));
        }

        void OnSongTapped(IUICommand command)
        {
            SongView songView = (SongView)command.Id;
            MediaElement1.Source = new Uri(songView.Location);
            MediaElement1.Play();
            currentSong.SetSong(songView);
            StackPanelCurrentSong.DataContext = currentSong;
        }

        private void MediaElement1_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine("Error");
            songIndex++;
            if (songIndex == songViews.Count)
            {
                songIndex = 0;
            }
            PlaySong();
        }
    }
}
