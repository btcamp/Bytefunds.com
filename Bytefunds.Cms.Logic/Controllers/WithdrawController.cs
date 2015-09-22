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
    public class WithdrawController : SurfaceController
    {
        public ActionResult Approved(int id)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                IContent content = Services.ContentService.GetById(id);
                content.SetValue("isCheck", true);
                Services.ContentService.Save(content);
                //获取memberid
                int memberid = content.GetValue<int>("memberId");
                IMember member = Services.MemberService.GetById(memberid);
                //发送审核邮件member:approved:tplid
                Dictionary<string, string> dir = new Dictionary<string, string>();
                dir.Add("{{name}}", member.Name);
                dir.Add("{{time}}", content.CreateDate.ToString("yyyy年MM月dd日 HH:mm:ss"));
                dir.Add("{{amount}}", content.GetValue<string>("amount"));
                Helpers.SendmailHelper.SendEmail(member.Username, "member:approved:tplid", dir);

                response.Success = true;
                response.Msg = "审核成功！";
            }
            catch (Exception ex)
            {
                response.Success = false;
                Common.CustomLog.WriteLog(ex.ToString());
                response.Msg = "审核失败！" + ex.Message;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}