using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MusicPlayer8.MusicService;

namespace MusicPlayer8
{
    public class CurrentSong : INotifyPropertyChanged
    {
        private SONG song;

        public void SetSong(SONG song)
        {
            this.song = song;
            this.Title = song.TITLE;
            this.Artist = song.ARTIST;
            this.Genre = song.GENRE;
            this.Rating = song.RATING;
            this.Location = song.LOCATION;
        }

        public void SetSong(MellowSongs mellowSong)
        {
            this.song = new SONG();
            this.song.TITLE = mellowSong.TITLE;
            this.song.ARTIST = mellowSong.ARTIST;
            this.song.GENRE = mellowSong.GENRE;
            this.Title = mellowSong.TITLE;
            this.Artist = mellowSong.ARTIST;
            this.Genre = mellowSong.GENRE;
            this.Rating = song.RATING;
            this.Location = song.LOCATION;
        }

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public String Title
        {
            get
            {
                if (song != null)
                {
                    return song.TITLE;
                }
                else
                {
                    return "N/A";
                }
            }
            set
            {
                RaisePropertyChanged();
            }
        }

        public String Artist
        {
            get
            {
                if (song != null)
                {
                    return song.ARTIST;
                }
                else
                {
                    return "N/A";
                }
            }
            set
            {
                RaisePropertyChanged();
            }
        }

        public String Genre
        {
            get
            {
                if (song != null)
                {
                    return song.GENRE;
                }
                else
                {
                    return "N/A";
                }
            }
            set
            {
                RaisePropertyChanged();
            }
        }

        public int Rating
        {
            get
            {
                if (song != null)
                {
                    return song.RATING;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                RaisePropertyChanged();
            }
        }

        public string Location
        {
            get
            {
                if (song != null)
                {
                    return song.LOCATION;
                }
                else
                {
                    return "N/A";
                }
            }
            set
            {
                RaisePropertyChanged();
            }
        }

    }
}
