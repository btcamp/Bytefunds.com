using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Umbraco.Web
{
  public static class UmbracoExtension
  {
    public static string GetUrl(this Umbraco.Web.UmbracoHelper helper, string root, string url)
    {
      return root + url;
    }

    public static Umbraco.Core.Models.IPublishedContent GetContentRoot(this Umbraco.Web.UmbracoHelper helper, IPublishedContent Model, IEnumerable<IPublishedContent> rootContents)
    {
      var topRoot = Model.Parent;
      while (rootContents.Any(o => o.Id == topRoot.Id) == false)
      {
        topRoot = topRoot.Parent;
      }
      return topRoot;
    }

    public static IEnumerable<ILanguage> GetAllLanguage(this Umbraco.Web.UmbracoHelper helper)
    {
      return ApplicationContext.Current.Services.LocalizationService.GetAllLanguages();
    }

  }
}