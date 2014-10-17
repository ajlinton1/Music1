using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using TagUpdaterApp.Properties;

namespace TagUpdaterApp
{
    public class TagUpdaterApp
    {
        static void Main(string[] args)
        {
            TagUpdater tagUpdater = new TagUpdater();
            tagUpdater.UpdateTags(Settings.Default.ConnectionString, Settings.Default.MusicDrive);
        }
    }
}
