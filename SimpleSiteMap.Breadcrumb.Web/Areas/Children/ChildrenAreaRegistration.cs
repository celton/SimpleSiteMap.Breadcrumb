using System.Web.Mvc;

namespace SimpleSiteMap.Breadcrumb.Web.Areas.Children
{
    public class ChildrenAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Children";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Children_default",
                "Children/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}