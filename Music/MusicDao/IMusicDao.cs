using System;
using System.Collections.Generic;
using com.andrewlinton.music;

namespace Music
{
    public interface IMusicDao
    {
        bool CopySong(string source);
        void CrawlFolder(string folder, Action<string> fileAction);
        void CrawlFoldersMp3(System.Collections.Generic.IEnumerable<string> musicFolders);
        void CrawlFoldersWMA(System.Collections.Generic.IEnumerable<string> musicFolders);
        System.Collections.Generic.IEnumerable<string> FilenameList { get; }
        System.Collections.Concurrent.ConcurrentQueue<string> FilenameQueue { get; set; }
        void GetMetadataFromTitle(string filename);
        void GetMp3Tags(string filename, out string title, out string artist, out string album, out string genre);
        void GetProcSongs();
        System.Collections.Generic.IEnumerable<SONG> GetSelectedSongs(System.Collections.Generic.IEnumerable<string> desiredGenres);
        SONG GetSongById(int songId);
        void GetSongContent();
        void GetSongMetadata(string filename, out string title, out string artist, out string album, out string genre);
        System.Collections.Generic.List<SONG> GetSongsByArtist(int artistId);
        void GetTags(string filename, out string title, out string artist, out string album, out string genre);
        void GetWmaTags(string filename, out string title, out string artist, out string album, out string genre, out long duration, out long bitrate, out long filesize);
        bool InDb(string filename);
        void Initialize();
        void InsertArtist(string name);
        void InsertSongContent();
        void NoArtist();
        event Action<string> OnError;
        event Action<string> OnNewArtist;
        event Action<string> OnNewSong;
        event Action<string> OnProcessFile;
        event Action<string> OnStatus;
        SONG PickRandomSong(Func<SONG, bool> selectSong);
        void ProcessFile(string fileName);
        void ProcessFilename(object stateInfo);
        void ProcessFileWorker(string filename);
        void ProcessFileWorkerConcurrent(string filename);
        void ProcessQueuedFilenamesWorker(object stateInfo);
        void ProcessQueueFilenames(int threadsNum);
        void QueryArtists();
        void QuerySongs();
        void RandomFill();
        bool SongProcessingDone();
        int SongsInserted { get; set; }
        void UpdateArtists();
        void UploadSongs(int numSongs);

        IEnumerable<SONG> GetSongFiles(string folder);

        bool HashExists(SONG song);

        bool InDb(SONG song);

        SONG InsertSong(SONG song);

        bool UpdateSong(String title, String artist, String album, String genre, long? duration, long? bitrate, long? filesize, String location);

        SONG UploadSong(SONG song);

        SONG EnqueueSong(SONG song);
    }
}
