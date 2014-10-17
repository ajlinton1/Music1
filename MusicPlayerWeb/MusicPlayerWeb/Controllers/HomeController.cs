using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MusicPlayerWeb.Models;

namespace MusicPlayerWeb.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public ActionResult Player()
        {
            return View();
        }

    }
}
