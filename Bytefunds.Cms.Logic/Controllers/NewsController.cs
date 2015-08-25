using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;

namespace Bytefunds.Cms.Logic.Controllers
{
    [PluginController("Api")]
    public class NewsController : SurfaceController
    {
        //
        // GET: /News/

        public ActionResult Get(int key)
        {
            IContent content = ApplicationContext.Services.ContentService.GetById(key);
            return Json(new { title = content.GetValue<string>("title"), content = content.GetValue<string>("content"), id = content.Id }, JsonRequestBehavior.AllowGet);
        }

    }
}
