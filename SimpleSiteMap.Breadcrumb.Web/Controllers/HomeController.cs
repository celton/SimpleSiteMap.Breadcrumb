using SimpleSiteMapController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleSiteMap.Breadcrumb.Web.Controllers
{
    public class HomeController : Controller
    {
        [SimpleSiteMapController("Início")]
        public ActionResult Index()
        {
            return View();
        }

        [SimpleSiteMapController("Parent - 1")]
        public ActionResult Parent1()
        {
            return View();
        }


        [SimpleSiteMapController("Parent - 2")]
        public ActionResult Parent2()
        {
            return View();
        }

        [SimpleSiteMapController("I'm the Brother", "Home", "Parent2")]
        public ActionResult BrotherOfParent2()
        {
            return View();
        }
    }
}