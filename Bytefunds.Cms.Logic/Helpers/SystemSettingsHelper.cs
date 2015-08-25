using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Bytefunds.Cms.Logic.Helpers
{
    public class SystemSettingsHelper
    {
        public static string GetSystemSettingsByKey(string key)
        {
            ServiceContext Services = ApplicationContext.Current.Services;
            IContentType contentType = Services.ContentTypeService.GetContentType("Setting");
            IEnumerable<IContent> allSettings = Services.ContentService.GetContentOfContentType(contentType.Id).Where(c => c.Trashed.Equals(false));

            IContent setting = allSettings.FirstOrDefault(r => r.GetValue<string>("key").Equals(key));
            string value = setting.GetValue<string>("value");
            return value;
        }

    }
}