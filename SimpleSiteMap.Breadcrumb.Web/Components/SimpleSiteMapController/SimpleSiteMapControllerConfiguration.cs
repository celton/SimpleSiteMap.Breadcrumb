using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleSiteMapController
{
    public class SimpleSiteMapControllerConfiguration : IDisposable, IServiceProvider
    {
        #region Properties
        /// <summary>
        /// Define ViewData name used by html helper (Menu). "SimpleSiteMapControllerMenu" is default value.
        /// </summary>
        public string ViewSimpleSiteMapControllerMenuName { get; set; }
        /// <summary>
        /// Define ViewData name used by html helper (Breadcrumbigation). "SimpleSiteMapControllerBreadcrumb" is default value.
        /// </summary>
        public string ViewSimpleSiteMapControllerBreadcrumbName { get; set; }
        /// <summary>
        /// Define the Area parent of all; Define the first Area of the link of Breadcrumbs structure. Empty is default value.
        /// </summary>
        public string ParentAreaOfAll { get; set; }
        /// <summary>
        /// Define the Controller parent of all; Define the first Controller of the link of Breadcrumbs structure. "Home" is default value.
        /// </summary>
        public string ParentControllerOfAll { get; set; }
        /// <summary>
        /// Define the Action parent of all; Define the first Action of the link of Breadcrumbs structure. "Index" is default value.
        /// </summary>        
        public string ParentActionOfAll { get; set; }
        /// <summary>
        /// Define the label parent of all; Define the first label of the link of Breadcrumbs structure. "Home" is default value.
        /// </summary>
        public string LabelOfGrandParent { get; set; }
        #endregion

        public SimpleSiteMapControllerConfiguration()
        {
            HttpContext.Current.Application.Add("SimpleSiteMapControllerConfiguration", this);
            SetDefaultValue();
            //TODO: Create list for menu
        }

        private void SetDefaultValue()
        {
            if (string.IsNullOrWhiteSpace(ViewSimpleSiteMapControllerMenuName))
                this.ViewSimpleSiteMapControllerMenuName = "SimpleSiteMapControllerMenu";

            if (string.IsNullOrWhiteSpace(ViewSimpleSiteMapControllerBreadcrumbName))
                this.ViewSimpleSiteMapControllerBreadcrumbName = "SimpleSiteMapControllerBreadcrumb";

            if (string.IsNullOrWhiteSpace(LabelOfGrandParent))
                this.LabelOfGrandParent = "Home";

            if (string.IsNullOrWhiteSpace(ParentControllerOfAll))
                this.ParentControllerOfAll = "Home";

            if (string.IsNullOrWhiteSpace(ParentActionOfAll))
                this.ParentActionOfAll = "Index";

            this.ParentAreaOfAll = string.Empty;
        }

        public void Verify()
        {
            var type = typeof(Controller);
            var classes = AppDomain.CurrentDomain.GetAssemblies().Where(d => d != null).SelectMany(s => s.GetTypes());

            #region Checking first node
            checkRoot(classes, this.ParentAreaOfAll, this.ParentControllerOfAll, this.ParentActionOfAll);
            #endregion

            #region Check if all defined list work or if there are broke roots;
            List<SimpleSiteMapControllerAttribute> attributes = new List<SimpleSiteMapControllerAttribute>();
            var attributesNative = classes.SelectMany(s => s.GetMethods()).SelectMany(s => s.GetCustomAttributes(typeof(SimpleSiteMapControllerAttribute), false)).ToList();

            foreach (var attributeNative in attributesNative)
            {
                attributes.Add((SimpleSiteMapControllerAttribute)attributeNative);
            }

            foreach (var attribute in attributes)
                checkRoot(classes, attribute.ParentArea, attribute.ParentController, attribute.ParentAction);
            #endregion
        }
        private void checkRoot(IEnumerable<Type> classes, string area, string controller, string action)
        {
            var selectedClass = classes.Where(p => p.Name == controller + "Controller"
              && ((string.IsNullOrWhiteSpace(area) && !p.Namespace.Contains("Areas"))
                  || (!string.IsNullOrWhiteSpace(area) && p.Namespace.Contains("Areas") && p.Namespace.Contains(area)))
              ).ToList();

            if (selectedClass == null || selectedClass.Count == 0)
                if (string.IsNullOrWhiteSpace(area))
                    throw new Exception(string.Format("SimpleSiteMapController - Configurations: Controller not found - Name: {0}.", controller));
                else
                    throw new Exception(string.Format("SimpleSiteMapController - Configurations: Controller not found - Controller: {0} / Area: {1}.", controller, area));

            if (selectedClass.Count > 1)
                if (string.IsNullOrWhiteSpace(area))
                    throw new Exception(string.Format("SimpleSiteMapController - Configurations: More than one controller found. Controller: {0}", controller));
                else
                    throw new Exception(string.Format("SimpleSiteMapController - Configurations: More than one controller found. Controller: {0} / Area: {1}.", controller, area));


            if (!string.IsNullOrWhiteSpace(action))
            {
                var selectedMethod = selectedClass.First().GetMethods().Where(d => d.Name.ToLower() == action.ToLower()).ToList();

                if (selectedMethod == null || selectedMethod.Count == 0)
                    throw new Exception(string.Format("SimpleSiteMapController - Configurations: Child method not defined or found. Controller: {1} / Action: {0}", action, controller));
            }
        }

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}