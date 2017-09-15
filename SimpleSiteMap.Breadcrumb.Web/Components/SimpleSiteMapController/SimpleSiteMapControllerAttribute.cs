using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleSiteMap.Components.SimpleSiteMapController;

namespace SimpleSiteMapController
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SimpleSiteMapControllerAttribute : ActionFilterAttribute
    {
        private string label { get; set; }
        private bool showInMenu { get; set; }
        private string parentController { get; set; }
        private string parentAction { get; set; }
        private string parentArea { get; set; }

        public bool ShowInMenu { get { return showInMenu; } }
        public string Label { get { return label; } }
        public string ParentController { get { return parentController; } }
        public string ParentAction { get { return parentAction; } }
        public string ParentArea { get { return parentArea; } }

        public SimpleSiteMapControllerAttribute(string label, bool showInMenu = false)
        {
            this.label = label;
            this.showInMenu = showInMenu;
        }

        public SimpleSiteMapControllerAttribute(string label, string parentController, string parentAction = "Index", bool showInMenu = false)
        {
            this.label = label;
            this.parentController = parentController;
            this.parentAction = parentAction;
            this.showInMenu = showInMenu;
        }

        public SimpleSiteMapControllerAttribute(string label, string parentArea, string parentController, string parentAction, bool showInMenu = false)
        {
            this.label = label;
            this.parentArea = parentArea;
            this.parentController = parentController;
            this.parentAction = parentAction;
            this.showInMenu = showInMenu;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            UrlHelper url = new UrlHelper(filterContext.RequestContext);
            var controller = filterContext.Controller.ToString().Split('.').Last().Replace("Controller", "");
            var action = filterContext.ActionDescriptor.ActionName;
            var area = (string)filterContext.HttpContext.Request.RequestContext.RouteData.DataTokens["area"] ?? string.Empty;

            SimpleSiteMapModel simpleBreadcrumb = new SimpleSiteMapModel();
            simpleBreadcrumb.Label = label;

            simpleBreadcrumb.URL = url.Action(action, controller, new { Area = area, id = "" });

            var parent = PopulateParent(filterContext, this.parentController, this.parentArea, this.parentAction);

            //If is the first one
            simpleBreadcrumb.SimpleSiteMapControllerParent = (parent.URL == simpleBreadcrumb.URL) ? null : parent;

            var configurations = (SimpleSiteMapControllerConfiguration)filterContext.HttpContext.Application["SimpleSiteMapControllerConfiguration"];
            //var ViewData = filterContext.Controller.ViewData;
            //ViewData[configurations.ViewSimpleSiteMapControllerBreadcrumbName] = simpleBreadcrumb;
            HttpContext.Current.Items.Add(configurations.ViewSimpleSiteMapControllerBreadcrumbName, simpleBreadcrumb);
        }

        public SimpleSiteMapModel PopulateParent(ActionExecutingContext filterContext, string parentController, string parentArea, string parentAction)
        {
            if (string.IsNullOrWhiteSpace(parentController))
                return PopulateParentDefault(filterContext);

            SimpleSiteMapModel simpleSiteMapController = new SimpleSiteMapModel();

            UrlHelper url = new UrlHelper(filterContext.RequestContext);
            var controller = parentController;
            var action = parentAction;
            var area = parentArea ?? string.Empty;
            simpleSiteMapController.URL = url.Action(action, controller, new { Area = area, id = "" });

            var type = typeof(Controller);
            var selectedClass = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p)
                    && p.Name == parentController + "Controller"
                    && (
                           (
                            string.IsNullOrWhiteSpace(parentArea) && p.Namespace.Contains("Areas") == false
                           )
                        || (
                            !string.IsNullOrWhiteSpace(parentArea) && p.Namespace.Contains("Areas") && p.Namespace.Contains(parentArea)
                           )
                       )
                    ).ToList();

            if (selectedClass == null || selectedClass.Count == 0)
                throw new Exception("SimpleSiteMapController: Parent node not found.");

            if (selectedClass.Count > 1)
                throw new Exception("SimpleSiteMapController: A Child node cannot have more than one Parent.");

            var selectedMethodList = selectedClass.First().GetMethods().Where(d => d.Name.ToLower() == action.ToLower()).ToList();

            if (selectedMethodList == null || selectedMethodList.Count == 0)
                throw new Exception("SimpleSiteMapController: Child method not defined or found.");

            List<object> attributes = new List<object>();
            foreach (var selectedMethodItem in selectedMethodList)
                attributes.AddRange(selectedMethodItem.GetCustomAttributes(false));

            var attribute = attributes.Where(d => d.GetType() == typeof(SimpleSiteMapControllerAttribute)).ToList();

            if (attribute == null)
                throw new Exception("SimpleSiteMapController: Attributes not found.");

            if (attribute.Count > 1)
                throw new Exception("SimpleSiteMapController: An ActionResult method cannot have more than one SimpleSiteMapControllerAttribute");

            var attributeSelected = (SimpleSiteMapControllerAttribute)attribute.First();

            simpleSiteMapController.Label = attributeSelected.Label;

            simpleSiteMapController.SimpleSiteMapControllerParent = PopulateParent(filterContext, attributeSelected.ParentController, attributeSelected.ParentArea, attributeSelected.ParentAction);

            return simpleSiteMapController;

        }

        private SimpleSiteMapModel PopulateParentDefault(ActionExecutingContext filterContext)
        {
            UrlHelper url = new UrlHelper(filterContext.RequestContext);
            var configurations = (SimpleSiteMapControllerConfiguration)filterContext.HttpContext.Application["SimpleSiteMapControllerConfiguration"];

            return new SimpleSiteMapModel
            {
                Label = configurations.LabelOfGrandParent,
                URL = url.Action(configurations.ParentActionOfAll, configurations.ParentControllerOfAll, new { Area = (string.IsNullOrWhiteSpace(configurations.ParentAreaOfAll) ? string.Empty : configurations.ParentAreaOfAll) }),
                SimpleSiteMapControllerParent = null
            };
        }


    }
}