using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using com.andrewlinton.AzureTable;

namespace MusicPlayer1.Tests
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class UnitTest1
	{
		public UnitTest1()
		{
			//
			// TODO: Add constructor logic here
			//
		}

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

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void GetSongs()
		{
			bool songsReturned = false;
			var musicRepository = new MusicRepository();
			var songs = musicRepository.GetSongs(0, null, null);
			foreach (var song in songs)
			{
				testContextInstance.WriteLine(string.Format("{0} : {1} : {2}: {3} {4}", song.Artist, song.Title, song.PartitionKey, song.RowKey, song.Location));
				songsReturned = true;
			}
			Assert.IsTrue(songsReturned);
		}
		
		[TestMethod]
		public void GetSongsSkip()
		{
			bool songsReturned = false;
			int skip = 100;
			var musicRepository = new MusicRepository();
			var songs = musicRepository.GetSongs(skip, null, null);
			foreach (var song in songs)
			{
				testContextInstance.WriteLine(string.Format("{0} : {1} : {2}: {3} {4}", song.Artist, song.Title, song.PartitionKey, song.RowKey, song.Location));
				songsReturned = true;
			}
			Assert.IsTrue(songsReturned);
		}

		[TestMethod]
		public void GetSongsFilterArtist()
		{
			bool songsReturned = false;
			int skip = 0;
			string artist = "M83";
			var musicRepository = new MusicRepository();
			var songs = musicRepository.GetSongs(skip, artist, null);
			foreach (var song in songs)
			{
				testContextInstance.WriteLine(string.Format("{0} : {1} : {2}: {3} {4}", song.Artist, song.Title, song.PartitionKey, song.RowKey, song.Location));
				songsReturned = true;
				Assert.AreEqual(artist, song.Artist);
			}
			Assert.IsTrue(songsReturned);
		}

		[TestMethod]
		public void GetSongsFilterGenre()
		{
			bool songsReturned = false;
			int skip = 0;
			string genre = "Ambient";
			var musicRepository = new MusicRepository();
			var songs = musicRepository.GetSongs(skip, null, genre);
			foreach (var song in songs)
			{
				testContextInstance.WriteLine(string.Format("{0} : {1} : {2}: {3} {4}", song.Artist, song.Title, song.PartitionKey, song.RowKey, song.Location));
				songsReturned = true;
				Assert.AreEqual(genre, song.Genre);
			}
			Assert.IsTrue(songsReturned);
		}

		[TestMethod]
		public void GetGenres()
		{
			var musicRepository = new MusicRepository();
			var genres = musicRepository.GetGenres();
			foreach (var genre in genres)
			{
				testContextInstance.WriteLine(genre.Name);
			}
		}
	}
}
