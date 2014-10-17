using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace MusicTest
{
    public class MusicDao
    {
        static String connString = "Data Source=dedsql004.mydbserver.net, 10030;Initial Catalog=mycruitercom;User Id=mycruitercom;Password=mycYRs8a;";
        static String destinationFolder = @"I:\Music";
        int songCount = -1;

        public MusicDao()
        {
            SqlConnection con = null;
            String ret = null;

            try
            {
                con = new SqlConnection(connString);
                String sql = "SELECT COUNT(*) AS Expr1 FROM SONGS";
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                songCount = (int)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                con.Close();
            }
        }

        public static void InsertSong(String title, String artist, String album, String genre, long duration, long bitrate, long filesize,String location)
        {
            SqlConnection con = null;

            try
            {
                con = new SqlConnection(connString);
                String sql = "INSERT INTO SONGS (TITLE,ARTIST,ALBUM,GENRE,DURATION,BITRATE,FILESIZE,LOCATION) VALUES (@Title,@Artist,@Album,@Genre,@Duration,@Bitrate,@Filesize,@Location)";
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Artist", artist);
                cmd.Parameters.AddWithValue("@Album", album);
                cmd.Parameters.AddWithValue("@Genre", genre);
                cmd.Parameters.AddWithValue("@Duration", duration);
                cmd.Parameters.AddWithValue("@Bitrate", bitrate);
                cmd.Parameters.AddWithValue("@Filesize", filesize);
                cmd.Parameters.AddWithValue("@Location", location);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                con.Close();
            }
        }

        public String PickRandomSong()
        {
            SqlConnection con = null;
            String ret = null;
            String sql;
            SqlCommand cmd;

            try
            {
                con = new SqlConnection(connString);
                con.Open();

                Random random = new Random();
                int songId = random.Next(songCount);
                sql = "SELECT * FROM SONGS WHERE ID=@ID";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@ID", songId);
                SqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                ret = rdr["LOCATION"].ToString();
                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
            finally
            {
                con.Close();
            }
            return ret;
        }

        public bool CopySong(String source)
        {
            try
            {
                String destination = destinationFolder + @"\" + Path.GetFileName(source);
                if (!File.Exists(destination))
                {
                    File.Copy(source, destination);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                if (ex.Message.Contains("There is not enough space on the disk."))
                {
                    Console.WriteLine("Device full");
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }

    }
}
