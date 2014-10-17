using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MusicPlayerWeb.Util;

namespace MusicPlayerWeb
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}"
            );

            config.Routes.MapHttpRoute(
                name: "MusicApi",
                routeTemplate: "api/{controller}/{genre}/{sequence}/{skip}/{artist}/{location}",
                defaults: new { genre = "", sequence = "", skip = 0 , artist = "", location = "" }
            );

			config.Formatters.Add(new JsonpMediaTypeFormatter());
        }
    }
}
