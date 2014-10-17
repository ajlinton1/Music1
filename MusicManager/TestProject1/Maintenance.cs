using System;
using System.Linq;
using System.Data.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using Music;
using HundredMilesSoftware.UltraID3Lib;
using IdSharp.Tagging.ID3v2;
using IdSharp.Tagging.ID3v1;
using Azure;
using TestProject1.Properties;
using com.andrewlinton.music;

namespace TestProject1
{
    [TestClass]
    public class Maintenance
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
        public void MissingSongs()
        {
            int songId = 0;
            int batchSize = 100;

            while (true)
            {
                bool songTaken = false;
                using (var dataContext = new MusicEntities1())
                {
                    IQueryable<SONG> songs = dataContext.SONG.Where(s => s.ID > songId).OrderBy(s => s.ID).Take(batchSize);

                    foreach (SONG song in songs)
                    {
                        songTaken = true;
                        try
                        {
                            FileInfo fileInfo = new FileInfo(song.LOCATION);
                            string directoryRoot = Directory.GetDirectoryRoot(song.LOCATION);
                            string filename = song.LOCATION;
                            if (directoryRoot.ToLower().IndexOf("e:\\") == -1)
                            {
                                filename = filename.Substring(1);
                                filename = "e" + filename;
                            }

                            if (!File.Exists(filename))
                            {
                                testContextInstance.WriteLine("File not found: {0}", filename);

                                using (var dataContext1 = new MusicEntities1())
                                {
                                    testContextInstance.WriteLine("Deleting SONG {0}", song.ID);
                                    SONG missingSong = dataContext1.SONG.Where(s => s.ID == song.ID).FirstOrDefault();
                                    missingSong.LOCATION = null;
                                    dataContext1.SONG.Remove(missingSong);
                                    dataContext1.SaveChanges();
                                }
                            }
                        }
                        catch (System.IO.DirectoryNotFoundException)
                        {
                            testContextInstance.WriteLine("Exception, file not found, song: {0}", song.LOCATION);
                        }
                        catch (System.IO.FileNotFoundException)
                        {
                            testContextInstance.WriteLine("Exception, file not found, song: {0}", song.LOCATION);
                        }
                        catch (Exception ex)
                        {
                            testContextInstance.WriteLine("Exception, song: {0}, {1}, {2} ", song.ID, ex.Message, ex.StackTrace);
                        }
                        songId = song.ID;
                    }
                }
                if (!songTaken)
                {
                    break;
                }
            }
        }

        [TestMethod]
        public void TagUpdaterTest()
        {
            string musicDrive = "e";
            TagUpdater tagUpdater = new TagUpdater();
            tagUpdater.UpdateTags(Settings.Default.ConnectionString, musicDrive);
        }

        [TestMethod]
        public void SyncTagsToDb()
        {
            using (var dataContext = new MusicEntities1())
            {
                IQueryable<SONG> songs = from s in dataContext.SONG
                                         orderby s.ID
                                         select s;

                foreach (SONG song in songs)
                {
                    try
                    {
                        if (Regex.IsMatch(song.LOCATION,".mp3",RegexOptions.IgnoreCase))
                        {
                            FileInfo fileInfo = new FileInfo(song.LOCATION);
                            string directoryRoot = Directory.GetDirectoryRoot(song.LOCATION);
                            string filename = song.LOCATION;
                            if (directoryRoot.ToLower().IndexOf("e:\\") == -1)
                            {
                                filename = filename.Substring(1);
                                filename = "e" + filename;
                            }

                            string genre = null;

                            UltraID3 ultraID3 = new UltraID3();
                            ultraID3.Read(filename);

                            genre = ultraID3.Genre;

                            IID3v2 iD3v2 = ID3v2Helper.CreateID3v2(filename);

                            if (string.IsNullOrWhiteSpace(genre))
                            {
                                genre = iD3v2.Genre;
                            }

                            IID3v1 iD3v1 = ID3v1Helper.CreateID3v1(filename);
                            if (string.IsNullOrWhiteSpace(genre))
                            {
                                genre = GenreHelper.GenreByIndex[iD3v1.GenreIndex];
                            }

                            ID3v2Tag id3v2Tag = ultraID3.ID3v2Tag;

                            //if ((!string.IsNullOrEmpty(ultraID3.Genre)) &&
                            //    (!string.IsNullOrEmpty(id3v2Tag.Genre)) &&
                            //    (ultraID3.Genre != id3v2Tag.Genre)
                            //    )
                            //{
                            //    testContextInstance.WriteLine("{0}, {1}", ultraID3.Genre, id3v2Tag.Genre);
                            //}

                            //genre = id3v2Tag.Genre;

                            //if (string.IsNullOrWhiteSpace(genre))
                            //{
                            //    genre = ultraID3.Genre;
                            //}

                            //if (!string.IsNullOrWhiteSpace(ultraID3.Genre))
                            //{
                            //    genre = ultraID3.Genre;
                            //}

                            //if (string.IsNullOrWhiteSpace(genre))
                            //{
                            //    genre = id3v2Tag.Genre;
                            //}


                            if ((genre != song.GENRE) 
                                && (!string.IsNullOrWhiteSpace(song.GENRE))
                                //&& (song.GENRE.ToLower().IndexOf("blues")==-1)
                                //&& (song.GENRE.ToLower().IndexOf("other") == -1)
                                //&& (song.GENRE.ToLower().IndexOf("unknown") == -1)
                                )
                            {
                                testContextInstance.WriteLine("{0}, DB genre: {1}, Disk genre: {2}", filename, song.GENRE, genre);

                                try
                                {
                                    if ((song.UPDATED > fileInfo.LastWriteTime)
                                        && (song.GENRE != "Other"))
                                    {
                                        id3v2Tag.Genre = song.GENRE;
                                        ultraID3.Write();
                                    }
                                    else if (genre!="Other")
                                    {
                                        song.GENRE = genre;
                                    }
                                }
                                catch (Exception e1)
                                {
                                    testContextInstance.WriteLine("Exception, song: {0}, {1}, {2}", song.LOCATION, e1.Message, e1.StackTrace);
                                }
                                
                            }
                        }
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        testContextInstance.WriteLine("Exception, file not found, song: {0}", song.LOCATION);
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        testContextInstance.WriteLine("Exception, file not found, song: {0}", song.LOCATION);
                    }
                    catch (Exception ex)
                    {
                        testContextInstance.WriteLine("Exception, song: {0}, {1}, {2}", song.LOCATION, ex.Message, ex.StackTrace);
                    }
                }
                dataContext.SaveChanges();
            }
        }

        [TestMethod]
        public void SyncTagsToFiles()
        {
            using (var dataContext = new MusicEntities1())
            {
                IQueryable<SONG> songs = dataContext.SONG.OrderByDescending(s => s.UPDATED).Take(5000);
                SONG[] songArray = songs.ToArray();

                foreach (SONG song in songArray)
                {
                    try
                    {
                        if (Regex.IsMatch(song.LOCATION, ".mp3", RegexOptions.IgnoreCase))
                        {
                            FileInfo fileInfo = new FileInfo(song.LOCATION);
                            string directoryRoot = Directory.GetDirectoryRoot(song.LOCATION);
                            string filename = song.LOCATION;
                            if (directoryRoot.ToLower().IndexOf("e:\\") == -1)
                            {
                                filename = filename.Substring(1);
                                filename = "e" + filename;
                            }

                            string genre = null;

                            IID3v2 iD3v2 = ID3v2Helper.CreateID3v2(filename);
                            genre = iD3v2.Genre;

                            UltraID3 ultraID3 = new UltraID3();
                            ultraID3.Read(filename);
                            if (string.IsNullOrWhiteSpace(genre))
                            {
                                genre = ultraID3.Genre;
                            }
                            else if (ultraID3.Genre != genre)
                            {
//                                ultraID3.Genre = genre;
                            }

                            IID3v1 iD3v1 = ID3v1Helper.CreateID3v1(filename);
                            if (Regex.IsMatch(genre,"("))
                            {
                                genre = GenreHelper.GenreByIndex[iD3v1.GenreIndex];
                            }

//                            ID3v2Tag id3v2Tag = ultraID3.ID3v2Tag;


                            if ((genre != song.GENRE)
                                && (!string.IsNullOrWhiteSpace(song.GENRE))
                                //&& (song.GENRE.ToLower().IndexOf("blues")==-1)
                                //&& (song.GENRE.ToLower().IndexOf("other") == -1)
                                //&& (song.GENRE.ToLower().IndexOf("unknown") == -1)
                                )
                            {
                                testContextInstance.WriteLine("{0}, DB genre: {1}, Disk genre: {2}", filename, song.GENRE, genre);

                                try
                                {
                                    ultraID3.Genre = song.GENRE;
                                    iD3v2.Genre = song.GENRE;
                                }
                                catch (Exception e1)
                                {
                                    testContextInstance.WriteLine("Exception, song: {0}, {1}, {2}", song.LOCATION, e1.Message, e1.StackTrace);
                                }

                                iD3v2.Save(filename);
                                ultraID3.Write();
                            }
                        }
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        testContextInstance.WriteLine("Exception, file not found, song: {0}", song.LOCATION);
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        testContextInstance.WriteLine("Exception, file not found, song: {0}", song.LOCATION);
                    }
                    catch (Exception ex)
                    {
                        testContextInstance.WriteLine("Exception, song: {0}, {1}, {2}", song.LOCATION, ex.Message, ex.StackTrace);
                    }
                }
            }
        }
    }
}
