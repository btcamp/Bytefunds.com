using Bytefunds.Cms.Logic.CustomSection;
using Bytefunds.Cms.Logic.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using umbraco;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Bytefunds.Cms.Logic.EventHandlers
{
    public class RegisterEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            CustomRaiseEvent.Registered += MemberService_Created;
            CustomRaiseEvent.ContentCreated += ContentService_Created;
            //Bytefunds.Cms.Logic.Quartz.FinotecQuartz.intiQuartz();
            //TODO 测试这两个New的事件是否有效。目标是实现，有新注册用户和有新的入金记录，就发邮件到指定邮箱通知管理员。
            ///参考连接：http://our.umbraco.org/documentation/Reference/Events/application-startup
            /// http://our.umbraco.org/wiki/reference/api-cheatsheet/using-applicationbase-to-register-events/overview-of-all-events
            /// 
            //umbraco.cms.businesslogic.member.Member.AfterSave+=Member_AfterSave;

            ContentService.Saved += ContentService_Saved;
            QuartzCore.QuartzJobUtils.Instance.StartQuartz();
        }



        private void ContentService_Saved(IContentService sender, Umbraco.Core.Events.SaveEventArgs<IContent> e)
        {

            foreach (IContent content in e.SavedEntities)
            {
                string typeAlias = content.ContentType.Alias;
                if (typeAlias == "Setting")
                {
                    content.ParentId = content.ContentType.Id;
                    ApplicationContext.Current.Services.ContentService.Save(content, 0, false);
                }
            }
        }

        void ContentService_Created(IContentService sender, Umbraco.Core.Events.NewEventArgs<Umbraco.Core.Models.IContent> e)
        {
            #region 出入金发邮件
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    //给管理员发邮件，用户入金出金操作
                    //File.WriteAllText("d:\\" + Guid.NewGuid().ToString() + ".txt", "contentcreated");
                    if (e.Alias.ToLower().Equals("payrecords") || e.Alias.ToLower().Equals("withdrawelement"))
                    {
                        string tplid = string.Empty;
                        string title = string.Empty;
                        string accountbuyId = string.Empty, accountwithdrawId = string.Empty;
                        string managerEmail = SystemSettingsHelper.GetSystemSettingsByKey("manager:email");
                        if (e.Alias.ToLower().Equals("payrecords"))
                        {
                            tplid = SystemSettingsHelper.GetSystemSettingsByKey("manager:payment:tplid");
                            accountbuyId = SystemSettingsHelper.GetSystemSettingsByKey("account:buy:tplid");

                            int tpl, accounttplid;

                            if (int.TryParse(tplid, out tpl) && int.TryParse(accountbuyId, out accounttplid))
                            {
                                IMedia content = ApplicationContext.Current.Services.MediaService.GetById(tpl);
                                IMedia accountbuytmp = ApplicationContext.Current.Services.MediaService.GetById(accounttplid);
                                //创建Content的时候Name属性赋值的email
                                IMember member = ApplicationContext.Current.Services.MemberService.GetById(e.Entity.GetValue<int>("memberPicker"));
                                IContent product = ApplicationContext.Current.Services.ContentService.GetById(e.Entity.GetValue<int>("buyproduct"));
                                if (member == null)
                                {
                                    return;
                                    // throw new CustomException.NotFoundEmailException("邮箱不存在");
                                }
                                Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                                MailSettingsSectionGroup mailSettings = (MailSettingsSectionGroup)configurationFile.GetSectionGroup("system.net/mailSettings");


                                string oldbodycontent = content.GetValue<string>("bodytext")
                                                        .Replace("{{name}}", member.Name)
                                                        .Replace("{{product}}", product.GetValue<string>("title"))
                                                        .Replace("{{amount}}", e.Entity.GetValue<double>("amountCny").ToString("N2"));
                                //发送邮件到管理员
                                library.SendMail(mailSettings.Smtp.Network.UserName, managerEmail, content.GetValue<string>("title"), oldbodycontent, true);
                                //发送邮件到用户
                                string accountContent = accountbuytmp.GetValue<string>("bodytext")
                                                        .Replace("{{name}}", member.Name)
                                                        .Replace("{{product}}", product.GetValue<string>("title"))
                                                        .Replace("{{rate}}", product.GetValue<string>("rate"))
                                                        .Replace("{{amount}}", e.Entity.GetValue<double>("amountCny").ToString("N2"));
                                Common.CustomLog.WriteLog(member.Username + "\r\n" + accountbuytmp.GetValue<string>("title"));
                                library.SendMail(mailSettings.Smtp.Network.UserName, member.Username, accountbuytmp.GetValue<string>("title"), accountContent, true);
                            }

                        }
                        else if (e.Alias.ToLower().Equals("withdrawelement"))
                        {

                            IMember member = ApplicationContext.Current.Services.MemberService.GetById(e.Entity.GetValue<int>("memberId"));
                            Dictionary<string, string> managerdir = new Dictionary<string, string>();
                            managerdir.Add("{{name}}", member.Name);
                            managerdir.Add("{{amount}}", e.Entity.GetValue<string>("amount"));
                            managerdir.Add("{{bankNumber}}", e.Entity.GetValue<string>("bankNumber"));
                            managerdir.Add("{{bankName}}", e.Entity.GetValue<string>("bankName"));
                            //发送给管理员
                            SendmailHelper.SendEmail(managerEmail, "manager:withdraw:tplid", managerdir);
                            Dictionary<string, string> accountdir = new Dictionary<string, string>();
                            accountdir.Add("{{name}}", member.Name);
                            accountdir.Add("{{amount}}", e.Entity.GetValue<string>("amount"));
                            //发送给用户
                            SendmailHelper.SendEmail(member.Username, "member:withdraw:tplId", accountdir);
                        }


                    }
                }
                catch (Exception ex)
                {
                    Common.CustomLog.WriteLog(ex.ToString());
                    return;
                }

            });
            #endregion
        }


        void MemberService_Created(IMemberService sender, Umbraco.Core.Events.NewEventArgs<Umbraco.Core.Models.IMember> e)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    int tplid;
                    string managerTeplateId = SystemSettingsHelper.GetSystemSettingsByKey("manager:register:tplid");
                    string rgisterTemplateId = SystemSettingsHelper.GetSystemSettingsByKey("member:register:tplid:bytefunds");
                    string managerEmail = SystemSettingsHelper.GetSystemSettingsByKey("manager:email");
                    Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                    MailSettingsSectionGroup mailSettings = (MailSettingsSectionGroup)configurationFile.GetSectionGroup("system.net/mailSettings");
                    if (int.TryParse(managerTeplateId, out tplid))
                    {
                        //用户创建成功给管理员和用户自己发邮件
                        IMedia mediatamplate =
                            ApplicationContext.Current.Services.MediaService.GetById(tplid);
                        string content = mediatamplate.GetValue<string>("bodytext").Replace("{{name}}", e.Entity.Name);
                        library.SendMail(mailSettings.Smtp.Network.UserName, managerEmail, mediatamplate.GetValue<string>("title"), content, true);
                    }
                    if (int.TryParse(rgisterTemplateId, out tplid))
                    {
                        //给用户发邮件
                        IMedia mediatamplate =
                            ApplicationContext.Current.Services.MediaService.GetById(tplid);
                        //string memberbodycontent = Helpers.SendmailHelper.Replace(mediatamplate.GetValue<string>("bodytext"),
                        //    e.Entity.Key, -1);
                        //Helpers.SendmailHelper.SendEmail(membercontent.GetValue<string>("title"), memberbodycontent,
                        //    e.Entity.Email);

                        library.SendMail(mailSettings.Smtp.Network.UserName, e.Entity.Email, mediatamplate.GetValue<string>("title"), mediatamplate.GetValue<string>("bodytext"), true);
                    }

                }
                catch (Exception ex)
                {
                    return;
                }
            });
        }


    }
}