using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web.Models;
using Umbraco.Web;
 
namespace Bytefunds.Cms.Logic.Models
{
    public class News : RenderModel
    {
        public News() : base(UmbracoContext.Current.PublishedContentRequest.PublishedContent) { }
        public string title { get; set; }
        public string Name { get; set; }
        public string bodyText { get; set; }
        public string Url { get; set; }

        public DateTime createDate { get; set; }
    }
}