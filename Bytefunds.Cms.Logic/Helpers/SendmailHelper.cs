using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Bytefunds.Cms.Logic.Helpers
{
    public class SendmailHelper
    {
        /// <summary>
        /// 异步发送邮件
        /// </summary>
        /// <param name="memberKey"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool SystemMsg(Guid memberKey, string title, string content)
        {
            var tomember = ApplicationContext.Current.Services.MemberService.GetByKey(memberKey);
            return SendEmail(title, content, tomember.Email);

        }

        /// <summary>
        /// 异步发送邮件
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="toemail"></param>
        /// <returns></returns>
        public static bool SendEmail(string title, string content, string toemail)
        {
            try
            {
                IMember currentMember = null;
                string managerEmail = SystemSettingsHelper.GetSystemSettingsByKey("manager:email");

                if (!toemail.Equals(managerEmail))
                {
                    currentMember = ApplicationContext.Current.Services.MemberService.GetByEmail(toemail);
                }
                IContentType ct = ApplicationContext.Current.Services.ContentTypeService.GetContentType("EmailRecords");
                IContent doc = ApplicationContext.Current.Services.ContentService.CreateContent(title + "|" + DateTime.Now.ToShortDateString(), ct.Id, "EmailRecords");
             

                doc.SetValue("memberId", (currentMember == null && toemail == managerEmail) ? "0" : currentMember.Id.ToString());
                doc.SetValue("emailAddress", (currentMember == null && toemail == managerEmail) ? toemail : currentMember.Email);

                doc.SetValue("title", title);
                doc.SetValue("content", content);
                doc.SetValue("isSend", false);
                doc.SetValue("retryCount", "0");
                ApplicationContext.Current.Services.ContentService.Save(doc);
                Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                MailSettingsSectionGroup mailSettings = (MailSettingsSectionGroup)configurationFile.GetSectionGroup("system.net/mailSettings");

                //开启线程发送邮件
                Models.SenMailModel model = new Models.SenMailModel()
                {
                    Content = content,
                    CurrentSendMailKey = doc.Key,
                    Services = ApplicationContext.Current.Services,
                    Title = title,
                    ToEmail = toemail,
                    UserName = mailSettings.Smtp.Network.UserName,
                    Context = HttpContext.Current
                };
                Thread th = new Thread(TaskSendMail);
                th.IsBackground = true;
                th.Start(model);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 线程发送邮件执行
        /// </summary>
        /// <param name="model"></param>
        private static void TaskSendMail(object model)
        {
            Models.SenMailModel sendmodel = (Models.SenMailModel)model;
            int count;
            string str = SystemSettingsHelper.GetSystemSettingsByKey("send:mail:count");

            if (!int.TryParse(str, out count))
            {
                count = 1;
            }
            while (true)
            {
                try
                {
                    if (str == count.ToString() || (string.IsNullOrEmpty(str) && count == 1))
                    {
                        sendmodel.Content += "<div style='background:url(" + string.Format("http://{0}:{1}/umbraco/API/MailService/EmailTrack/" + sendmodel.CurrentSendMailKey.ToString(), sendmodel.Context.Request.Url.Host, sendmodel.Context.Request.Url.Port) + ")'></div>";
                        //sendmodel.Content += "<div style='background:url(" + string.Format("http://{0}:{1}/images/100usd.png", sendmodel.Context.Request.Url.Host, sendmodel.Context.Request.Url.Port) + ")'></div>";
                        //sendmodel.Content += "<link href='" + string.Format("http://{0}:{1}/umbraco/API/MailService/EmailTrack/" + sendmodel.CurrentSendMailKey.ToString(), sendmodel.Context.Request.Url.Host, sendmodel.Context.Request.Url.Port) + "' rel='stylesheet' />";
                        //sendmodel.Content += "<img src='" + string.Format("http://{0}:{1}/images/100usd.png", sendmodel.Context.Request.Url.Host, sendmodel.Context.Request.Url.Port) + "' />";
                    }
                    umbraco.library.SendMail(sendmodel.UserName, sendmodel.ToEmail, sendmodel.Title, sendmodel.Content, true);
                    IContent currentContent = sendmodel.Services.ContentService.GetById(sendmodel.CurrentSendMailKey);
                    currentContent.SetValue("retryCount", (currentContent.GetValue<int>("retryCount") + 1));
                    currentContent.SetValue("isSend", true);
                    currentContent.SetValue("sendTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sendmodel.Services.ContentService.Save(currentContent);
                    count--;
                    break;
                }
                catch (Exception)
                {
                    if (count <= 0)
                    {
                        break;
                    }
                }
            }

        }

        //public static IMedia GetDefaultMailtemplate()
        //{
        //    return GetMailtemplates().FirstOrDefault();
        //}
        //public static IEnumerable<IMedia> GetMailtemplates()
        //{
        //    int templateContainerID = 4372;
        //    var templateContainer = ApplicationContext.Current.Services.MediaService.GetById(templateContainerID);
        //    return templateContainer.Children().Where(o => o.GetValue<int>("templatetype") == (int)type);
        //}
        //public static IEnumerable<IMedia> GetMailtemplatesByParentId(MailTemplateType type, int templateContainerID)
        //{
        //    var templateContainer = ApplicationContext.Current.Services.MediaService.GetById(templateContainerID);
        //    var children = templateContainer.Children().ToList();
        //    return templateContainer.Children().Where(o => o.GetValue<int>("templatetype") == (int)type);
        //}
        public static string Replace(string content, Guid memberKey, int recoredid)
        {
            var tomember = ApplicationContext.Current.Services.MemberService.GetByKey(memberKey);
            if (content.IndexOf(tomember.ContentType.Name) < 0)
            {
                content = content.Replace("{{Member", "{{" + tomember.ContentType.Name);
            }
            content = contentReplace(content, tomember);
            content = memberReplace(content, tomember);
            if (recoredid != -1)
            {
                var record = ApplicationContext.Current.Services.ContentService.GetById(recoredid);
                content = contentReplace(content, record);
            }
            return content;
        }
        private static string memberReplace(string content, IMember member)
        {
            string contenttypename = ApplicationContext.Current.Services.EntityService.Get(member.ContentTypeId, true).Name;
            content = content.Replace("{{" + contenttypename + ".Login}}", member.Username);
            content = content.Replace("{{" + contenttypename + ".RawPasswordValue}}", member.RawPasswordValue);
            return content;
        }
        private static string contentReplace(string content, IContentBase entity)
        {
            var ptypes = entity.PropertyTypes;
            string contenttypename = ApplicationContext.Current.Services.EntityService.Get(entity.ContentTypeId, true).Name;

            foreach (var ptype in ptypes)
            {
                PreValueCollection coll = ApplicationContext.Current.Services.DataTypeService.GetPreValuesCollectionByDataTypeId(ptype.DataTypeDefinitionId);
                if (coll.PreValuesAsDictionary.Count == 0)
                {
                    string value=entity.GetValue<string>(ptype.Alias);
                    
                    content = content.Replace("{{" + contenttypename + "." + ptype.Alias + "}}", entity.GetValue<string>(ptype.Alias));
                }
                else
                {

                    foreach (var item in coll.PreValuesAsDictionary)
                    {
                        var kk = entity.GetValue<int>(ptype.Alias);
                        if (item.Value.Id.Equals(entity.GetValue<int>(ptype.Alias)))
                        {
                            string strval = item.Value.Value;
                            content = content.Replace("{{" + contenttypename + "." + ptype.Alias + "}}", strval);
                        }
                    }
                }

            }
            return content;
        }


        public static void SendEmail_WithoutRecord(string title,string content,string toMail)
        {
            try
            {
                Random r = new Random((int)DateTime.Now.Ticks);
                int sleepTime  = r.Next(100, 1000);
                Thread.Sleep(sleepTime);
                Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                MailSettingsSectionGroup mailSettings = (MailSettingsSectionGroup)configurationFile.GetSectionGroup("system.net/mailSettings");
                umbraco.library.SendMail(mailSettings.Smtp.Network.UserName,toMail,title,content, true);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

     
    }

}