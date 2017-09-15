using SimpleSiteMapController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleSiteMap.Components.SimpleSiteMapController
{
    public class SimpleSiteMapControllerContainer
    {
        public static SimpleSiteMapControllerConfiguration Container;

        public static void Start()
        {
            Container = new SimpleSiteMapControllerConfiguration();
            Container.LabelOfGrandParent = "Início";
            Container.Verify();
        }
    }
}