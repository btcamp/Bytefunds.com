using Bytefunds.Cms.Logic.Common;
using Bytefunds.Cms.Logic.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;

namespace Bytefunds.Cms.Logic.Controllers
{
    [PluginController("Api")]
    public class PaymentController : SurfaceController
    {
        public ActionResult PaySuccess(string Content, bool isServerCall = false)
        {

            string temp = Content;
            try
            {
                Content = DESHelper.DecryptDES(Content);
                CustomLog.WriteLog(Content);
                temp = Content;
                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                PaymentModel payModel = jss.Deserialize<Models.PaymentModel>(Content);

                //判断订单是否存在
                if (Helpers.UmbPayRecordsHelper.IsExistsBillno(payModel.Billno))
                {
                    IContent content = null;
                    IMember currentMember = null;
                    if (payModel.Email.Contains("^_^"))
                    {
                        string[] array = payModel.Email.Split(new string[] { "^_^" }, StringSplitOptions.RemoveEmptyEntries);

                        currentMember = Services.MemberService.GetByEmail(array[0]);
                        content = Services.ContentService.GetById(int.Parse(array[1]));
                        if (currentMember != null)
                        {
                            content.SetValue("username", currentMember.Name);
                            content.Name = currentMember.Email;
                            content.SetValue("email", currentMember.Email);
                            content.SetValue("mobilePhone", currentMember.GetValue<string>("tel"));
                            content.SetValue("memberPicker", currentMember.Id.ToString());
                            //保存member的信息
                            double assets = currentMember.GetValue<double>("assets");
                            double fundAccount = currentMember.GetValue<double>("fundAccount");
                            currentMember.SetValue("assets", "0");
                            currentMember.SetValue("fundAccount", (fundAccount + payModel.Amount).ToString("f2"));
                            Services.MemberService.Save(currentMember);
                        }
                        //cny 
                        content.SetValue("amountCny", payModel.Amount.ToString());
                        content.SetValue("rechargeDateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        content.SetValue("payBillno", payModel.Billno);
                        content.SetValue("amountCny", payModel.Amount.ToString());
                        content.SetValue("rechargeDateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        content.SetValue("isdeposit", true);
                        IContent product = Services.ContentService.GetById(content.GetValue<int>("buyproduct"));
                        int months = product.GetValue<int>("cycle");
                        content.SetValue("expirationtime", DateTime.Now.AddMonths(months).ToString("yyyy-MM-dd HH:mm:ss"));
                        Services.ContentService.Save(content);
                        //触发创建事件
                        EventHandlers.CustomRaiseEvent.RaiseContentCreated(content);

                        //赠送5000元定期宝一月期
                        System.Threading.Tasks.Task.Factory.StartNew((ser) =>
                        {
                            try
                            {
                                ServiceContext sc = ser as ServiceContext;
                                int num = payModel.Amount >= 5000 ? 5000 : 1000;
                                IContentType ct = sc.ContentTypeService.GetContentType("PayRecords");

                                IContent createContent = sc.ContentService.CreateContent(currentMember.Name + "赠送定期宝", ct.Id, "PayRecords");
                                createContent.SetValue("username", currentMember.Name);
                                createContent.SetValue("email", currentMember.Username);
                                createContent.SetValue("amountCny", num);
                                createContent.SetValue("mobilePhone", currentMember.GetValue<string>("tel"));
                                createContent.SetValue("rechargeDateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                createContent.SetValue("expirationtime", DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd HH:mm:ss"));
                                createContent.SetValue("memberPicker", currentMember.Id);
                                createContent.SetValue("payBillno", payModel.Billno + ":购买产品赠送的定期宝");
                                createContent.SetValue("isdeposit", true);
                                createContent.SetValue("isexpired", false);
                                createContent.SetValue("buyproduct", 2337);
                                createContent.SetValue("isGive", true);
                                sc.ContentService.Save(createContent);
                                EventHandlers.CustomRaiseEvent.RaiseContentCreated(createContent);
                                CustomLog.WriteLog("赠送成功！");
                            }
                            catch (Exception ex)
                            {
                                CustomLog.WriteLog(ex.ToString());
                            }
                        }, Services);
                        CustomLog.WriteLog("success!!");
                        return Json("ok", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        try
                        {
                            //充值到余额
                            IContentType ct = Services.ContentTypeService.GetContentType("DepositRecords");
                            IContent depositcontent = Services.ContentService.CreateContent(payModel.Email, ct.Id, "DepositRecords");
                            currentMember = Services.MemberService.GetByEmail(payModel.Email);
                            depositcontent.SetValue("username", payModel.Email);
                            depositcontent.Name = payModel.Email;
                            depositcontent.SetValue("email", payModel.Email);
                            depositcontent.SetValue("mobilePhone", payModel.Phone);
                            depositcontent.SetValue("amountCny", payModel.Amount.ToString());
                            depositcontent.SetValue("payBillno", payModel.Billno);
                            depositcontent.SetValue("memberPicker", currentMember.Id.ToString());
                            decimal assets = currentMember.GetValue<decimal>("okassets");
                            currentMember.SetValue("okassets", (assets + (decimal)payModel.Amount).ToString());
                            Services.ContentService.Save(depositcontent);
                            Services.MemberService.Save(currentMember);
                            return Json("ok", JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception ex)
                        {
                            CustomLog.WriteLog("1:" + ex.StackTrace);
                            CustomLog.WriteLog("2:" + ex.ToString());
                            return Json("ok", JsonRequestBehavior.AllowGet);
                        }
                    }

                }
                else
                {
                    CustomLog.WriteLog("重复提交" + payModel.Billno);
                    return new ContentResult() { Content = "重复提交" };
                }

            }
            catch (Exception ex)
            {
                CustomLog.WriteLog(ex.ToString());
                return new ContentResult() { Content = "fail" + temp + ex };
            }
        }

        public ActionResult ChipsSuccess(string Content, bool isServerCall = false)
        {
            string temp = Content;
            try
            {
                Content = DESHelper.DecryptDES(Content);
                CustomLog.WriteLog(Content);
                temp = Content;
                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                PaymentModel payModel = jss.Deserialize<Models.PaymentModel>(Content);

                //判断订单是否存在
                if (Helpers.UmbPayRecordsHelper.IsExistsBillno(payModel.Billno))
                {
                    IContent content = null;
                    IMember currentMember = null;
                    if (payModel.Email.Contains("^_^"))
                    {
                        string[] array = payModel.Email.Split(new string[] { "^_^" }, StringSplitOptions.RemoveEmptyEntries);
                        //memberid
                        currentMember = Services.MemberService.GetById(int.Parse(array[0]));
                        //content = Services.ContentService.GetById(int.Parse(array[1]));
                        //chips:pay:successurl
                        if (currentMember != null)
                        {
                            IContentType ct = Services.ContentTypeService.GetContentType("Chipsdepositdocument");
                            string title = string.Format("比特公寓成都万达城店[{0}]", payModel.Amount);
                            content = Services.ContentService.CreateContent(currentMember.Name + "_" + title, ct.Id, "Chipsdepositdocument");
                            content.SetValue("member", currentMember.Id.ToString());
                            content.SetValue("orderid", payModel.Billno);
                            content.SetValue("chipsProduct", title);
                            content.SetValue("amount", payModel.Amount.ToString());
                            content.SetValue("username", currentMember.GetValue<string>("tel") + "_" + payModel.Amount);
                            content.SetValue("isRefund", false);
                            content.SetValue("isOk", false);
                            Services.ContentService.Save(content);
                            //触发创建事件
                            //EventHandlers.CustomRaiseEvent.RaiseContentCreated(content);
                            CustomLog.WriteLog("success!!");
                        }
                        else
                        {
                            CustomLog.WriteLog("不存在此用户!! memberid :" + array[0]);
                        }
                    }
                    else
                    {

                    }
                    return Json("ok", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    CustomLog.WriteLog("重复提交" + payModel.Billno);
                    return new ContentResult() { Content = "重复提交" };
                }

            }
            catch (Exception ex)
            {
                CustomLog.WriteLog(ex.ToString());
                return new ContentResult() { Content = "fail" + temp + ex };
            }
        }

        public ActionResult Clear()
        {
            IContentType ct = Services.ContentTypeService.GetContentType("PayRecords");
            IEnumerable<IContent> list = Services.ContentService.GetContentOfContentType(ct.Id).Where(e => e.GetValue<bool>("isdeposit") == false);
            foreach (IContent item in list)
            {
                if (item.CreateDate < DateTime.Now.AddDays(-7))
                {
                    Services.ContentService.Delete(item);
                }
            }
            ResponseModel model = new ResponseModel();
            model.Success = true;
            model.Msg = "清除成功!";
            return Json(model, JsonRequestBehavior.AllowGet);
        }


    }
}