using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Runtime.Serialization.Json;

namespace MusicPlayer8
{
    public class Player
    {
        public void Play()
        {
            Debug.WriteLine("Playing");
        }

        public void LoadMellowSongs()
        {
        }

        public static async Task<T> Get<T>(string url)
        {
            HttpClient client = new HttpClient();
            Stream stream = await client.GetStreamAsync(url);

            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }

        //private static async Task<ApiPhotos> LoadInteresting()
        //{
        //    string url = "http://api.flickr.com/services/rest/?method=flickr.interestingness.getList&api_key={0}&format=json&nojsoncallback=1";
        //    url = string.Format(url, Constants.ApiKey);

        //    ApiResult result = await WebHelper.Get<ApiResult>(url);

        //    return result.Photos;
        //}

    }
}
