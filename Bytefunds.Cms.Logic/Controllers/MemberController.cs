using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;
using Bytefunds.Cms.Logic.Models;
using Umbraco.Core.Models;

namespace Bytefunds.Cms.Logic.Controllers
{
    [PluginController("Api")]
    public class MemberController : SurfaceController
    {
        public MemberController()
        {
            //IMediaService ms = Services.MediaService;
            //if (depositImage != null && depositImage.ContentLength > 0)
            //{
            //    IMedia mediaImage = ms.CreateMedia(realName + "(" + amountUsd + "$ / " + rechargeDateTime.ToString("yyyy-MM-dd HH:mm:ss") + ")", 5121, "Image");

            //    mediaImage.SetValue("umbracoFile", depositImage);
            //    ms.Save(mediaImage);
            //}
        }
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
                        Services.MemberService.Save(m);
                        result.Success = true;
                        result.Msg = "欢迎您的加入";
                        result.IsRedirect = true;
                        result.RedirectUrl = "/memberinfo";
                        EventHandlers.CustomRaiseEvent.RaiseRegistered(m);
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
            //var member = Services.MemberService.GetByEmail(model.Email);
            IMember member = Services.MemberService.GetMembersByPropertyValue("tel", model.Phone).FirstOrDefault();
            if (member == null)
                result.Msg = "该用户未注册，请先进行注册在登录";
            else
            {
                if (Members.Login(member.Email, model.Password) == false)
                {
                    result.Msg = "登陆手机号或密码错误，请重新输入";
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

        public ActionResult Reservation([Bind(Prefix = "reservationModel")]ReservationViewModel model)
        {
            try
            {
                string subject = string.Format("{0}-{1}-{2}-{3}-预约", model.Name, model.Phone, model.Email, DateTime.Now.ToString("yyyyMMddHHmmss"));
                umbraco.library.SendMail("jiangchun1320@163.com", "jiangchun1320@qq.com", subject, model.Text, true);
                ResponseModel responseModel = new ResponseModel();
                responseModel.Success = true;
                responseModel.Msg = "恭喜你预约成功！请等待我们的客户与您联系！";
                responseModel.IsRedirect = false;
                return Json(responseModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public ActionResult BankCertification([Bind(Prefix = "bankCertificationViewModel")] BankCertificationViewModel model)
        {
            ResponseModel responseModel = new ResponseModel();
            if (!Members.IsLoggedIn())
            {
                responseModel.Success = false;
                responseModel.Msg = "请先进行登录过在进行补充信息";
            }
            else if (!ModelState.IsValid)
            {
                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Count > 0)
                    {
                        responseModel.Success = false;
                        responseModel.Msg = item.Value.Errors.FirstOrDefault().ErrorMessage;
                    }
                }
            }
            else
            {
                IMember member = Services.MemberService.GetById(Members.GetCurrentMemberId());
                member.SetValue("bankName", model.BankName);
                member.SetValue("bankAccountName", model.BankAccountName);
                member.SetValue("bankCardNumber", model.BankCardNumber);
                member.SetValue("bankCertification", true);
                Services.MemberService.Save(member);
                responseModel.Success = true;
                responseModel.Msg = "保存成功，感谢您的支持，我们将会做的更好！";
                responseModel.RedirectUrl = "/memberinfo";
            }
            return Json(responseModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Verified([Bind(Prefix = "verifiedViewModel")]VerifiedViewModel model)
        {
            ResponseModel responseModel = new ResponseModel();
            if (!Members.IsLoggedIn())
            {
                responseModel.Success = false;
                responseModel.Msg = "请先进行登录过在进行补充信息";
            }
            else if (!ModelState.IsValid)
            {
                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Count > 0)
                    {
                        responseModel.Success = false;
                        responseModel.Msg = item.Value.Errors.FirstOrDefault().ErrorMessage;
                    }
                }
            }
            else
            {
                IMember member = Services.MemberService.GetById(Members.GetCurrentMemberId());
                member.SetValue("idcard", model.IDCard);

                if (model.Card1 != null && model.Card2 != null)
                {
                    IMedia media1 = Services.MediaService.CreateMedia(string.Format("{0}_{1}", model.IDCard, 1), 4417, "Image");
                    IMedia media2 = Services.MediaService.CreateMedia(string.Format("{0}_{1}", model.IDCard, 2), 4417, "Image");
                    media1.SetValue("umbracoFile", model.Card1);
                    media2.SetValue("umbracoFile", model.Card2);
                    List<IMedia> list = new List<IMedia>() { media1, media2 };
                    Services.MediaService.Save(list);

                    member.SetValue("card1", media1.Id);
                    member.SetValue("card2", media2.Id);
                    Services.MemberService.Save(member);
                    responseModel.Success = true;

                    responseModel.Msg = "保存成功，我们将在24小时内进行审核！";
                    responseModel.RedirectUrl = "/memberinfo";
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        //发送邮件通知管理人
                        umbraco.library.SendMail("jiangchun1320@163.com", "jiangchun1320@qq.com", "用户实名认证提交", string.Format("{0}提交了身份证信息！请尽快审核！", member.Name), true);
                    });
                }
                else
                {
                    responseModel.Success = false;
                    responseModel.Msg = "请选择身份证正反面照！";
                }
            }
            return Json(responseModel, JsonRequestBehavior.AllowGet);
        }
    }
}