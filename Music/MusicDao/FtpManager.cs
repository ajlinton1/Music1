using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Music
{
    public class FtpManager
    {
        static String username = "210291";
        static String password = "mycYRs8a";
        static String address = "ftp://65.182.208.175/[Account]com/html/Media/";

        public static void Upload(String sourceFilename,int fileNumber)
        {
            String remoteFilename = String.Format("{0}Track{1}.WMA", address,fileNumber);
            Uri ftpSite = new Uri(remoteFilename);
            FileInfo fileInfo = new FileInfo(sourceFilename);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpSite);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;
            request.ContentLength = fileInfo.Length;
            request.Credentials = new NetworkCredential(username, password);
            byte[] byteBuffer = new byte[4096];
            using (Stream requestStream = request.GetRequestStream())
            {
                using (FileStream fileStream = new FileStream(sourceFilename, FileMode.Open))
                {
                    int bytesRead = 0;
                    do
                    {
                        bytesRead = fileStream.Read(byteBuffer, 0, byteBuffer.Length);
                        if (bytesRead > 0)
                        {
                            requestStream.Write(byteBuffer, 0, bytesRead);
                        }
                    }
                    while (bytesRead > 0);
                }
            }
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine(response.StatusDescription);
            }
        }

        public static IEnumerable<String> List()
        {
            String remoteFilename = address; 
            Uri ftpSite = new Uri(remoteFilename);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpSite);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.UseBinary = true;
            request.Credentials = new NetworkCredential(username, password);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = null;
            StreamReader readStream = null;
            String songListString = null;
            try
            {
                responseStream = response.GetResponseStream();
                readStream = new StreamReader(responseStream, System.Text.Encoding.UTF8);

                if (readStream != null)
                {
                    // Display the data received from the server.
                    songListString = readStream.ReadToEnd();
                    Console.WriteLine(songListString);
                }
                Console.WriteLine("List status: {0}", response.StatusDescription);
            }
            finally
            {
                if (readStream != null)
                {
                    readStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }

            char[] c = { '\n', '\r' };
            String[] songs = songListString.Split(c,StringSplitOptions.RemoveEmptyEntries);
            return songs;
        }

        public static void Delete(String song)
        {
            String remoteFilename = String.Format("{0}{1}", address, song);
            Console.WriteLine(remoteFilename);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remoteFilename);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.UseBinary = true;
            request.Credentials = new NetworkCredential(username, password);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
/*            MusicDao musicDao=new MusicDao();
            String songIdString = song.Substring(5);
            songIdString = songIdString.Substring(0,songIdString.IndexOf('.'));
            int songId = Convert.ToInt32(songIdString);
            musicDao.UpdateSongUploadStatus(songId, false); */
        }

        public static void DeleteAll()
        {
            IEnumerable<String> songs = List();
            foreach (String song in songs)
            {
                Delete(song);
            }
        }

    }
}
