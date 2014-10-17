using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MusicPlayer1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

			config.Routes.MapHttpRoute(
				name: "MusicApi",
//				routeTemplate: "api/{controller}/{genre}/{sequence}/{skip}/{artist}/{location}",
				routeTemplate: "api/{controller}/{skip}/{artist}/{genre}",
//				defaults: new { genre = "", sequence = "", skip = 0, artist = "", location = "" }
				defaults: new { skip = "", artist = "", genre = ""}
			);

        }
    }
}
