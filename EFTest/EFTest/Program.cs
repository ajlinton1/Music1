using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new MusicEntities())
            {
                var results = from song in context.SONGs
                              where song.ARTIST.IndexOf("Lucius") > -1
                              select song;

                foreach (var song in results)
                {
                    Console.WriteLine(song.TITLE);
                }
            }
        }
    }
}
