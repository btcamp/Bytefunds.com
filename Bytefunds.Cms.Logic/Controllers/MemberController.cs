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
using System.Text;
using System.Web.Configuration;
using System.Net.Configuration;
using System.Configuration;
using Bytefunds.Cms.Logic.Helpers;
using Umbraco.Core.Services;

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

            string code = Session["code"] == null ? string.Empty : Session["code"].ToString();
            if (code != model.Code)
            {
                result.Success = false;
                result.Msg = "验证码错误，请重新获取验证码";
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            IMember validatemember = Services.MemberService.GetMembersByPropertyValue("tel", model.Phone).FirstOrDefault();

            if (validatemember != null)
            {
                result.Success = false;
                result.Msg = "该手机号已经注册，请用手机号进行登录";
                result.IsRedirect = false;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
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
                        m.SetValue("fundAccount", "5000");
                        Services.MemberService.SavePassword(m, model.Password);
                        Services.MemberService.Save(m);
                        result.Success = true;
                        result.Msg = "欢迎您的加入，您成功注册，我们额外赠送5000元定期宝一个月";
                        result.IsRedirect = true;
                        result.RedirectUrl = "/memberinfo?show=true";
                        EventHandlers.CustomRaiseEvent.RaiseRegistered(m);
                        //赠送5000元定期宝一月期
                        System.Threading.Tasks.Task.Factory.StartNew((ser) =>
                        {
                            ServiceContext sc = ser as ServiceContext;
                            IContentType ct = sc.ContentTypeService.GetContentType("PayRecords");

                            IContent content = sc.ContentService.CreateContent(m.Name + "赠送定期宝", ct.Id, "PayRecords");
                            content.SetValue("username", m.Name);
                            content.SetValue("email", m.Username);
                            content.SetValue("amountCny", 5000);
                            content.SetValue("mobilePhone", m.GetValue<string>("tel"));
                            content.SetValue("rechargeDateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            content.SetValue("expirationtime", DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd HH:mm:ss"));
                            content.SetValue("memberPicker", m.Id);
                            content.SetValue("payBillno", "注册赠送的定期宝");
                            content.SetValue("isdeposit", true);
                            content.SetValue("isexpired", false);
                            content.SetValue("buyproduct", 2337);
                            content.SetValue("isGive", true);
                            sc.ContentService.Save(content);
                            EventHandlers.CustomRaiseEvent.RaiseContentCreated(content);
                        }, Services);

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
                    result.RedirectUrl = "memberinfo";
                    //清空登陆失败次数
                    member.FailedPasswordAttempts = 0;
                    Services.MemberService.Save(member);
                }
            }
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

        public ActionResult Withdraw([Bind(Prefix = "withdrawModel")]WithdrawViewModel model)
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

                if (!member.GetValue<bool>("verified"))
                {
                    responseModel.Msg = "请先进行实名认证，方可进行提现申请";
                    responseModel.Success = false;
                    responseModel.RedirectUrl = "/memberinfo/verified";
                }
                else
                {

                    IContentType ct = Services.ContentTypeService.GetContentType("WithdrawElement");
                    IContent content = Services.ContentService.CreateContent(model.Name + "[" + model.Amount + "]", ct.Id, "WithdrawElement");
                    content.SetValue("memberId", Members.GetCurrentMemberId().ToString());
                    content.SetValue("amount", model.Amount.ToString());
                    content.SetValue("memberName", model.Name);
                    content.SetValue("bankName", model.BankName);
                    content.SetValue("bankNumber", model.BankNumber);
                    content.SetValue("bankDetail", model.BankDetail);
                    content.SetValue("isCheck", false);
                    content.SetValue("okassets", Members.GetCurrentMember()["okassets"]);
                    Services.ContentService.Save(content);
                    EventHandlers.CustomRaiseEvent.RaiseContentCreated(content);
                    responseModel.Msg = "恭喜你！已经成功提交提现申请";
                    responseModel.Success = true;
                }
            }
            return Json(responseModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidatePhone(string phone)
        {
            IMember member = Services.MemberService.GetMembersByPropertyValue("tel", phone).FirstOrDefault();
            bool flg = member == null;
            return Json(flg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RenewalFund([Bind(Prefix = "renewalViewModel")]RenewalFundViewModel model)
        {
            ResponseModel response = new ResponseModel();
            IMember member = Services.MemberService.GetById(Members.GetCurrentMemberId());

            //旧的交易记录
            IContent oldContent = Services.ContentService.GetById(model.CurrentPayId);
            //旧的产品
            IContent oldProduct = Services.ContentService.GetById(oldContent.GetValue<int>("buyproduct"));
            if (model.RenewalFundId == -1)
            {
                response.Success = true;
                response.Msg = "恭喜您已经成功解约，到期将余额返回到您账户余额";
                response.RedirectUrl = "/memberinfo";
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    //member:ter:tplid
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("{{name}}", member.Name);
                    dic.Add("{{product}}", oldProduct.GetValue<string>("title"));
                    dic.Add("{{start}}", oldContent.GetValue<DateTime>("rechargeDateTime").ToString("yyyy-MM-dd HH:mm:ss"));
                    dic.Add("{{end}}", oldContent.GetValue<DateTime>("expirationtime").ToString("yyyy-MM-dd HH:mm:ss"));
                    Helpers.SendmailHelper.SendEmail(member.Username, "member:ter:tplid", dic);
                    dic.Add("{{option}}", "到期解约");
                    string manageremail = SystemSettingsHelper.GetSystemSettingsByKey("manager:email");
                    Helpers.SendmailHelper.SendEmail(manageremail, "manager:ter:tplid", dic);
                });
            }
            else
            {
                //续约的产品
                IContent product = Services.ContentService.GetById(model.RenewalFundId);
                //新的交易记录
                DateTime start = oldContent.GetValue<DateTime>("expirationtime").AddDays(1);
                DateTime end = start.AddMonths(product.GetValue<int>("cycle"));
                IContentType ct = ApplicationContext.Services.ContentTypeService.GetContentType("PayRecords");
                IContent content = ApplicationContext.Services.ContentService.CreateContent("无用户名", ct.Id, "PayRecords");

                content.SetValue("username", member.Name);
                content.Name = member.Email;
                content.SetValue("email", member.Email);
                content.SetValue("memberPicker", member.Id);
                content.SetValue("buyproduct", product.Id);
                content.SetValue("mobilePhone", member.GetValue<string>("tel"));
                content.SetValue("memberPicker", member.Id.ToString());
                content.SetValue("amountCny", oldContent.GetValue<string>("amountCny"));
                content.SetValue("rechargeDateTime", start.ToString("yyyy-MM-dd HH:mm:ss"));
                content.SetValue("expirationtime", end.ToString("yyyy-MM-dd HH:mm:ss"));
                content.SetValue("payBillno", "续约订单：" + oldContent.GetValue<string>("payBillno"));
                content.SetValue("isdeposit", true);
                content.SetValue("isexpired", false);
                Services.ContentService.Save(content);

                response.Success = true;
                response.Msg = "恭喜你已经完成自动续约！请查收邮件";
                response.IsRedirect = true;
                response.RedirectUrl = "/memberinfo";
                //发送邮件
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    //member:renewal:tplid
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("{{name}}", member.Name);
                    dic.Add("{{oldproduct}}", oldProduct.GetValue<string>("title"));
                    dic.Add("{{newproduct}}", product.GetValue<string>("title"));
                    dic.Add("{{start}}", start.ToString("yyyy-MM-dd HH:mm:ss"));
                    dic.Add("{{end}}", end.ToString("yyyy-MM-dd HH:mm:ss"));
                    Helpers.SendmailHelper.SendEmail(member.Username, "member:renewal:tplid", dic);
                    dic.Add("{{option}}", "自动续约");
                    string manageremail = SystemSettingsHelper.GetSystemSettingsByKey("manager:email");
                    Helpers.SendmailHelper.SendEmail(manageremail, "manager:renewal:tplid", dic);
                });
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Transferred([Bind(Prefix = "transferredViewModel")]TransferredViewModel model)
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

                if (member.GetValue<double>("assets") < model.Amount)
                {
                    responseModel.Success = false;
                    responseModel.Msg = "购买的金额超限，您账户余额是：" + member.GetValue<double>("assets").ToString("N2") + "元";
                    return Json(responseModel, JsonRequestBehavior.AllowGet);
                }


                //新增产品数据
                IContentType ct = ApplicationContext.Services.ContentTypeService.GetContentType("PayRecords");
                IContent content = ApplicationContext.Services.ContentService.CreateContent("无用户名", ct.Id, "PayRecords");

                IContent product = Services.ContentService.GetById(model.ProductId);
                DateTime start = DateTime.Now, end = start.AddMonths(product.GetValue<int>("cycle"));
                content.SetValue("username", member.Name);
                content.Name = member.Email;
                content.SetValue("email", member.Email);
                content.SetValue("memberPicker", member.Id);
                content.SetValue("buyproduct", product.Id);
                content.SetValue("mobilePhone", member.GetValue<string>("tel"));
                content.SetValue("memberPicker", member.Id.ToString());
                content.SetValue("amountCny", model.Amount.ToString());
                content.SetValue("rechargeDateTime", start.ToString("yyyy-MM-dd HH:mm:ss"));
                content.SetValue("expirationtime", end.ToString("yyyy-MM-dd HH:mm:ss"));
                content.SetValue("payBillno", "账户余额金额购买");
                content.SetValue("isdeposit", true);
                content.SetValue("isexpired", false);
                Services.ContentService.Save(content);
                EventHandlers.CustomRaiseEvent.RaiseContentCreated(content);
                decimal assets = member.GetValue<decimal>("assets"), okassets = member.GetValue<decimal>("okassets"), fundAccount = member.GetValue<decimal>("fundAccount");
                assets = assets - (decimal)model.Amount;

                if (assets < okassets)
                {
                    //如果 可提现金额小于已经转出后的余额 也应扣款
                    member.SetValue("okassets", assets.ToString());
                }
                member.SetValue("assets", assets.ToString());
                member.SetValue("fundAccount", (fundAccount + (decimal)model.Amount).ToString());
                Services.MemberService.Save(member);

                responseModel.Success = true;
                responseModel.Msg = "恭喜你成功转入到定期宝！";
                responseModel.RedirectUrl = "/memberinfo";
            }
            return Json(responseModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCode(string email)
        {
            try
            {
                //member:register:code:tplid
                Random rand = new Random();
                int num = rand.Next(100000, 999999);
                Session["code"] = num.ToString();
                Dictionary<string, string> dir = new Dictionary<string, string>();
                dir.Add("{{code}}", num.ToString());
                Helpers.SendmailHelper.SendEmail(email, "member:register:code:tplid", dir);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Test()
        {
            //获取所有已经购买的产品
            //IContentType ct = Services.ContentTypeService.GetContentType("PayRecords");
            //IEnumerable<IContent> list = Services.ContentService.GetContentOfContentType(ct.Id).Where(e => e.GetValue<bool>("isdeposit") == true && e.GetValue<bool>("isexpired") == false);
            //IEnumerable<int> memberids = list.GroupBy(e => e.GetValue<int>("memberPicker")).Select(e => e.Key);
            //foreach (int memberid in memberids)
            //{
            //    decimal sumProfit = 0;
            //    IMember member = Services.MemberService.GetById(memberid);
            //    StringBuilder sb = new StringBuilder();
            //    //获取product的收益率
            //    foreach (IContent content in list.Where(e => e.GetValue<int>("memberPicker") == memberid))
            //    {
            //        //yyyy-MM-dd
            //        if (content.GetValue<DateTime>("expirationtime") > DateTime.Now.AddDays(1))
            //        {
            //            int productId = content.GetValue<int>("buyproduct");
            //            IContent product = Services.ContentService.GetById(productId);
            //            int number = 0;
            //            if (int.TryParse(product.GetValue<string>("rate"), out number))
            //            {
            //                decimal rate = (decimal)number / (decimal)100;
            //                decimal profit = (content.GetValue<decimal>("amountCny") * rate) / (decimal)365;
            //                //修改member的最新收益
            //                sumProfit += profit;
            //                //记录这次计算结果；
            //                string earningsName = string.Format("{0} ({1})", content.GetValue<string>("username"), profit.ToString("F2"));
            //                IContentType earningsContentType = Services.ContentTypeService.GetContentType("EarningsRecordsElement");
            //                IContent earningsContent = Services.ContentService.CreateContent(earningsName, earningsContentType.Id, "EarningsRecordsElement");
            //                earningsContent.SetValue("memberid", memberid);
            //                earningsContent.SetValue("productid", content.Id);
            //                earningsContent.SetValue("earning", profit.ToString("f2"));
            //                Services.ContentService.Save(earningsContent);
            //                sb.AppendFormat("<p style='orphans: 2; widows: 2; padding: 1px 34px 21px; margin: 0px;'><span style='font-size: 14px; line-height: 21px;'>您购买的产品《{0}》投入了{1}元；昨日的收益是：{2}元</span></p>", product.GetValue<string>("title"), content.GetValue<decimal>("amountCny").ToString("N2"), profit.ToString("N2"));
            //            }
            //        }
            //        else
            //        {
            //            //到期的 将余额提出到账户余额中
            //            decimal assets = member.GetValue<decimal>("assets");
            //            member.SetValue("assets", (assets + content.GetValue<decimal>("amountCny")).ToString("f2"));
            //            Services.MemberService.Save(member);
            //            //修改基金到期
            //            content.SetValue("isexpired", true);
            //            Services.ContentService.Save(content);
            //        }
            //    }

            //    member.SetValue("latestearnings", sumProfit.ToString("f2"));
            //    decimal accumulatedEarnings = member.GetValue<decimal>("accumulatedEarnings");
            //    member.SetValue("accumulatedEarnings", (accumulatedEarnings + sumProfit).ToString("f2"));
            //    Services.MemberService.Save(member);
            //    //发送邮件通知
            //    if (sb.Length > 0)
            //    {
            //        sb.AppendFormat("<p style='orphans: 2; widows: 2; padding: 1px 34px 21px; margin: 0px;'><span style='font-size: 14px; line-height: 21px;'>您购买的所有产品总收益：{0}</span></p>", sumProfit.ToString("N2"));
            //        //获取收益通知邮件模板
            //        Dictionary<string, string> dic = new Dictionary<string, string>();
            //        dic.Add("{{msg}}", sb.ToString());
            //        dic.Add("{{name}}", member.Name);
            //        Helpers.SendmailHelper.SendEmail(member.Username, "member:newprofit", dic);
            //    }
            //}
            return Content("test");
        }
        public ActionResult Clear()
        {
            IContentType ct = Services.ContentTypeService.GetContentType("EarningsRecordsElement");
            IEnumerable<IContent> result = Services.ContentService.GetContentOfContentType(ct.Id);
            foreach (var item in result)
            {
                //Services.ContentService.Delete(item);
                string str = string.Empty;
            }
            return Content("OK");
        }

        public ActionResult GetProfitsByProductId(int id)
        {
            int memberId = 0;
            EarninisRecordsViewModel model = new EarninisRecordsViewModel();
            if (int.TryParse(SystemSettingsHelper.GetSystemSettingsByKey("show:accountId"), out memberId))
            {
                IContentType ct = Services.ContentTypeService.GetContentType("EarningsRecordsElement");
                IEnumerable<IContent> result = Services.ContentService.GetContentOfContentType(ct.Id)
                                                .Where(e => e.GetValue<int>("memberId") == memberId)
                                                .Where(e => e.GetValue<int>("productid") == id);
                if (result != null && result.Count() > 0)
                {
                    var groupkeys = result.OrderByDescending(e => e.CreateDate)
                                               .GroupBy(e => e.CreateDate.ToString("MM/dd"))
                                               .Select(e => new
                                               {
                                                   Key = e.Key,
                                                   Value = e.Sum(a => a.GetValue<decimal>("earning"))
                                               });

                    foreach (var item in groupkeys.OrderBy(e => Convert.ToDateTime(e.Key)))
                    {
                        model.Times.Add(item.Key);
                        model.Datas.Add(item.Value);
                    }
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProfits()
        {
            EarninisRecordsViewModel model = new EarninisRecordsViewModel();
            if (Members.IsLoggedIn())
            {
                IContentType ct = Services.ContentTypeService.GetContentType("EarningsRecordsElement");
                IEnumerable<IContent> result = Services.ContentService.GetContentOfContentType(ct.Id).Where(e => e.GetValue<int>("memberId") == Members.GetCurrentMemberId());

                if (result != null && result.Count() > 0)
                {
                    var groupkeys = result.OrderByDescending(e => e.CreateDate)
                                               .GroupBy(e => e.CreateDate.ToString("MM/dd"))
                                               .Take(7)
                                               .Select(e => new
                                               {
                                                   Key = e.Key,
                                                   Value = e.Sum(a => a.GetValue<decimal>("earning"))
                                               });

                    foreach (var item in groupkeys.OrderBy(e => Convert.ToDateTime(e.Key)))
                    {
                        model.Times.Add(item.Key);
                        model.Datas.Add(item.Value);
                    }
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}