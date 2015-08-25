using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models;

namespace System
{
    public static class UmbracoExtension
    {
        public static MvcHtmlString GetbreadcrumbPath(this Umbraco.Web.UmbracoHelper helper, IPublishedContent content)
        {
            // <li><i class="fa fa-home pr-10"></i><a href="index.html">Home</a></li>
            //<li class="active">Page Services 2</li>
            return null;
        }

        public static string ReomveHtmlAttribute(this string str)
        {
            Regex regex = new Regex(@"<.+?>");
            return regex.Replace(str, "");
        }
    }
}