using Bytefunds.Cms.Logic.Models;
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
    public class PraiseController : SurfaceController
    {
        public ActionResult Chips()
        {
            ResponseModel response = new ResponseModel();
            if (!Members.IsLoggedIn())
            {
                response.Success = false;
                response.Msg = "请先登录！";
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            int id = Members.GetCurrentMemberId();

            IContentType ct = Services.ContentTypeService.GetContentType("PraiseDocument");
            IContent content = Services.ContentService.GetContentOfContentType(ct.Id).FirstOrDefault(e => e.GetValue<int>("member") == id);
            if (content != null)
            {
                response.Success = false;
                response.Msg = "您已经赞过啦！";
            }
            else
            {
                content = Services.ContentService.CreateContent(Guid.NewGuid().ToString(), ct.Id, "PraiseDocument");
                content.SetValue("member", id.ToString());
                Services.ContentService.Save(content);
                response.Success = true;
                response.Msg = "非常感谢您的支持！";
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}