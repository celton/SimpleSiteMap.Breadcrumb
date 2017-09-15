using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleSiteMapController
{
    public class SimpleSiteMapModel
    {
        public string URL { get; set; }
        public string ShowInMenu { get; set; }
        public string Label { get; set; }

        public List<SimpleSiteMapModel> SimpleSiteMapControllerChildren { get; set; }
        public SimpleSiteMapModel SimpleSiteMapControllerParent { get; set; }
    }
}