using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcContrib.Pagination;
using MvcContrib.UI.Pager;
using System.Text;
using System.Linq.Expressions;
using System.Web.Routing;

namespace Aufen.PortalReportes.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static IHtmlString ButtonIconActionLink(this HtmlHelper htmlHelper, string icon, string buttonTooltip, string action, string controllerName, object htmlAttributes, object routeValues)
        {
            TagBuilder builder;
            UrlHelper urlHelper;
            urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            builder = new TagBuilder("a");
            builder.InnerHtml = String.Format(@"<button type=""button"" class=""btn btn-default btn-xs"" >
  <span class=""glyphicon glyphicon-{0}"" rel=""tooltip"" title=""{1}""></span>
</button>", !String.IsNullOrEmpty(icon) ? icon : "start", buttonTooltip);
            builder.Attributes["href"] = urlHelper.Action(action, controllerName, routeValues);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            return MvcHtmlString.Create(builder.ToString());
        }
    }
}