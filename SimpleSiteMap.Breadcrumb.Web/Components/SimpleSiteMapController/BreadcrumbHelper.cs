using SimpleSiteMapController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace System.Web.Mvc.Html
{
    public static class BreadcrumbHelper
    {
        public static IHtmlString GenerateBreadcrumb(this HtmlHelper helper)
        {
            StringBuilder htmlResult = new StringBuilder();
            StringBuilder htmlItens = new StringBuilder();
            var configurations = (SimpleSiteMapControllerConfiguration)HttpContext.Current.Application["SimpleSiteMapControllerConfiguration"];
            var simpleSiteMapModel = (SimpleSiteMapModel)HttpContext.Current.Items[configurations.ViewSimpleSiteMapControllerBreadcrumbName];
            SimpleSiteMapModel simpleSiteMapModelParent = null;

            htmlResult.Append("<ol class=\"breadcrumb\">");

            if (simpleSiteMapModel != null)
            {
                htmlItens.Append("<li>");
                htmlItens.Append(simpleSiteMapModel.Label);
                htmlItens.Append("</li>");
                simpleSiteMapModelParent = simpleSiteMapModel.SimpleSiteMapControllerParent;
            }

            while (simpleSiteMapModelParent != null)
            {
                htmlItens.Insert(0, string.Format("<li><a href=\"{0}\">{1}</a></li>", simpleSiteMapModelParent.URL, simpleSiteMapModelParent.Label));
                simpleSiteMapModelParent = simpleSiteMapModelParent.SimpleSiteMapControllerParent;
            }

            htmlResult.Append(htmlItens.ToString());
            htmlResult.Append("</ol>");

            return new HtmlString(htmlResult.ToString());
        }
    }
}