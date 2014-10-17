using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject1.AzureMusic;

namespace TestProject1
{
    [TestClass]
    public class AzureTest
    {
        [TestMethod()]
        public void GetMellowSongsAzure()
        {
            ServiceMusicClient serviceMusicClient = new ServiceMusicClient();
            TestProject1.AzureMusic.SONG[] songs = serviceMusicClient.GetMellowSongs(100);
            foreach (TestProject1.AzureMusic.SONG song in songs)
            {
                Console.WriteLine(String.Format("{0} {1} {2}", song.TITLE, song.RATING, song.GENRE));
            }
        }

        [TestMethod()]
        public void GetRandomSongsAzure()
        {
            ServiceMusicClient serviceMusicClient = new ServiceMusicClient();
            TestProject1.AzureMusic.SONG[] songs = serviceMusicClient.GetRandomSongs(100);
            foreach (TestProject1.AzureMusic.SONG song in songs)
            {
                Console.WriteLine(String.Format("{0} {1}", song.TITLE, song.RATING));
            }
        }

        [TestMethod()]
        public void GetRecentSongsAzure()
        {
            ServiceMusicClient serviceMusicClient = new ServiceMusicClient();
            TestProject1.AzureMusic.SONG[] songs = serviceMusicClient.GetRecentSongs(100);
            foreach (TestProject1.AzureMusic.SONG song in songs)
            {
                Console.WriteLine(String.Format("{0} {1}", song.TITLE, song.RATING));
            }
        }

    }
}
