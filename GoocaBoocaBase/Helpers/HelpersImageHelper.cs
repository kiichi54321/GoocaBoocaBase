using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GoocaBoocaBase.Helpers
{
    public static class ImageHelper
    {
        public static MvcHtmlString Image(this HtmlHelper helper, string id, string url, string alternateText)
        {
            return Image(helper, id, url, alternateText,string.Empty, null);
        }

        public static MvcHtmlString Image(this HtmlHelper helper, string id, string url, string alternateText, string className,object htmlAttributes)
        {
            // Create tag builder
            var builder = new TagBuilder("img");

            // Create valid id
            builder.GenerateId(id);

            // Add attributes
            builder.MergeAttribute("src", url);
            builder.MergeAttribute("alt", alternateText);
            builder.MergeAttribute("class", className);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            // Render tag
            return new MvcHtmlString( builder.ToString(TagRenderMode.SelfClosing));
        }

    }
}