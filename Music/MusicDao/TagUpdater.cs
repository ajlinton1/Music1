using System;
using System.Linq;
//using System.Data.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using Music;
using HundredMilesSoftware.UltraID3Lib;
using IdSharp.Tagging.ID3v2;
using IdSharp.Tagging.ID3v1;
using Azure;
using com.andrewlinton.music;

namespace Music
{
    public class TagUpdater
    {
        public void UpdateTags(string connectionString, string musicDrive)
        {
            int songId = 0;
            int batchSize = 100;

            while (true)
            {
                bool songTaken = false;
                using (var dataContext = new MusicEntities1())
                {
                    IQueryable<SONG> songs = dataContext.SONG.Where(s => s.ID > songId && s.LOCATION.ToLower().Contains(".mp3")).OrderBy(s => s.ID).Take(batchSize);

                    foreach (SONG song in songs)
                    {
                        songTaken = true;
                        try
                        {
                            FileInfo fileInfo = new FileInfo(song.LOCATION);
                            string directoryRoot = Directory.GetDirectoryRoot(song.LOCATION);
                            string filename = song.LOCATION;
                            if (directoryRoot.ToLower().IndexOf(musicDrive) !=0)
                            {
                                filename = filename.Substring(1);
                                filename = musicDrive + filename;
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

                            if ((genre != song.GENRE)
                                && (!string.IsNullOrWhiteSpace(song.GENRE))
                                && (song.GENRE.ToLower().IndexOf("blues") == -1)
                                && (song.GENRE.ToLower().IndexOf("other") == -1)
                                && (song.GENRE.ToLower().IndexOf("unknown") == -1)
                                )
                            {
                                Console.WriteLine(String.Format("{0}, DB genre: {1}, Disk genre: {2}", filename, song.GENRE, genre));

                                try
                                {
                                    ultraID3.Genre = song.GENRE;
                                }
                                catch (Exception e1)
                                {
                                    Console.WriteLine(String.Format("Exception, song: {0}, {1}, {2}", song.LOCATION, e1.Message, e1.StackTrace));
                                }

                                id3v2Tag.Genre = song.GENRE;
                                ultraID3.Write();
                            }
                        }
                        catch (System.IO.DirectoryNotFoundException)
                        {
                            Console.WriteLine(String.Format("Exception, file not found, song: {0}", song.LOCATION));
                        }
                        catch (System.IO.FileNotFoundException)
                        {
                            Console.WriteLine(String.Format("Exception, file not found, song: {0}", song.LOCATION));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(String.Format("Exception, song: {0}, {1}, {2}", song.LOCATION, ex.Message, ex.StackTrace));
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
    }
}

        //public void UpdateTags()
        //{
        //    using (DataClasses1DataContext dataContext = new DataClasses1DataContext(Music.Properties.Settings.Default.ConnectionString))
        //    {
        //        IQueryable<SONG> songs = from s in dataContext.SONGs
        //                                 orderby s.ID
        //                                 select s;

        //        foreach (SONG song in songs)
        //        {
        //            try
        //            {
        //                if (song.LOCATION.ToLower().IndexOf("mp3") > -1)
        //                {
        //                    FileInfo fileInfo = new FileInfo(song.LOCATION);
        //                    string directoryRoot = Directory.GetDirectoryRoot(song.LOCATION);
        //                    string filename = song.LOCATION;
        //                    if (directoryRoot.ToLower().IndexOf("e:\\") == -1)
        //                    {
        //                        filename = filename.Substring(1);
        //                        filename = "e" + filename;
        //                    }

        //                    string genre = null;

        //                    UltraID3 ultraID3 = new UltraID3();
        //                    ultraID3.Read(filename);

        //                    genre = ultraID3.Genre;

        //                    IID3v2 iD3v2 = ID3v2Helper.CreateID3v2(filename);

        //                    if (string.IsNullOrWhiteSpace(genre))
        //                    {
        //                        genre = iD3v2.Genre;
        //                    }

        //                    IID3v1 iD3v1 = ID3v1Helper.CreateID3v1(filename);
        //                    if (string.IsNullOrWhiteSpace(genre))
        //                    {
        //                        genre = GenreHelper.GenreByIndex[iD3v1.GenreIndex];
        //                    }

        //                    ID3v2Tag id3v2Tag = ultraID3.ID3v2Tag;

        //                    if ((genre != song.GENRE) 
        //                        && (!string.IsNullOrWhiteSpace(song.GENRE))
        //                        && (song.GENRE.ToLower().IndexOf("blues")==-1)
        //                        && (song.GENRE.ToLower().IndexOf("other") == -1)
        //                        && (song.GENRE.ToLower().IndexOf("unknown") == -1)
        //                        )
        //                    {
        //                        Console.WriteLine(String.Format("{0}, DB genre: {1}, Disk genre: {2}", filename, song.GENRE, genre));

        //                        try
        //                        {
        //                            ultraID3.Genre = genre;
        //                        }
        //                        catch (Exception e1)
        //                        {
        //                            Console.WriteLine(String.Format("Exception, song: {0}, {1}, {2}", song.LOCATION, e1.Message, e1.StackTrace));
        //                        }
                                
        //                        id3v2Tag.Genre = genre;
        //                        ultraID3.Write();
        //                    }
        //                }
        //            }
        //            catch (System.IO.DirectoryNotFoundException)
        //            {
        //                Console.WriteLine(String.Format("Exception, file not found, song: {0}", song.LOCATION));
        //            }
        //            catch (System.IO.FileNotFoundException)
        //            {
        //                Console.WriteLine(String.Format("Exception, file not found, song: {0}", song.LOCATION));
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine(String.Format("Exception, song: {0}, {1}, {2}", song.LOCATION, ex.Message, ex.StackTrace));
        //            }
        //        }
