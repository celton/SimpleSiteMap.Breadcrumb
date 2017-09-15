using SimpleSiteMapController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleSiteMap.Breadcrumb.Web.Areas.Children.Controllers
{
    public class Children2Controller : Controller
    {
        // GET: Children/Children2
        [SimpleSiteMapController("Children - 2", "Home", "Parent2")]
        public ActionResult Index()
        {
            return View();
        }
    }
}