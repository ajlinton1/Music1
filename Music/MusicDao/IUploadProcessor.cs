using System;

namespace Music
{
    public interface IUploadProcessor
    {
        void DrainQueue();
        void Enqueue(Tuple<string, string> fileinfo);
        int FilesUploaded { get; set; }
        bool UploadFile(Tuple<string, string> fileinfo);

        event Action<string> OnStatus;
        event Action<string> OnError;

    }
}
