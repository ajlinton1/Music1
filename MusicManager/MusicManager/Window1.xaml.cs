using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;
using Azure;
using Music;
using MusicManager.Properties;

namespace MusicManager
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        ObservableCollection<String> newSongList = new ObservableCollection<string>();
        ObservableCollection<String> errorList = new ObservableCollection<string>();
        ObservableCollection<String> fillList = new ObservableCollection<string>();
        ObservableCollection<String> artistList = new ObservableCollection<string>();

        public Window1()
        {
            InitializeComponent();
            listViewNewSongs.ItemsSource = newSongList;
            listViewErrors.ItemsSource = errorList;
            listViewFill.ItemsSource = fillList;
            listViewArtists.ItemsSource = artistList;
        }

        private void buttonIndex_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                labelStatus.Content = "Started";
                 new Thread(() => { Reindex(); } ).Start();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
            }
        }

        public void NewSong(String newSongName)
        {
            Action update = () =>
            {
                newSongList.Add(newSongName);
            };
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, update);
       }

        public void Error(String errorInfo)
        {
            Action update = () =>
            {
                errorList.Add(errorInfo);
            };
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, update);
        }

        public void Status(String statusInfo)
        {
            Action update = () =>
            {
                Debug.WriteLine(statusInfo);
                labelStatus.Content = statusInfo;
                if (statusInfo.Contains("Done"))
                {
                    progressBarIndex.Value = progressBarIndex.Maximum;
                }
                else
                {
                    progressBarIndex.Value = ++progressBarIndex.Value;
                    if (progressBarIndex.Value == progressBarIndex.Maximum)
                    {
                        progressBarIndex.Value = 0;
                    }
                }
            };
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, update);
        }

        public void Fill(String title)
        {
            Action update = () =>
            {
                fillList.Add(title);
            };
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, update);
        }

        public void AddArtist(String artistName)
        {
            Action update = () =>
            {
                artistList.Add(artistName);
            };
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, update);
        }
                
        public void Reindex(int threadsNum = 10)
        {
            Model model = new Model();
            string[] musicFolders = new string[1];
            musicFolders[0] = Settings.Default.MusicFolder;
            var azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            IUploadProcessor uploadProcessor = new UploadProcessor(Settings.Default.AzureContainerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            var musicDao = new MusicDao(Settings.Default.ConnectionString, musicFolders, uploadProcessor, azureService);
            model.Reindex(threadsNum, musicFolders, musicDao);
        }

        private void buttonFill_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => { Fill(); }).Start();
        }

        public void Fill()
        {
            Status("Filling");
            AzureService azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            IUploadProcessor uploadProcessor = new UploadProcessor(Settings.Default.AzureContainerName, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
            IMusicDao musicDao = new MusicDao(Settings.Default.ConnectionString, new string[] { Settings.Default.MusicFolder }, uploadProcessor, azureService);
            musicDao.Initialize();
            musicDao.RandomFill();
            Status("Done");
        }

    }
}
