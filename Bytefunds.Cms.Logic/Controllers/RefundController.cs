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
    public class RefundController : SurfaceController
    {
        public ActionResult Approved(int? id)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (id == null)
                {
                    response.Success = false;
                    response.Msg = "非法请求";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
                IContent content = Services.ContentService.GetById(id.Value);
                content.SetValue("isOk", true);
                int chipsid = content.GetValue<int>("chipsid");
                IContent chipsContent = Services.ContentService.GetById(chipsid);
                chipsContent.SetValue("isOk", true);
                Services.ContentService.Save(new List<IContent>() { content, chipsContent });
                response.Success = true;
                response.Msg = "已经成功通过审核！";
                //TODO发送短信通知
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Msg = ex.Message;
                return Json(response, JsonRequestBehavior.AllowGet);
            }
        }
    }
}