using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Music;
using Azure;
using Indexer.Properties;

namespace Indexer
{
    class Indexer
    {
        AppEvents appEventLog = new AppEvents("Indexer", "Indexer");

        public Indexer(MusicDao musicDao, string[] musicFolders)
        {
            try
            {
                appEventLog.WriteToLog("Starting", EventLogEntryType.Information, CategoryType.AppStartUp, EventIDType.NA);
                musicDao.CrawlFoldersWMA(musicFolders);
                musicDao.CrawlFoldersMp3(musicFolders);
                musicDao.UpdateArtists();
            }
            catch (Exception ex)
            {
                appEventLog.WriteToLog(ex.Message, EventLogEntryType.Error, CategoryType.None, EventIDType.ExceptionThrown);
            }
            finally
            {
                appEventLog.WriteToLog("Terminating", EventLogEntryType.Information, CategoryType.AppShutDown, EventIDType.NA);
            }
        }

        static void Main(string[] args)
        {
            AzureService azureService = new AzureService(Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey);
 //           new Indexer(Settings.Default.ConnectionString, null, Settings.Default.AzureAccountName, Settings.Default.AzureAccountKey, Settings.Default.AzureContainerName, azureService);
        }

        public void NewSong(String newSongName)
        {
            appEventLog.WriteToLog(String.Format("New Song: {0}", newSongName), EventLogEntryType.Information, CategoryType.None, EventIDType.NA);
            Console.WriteLine("New Song: {0}", newSongName);
        }

        public void Error(String errorInfo)
        {
            appEventLog.WriteToLog(String.Format("Error: {0}", errorInfo), EventLogEntryType.Error, CategoryType.None, EventIDType.ExceptionThrown);
            Console.WriteLine("Error: {0}", errorInfo);
        }

        public void Status(String statusInfo)
        {
            Console.WriteLine("Status: {0}", statusInfo);
        }

        public void AddArtist(String artistName)
        {
            appEventLog.WriteToLog(String.Format("Adding Artist: {0}", artistName), EventLogEntryType.Information, CategoryType.None, EventIDType.NA);
            Console.WriteLine("Adding Artist: {0}", artistName);
        }

    }
}
