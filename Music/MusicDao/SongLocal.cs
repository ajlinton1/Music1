using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.andrewlinton.music;

namespace Music
{
    public class SongLocal : SONG
    {
        public SongLocal(SONG song)
        {
        }

        public DateTime? FileModifiedDate
        {
            get;
            set;
        }
    }
}
