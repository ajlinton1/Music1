using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Music;

namespace TestProject1
{
    public class MockUploadProcessor : IUploadProcessor
    {
        public void DrainQueue()
        {
            
        }

        public void Enqueue(Tuple<string, string> fileinfo)
        {
            
        }

        public int FilesUploaded
        {
            get
            {
                return -1;
            }
            set
            {
                
            }
        }

        public bool UploadFile(Tuple<string, string> fileinfo)
        {
            return true;
        }


        public event Action<string> OnStatus;

        public event Action<string> OnError;
    }
}
