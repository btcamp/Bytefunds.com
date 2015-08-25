using Bytefunds.Cms.Logic.Common;
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
    public class PaymentController : SurfaceController
    {
        public ActionResult PaySuccess(string Content, bool isServerCall = false)
        {

            string temp = Content;
            try
            {
                Content = DESHelper.DecryptDES(Content);
                System.IO.File.AppendAllText(System.IO.Path.Combine(Server.MapPath("~/"), Guid.NewGuid().ToString() + ".txt"), Content);
                temp = Content;
                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                PaymentModel payModel = jss.Deserialize<Models.PaymentModel>(Content);

                //判断订单是否存在
                if (Helpers.UmbPayRecordsHelper.IsExistsBillno(payModel.Billno))
                {
                    IContent content = null;
                    if (payModel.Email.Contains("^_^"))
                    {
                        string[] array = payModel.Email.Split(new string[] { "^_^" }, StringSplitOptions.RemoveEmptyEntries);

                        IMember currentMember = Services.MemberService.GetByEmail(array[0]);
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
                            currentMember.SetValue("assets", (assets + payModel.Amount).ToString("f2"));
                            currentMember.SetValue("fundAccount", (fundAccount + payModel.Amount).ToString("f2"));
                            Services.MemberService.Save(currentMember);
                        }
                        //cny
                        content.SetValue("amountCny", payModel.Amount.ToString());
                        content.SetValue("rechargeDateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        content.SetValue("payBillno", payModel.Billno);
                    }
                    else
                    {
                        IContentType ct = Services.ContentTypeService.GetContentType("PayRecords");
                        content = Services.ContentService.CreateContent(payModel.Username, ct.Id, "PayRecords");
                        content.SetValue("username", payModel.Username);
                        content.Name = payModel.Username;
                        content.SetValue("email", payModel.Email);
                        content.SetValue("mobilePhone", payModel.Phone);
                    }
                    content.SetValue("amountCny", payModel.Amount.ToString());

                    content.SetValue("rechargeDateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    content.SetValue("payBillno", payModel.Billno);
                    content.SetValue("isdeposit", true);
                    Services.ContentService.Save(content);
                    //触发创建事件
                    EventHandlers.CustomRaiseEvent.RaiseContentCreated(content);
                    System.IO.File.AppendAllText(System.IO.Path.Combine(Server.MapPath("~/"), Guid.NewGuid().ToString() + ".txt"), "success!!!");
                    return Json("ok", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    System.IO.File.AppendAllText("c:\\PaySuccess.txt", "重复提交" + payModel.Billno);
                    return new ContentResult() { Content = "重复提交" };
                }

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(System.IO.Path.Combine(Server.MapPath("~/"), Guid.NewGuid().ToString() + ".txt"), ex.ToString());
                return new ContentResult() { Content = "fail" + temp + ex };
            }
        }


        public ActionResult Clear()
        {
            IContentType ct = Services.ContentTypeService.GetContentType("PayRecords");
            IEnumerable<IContent> list = Services.ContentService.GetContentOfContentType(ct.Id);
            foreach (IContent item in list)
            {
                Services.ContentService.Delete(item);
            }
            ResponseModel model = new ResponseModel();
            model.Success = true;
            model.Msg = "清除成功!";
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}