using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Bytefunds.Cms.Logic.Models;

namespace Bytefunds.Cms.Logic.Controllers
{
    [PluginController("Api")]
    public class MemberController : SurfaceController
    {
        // GET: Member
        [HttpPost]
        public ActionResult Regsiter([Bind(Prefix = "registerModel")]MemberRegisterViewModel model)
        {
            ResponseModel result = new ResponseModel();
            RegisterModel registerModel = RegisterModel.CreateModel();
            registerModel.Email = model.Email;
            registerModel.Password = model.Password;
            registerModel.Name = model.Name;

            MembershipCreateStatus status;
            //注册用户
            var member = Members.RegisterMember(registerModel, out status, true);
            switch (status)
            {
                case MembershipCreateStatus.DuplicateEmail:
                case MembershipCreateStatus.DuplicateProviderUserKey:
                case MembershipCreateStatus.DuplicateUserName:
                    {
                        result.Success = false;
                        result.Msg = "用户重复，请重新注册！";
                        break;
                    }
                case MembershipCreateStatus.InvalidAnswer:
                    break;
                case MembershipCreateStatus.InvalidEmail:
                    {
                        result.Success = false;
                        result.Msg = "邮箱格式不正确请重新输入！";
                        break;
                    }
                case MembershipCreateStatus.InvalidPassword:
                    break;
                case MembershipCreateStatus.InvalidProviderUserKey:
                    break;
                case MembershipCreateStatus.InvalidQuestion:
                    break;
                case MembershipCreateStatus.InvalidUserName:
                    break;
                case MembershipCreateStatus.ProviderError:
                    break;
                case MembershipCreateStatus.Success:
                    {
                        var m = Services.MemberService.GetByUsername(member.UserName);
                        m.SetValue("tel", model.Phone);
                        Services.MemberService.SavePassword(m, model.Password);
                        result.Success = true;
                        result.Msg = "欢迎您的加入";
                        break;
                    }

                case MembershipCreateStatus.UserRejected:
                    break;
                default:
                    break;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Login([Bind(Prefix = "loginModel")]MemberLoginViewModel model)
        {
            ResponseModel result = new ResponseModel();
            var member = Services.MemberService.GetByEmail(model.Email);
            if (Members.Login(model.Email, model.Password) == false)
            {
                if (member == null)
                    result.Msg = "该用户未注册，请先进行注册在登录";
            }
            else
            {
                HttpCookie cookie = Request.Cookies.Get(FormsAuthentication.FormsCookieName);
                cookie.Expires = DateTime.Now.AddDays(7);
                Request.Cookies.Add(cookie);
                result.Success = false;
                result.Success = true;
                result.Msg = "用户登陆成功，页面即将跳转";
                //清空登陆失败次数
                member.FailedPasswordAttempts = 0;
                Services.MemberService.Save(member);
            }

            result.RedirectUrl = "/";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            if (Members.IsLoggedIn())
            {
                Members.Logout();
            }
            ResponseModel model = new ResponseModel();
            model.Success = true;
            model.Msg = "成功退出登录！";
            model.RedirectUrl = "/";
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}