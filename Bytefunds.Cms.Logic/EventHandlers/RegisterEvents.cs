using Bytefunds.Cms.Logic.CustomSection;
using Bytefunds.Cms.Logic.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
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
        }

        private void ContentService_Saved(IContentService sender, Umbraco.Core.Events.SaveEventArgs<IContent> e)
        {
         
            foreach (IContent content in e.SavedEntities)
            {
                string typeAlias=content.ContentType.Alias;
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

            try
            {

                //给管理员发邮件，用户入金出金操作
                //File.WriteAllText("d:\\" + Guid.NewGuid().ToString() + ".txt", "contentcreated");
                if (e.Alias.ToLower().Equals("payrecords") || e.Alias.ToLower().Equals("refundtype"))
                {
                    string tplid = string.Empty;
                    string title = string.Empty;
                    if (e.Alias.ToLower().Equals("payrecords"))
                    {
                        tplid = SystemSettingsHelper.GetSystemSettingsByKey("manager:payment:tplid");
                    }
                    else if (e.Alias.ToLower().Equals("refundtype"))
                    {
                        tplid = SystemSettingsHelper.GetSystemSettingsByKey("manager:refund:tplid");
                    }

                    int tpl;
                    if (int.TryParse(tplid, out tpl))
                    {
                        IMedia content = ApplicationContext.Current.Services.MediaService.GetById(tpl);
                        //创建Content的时候Name属性赋值的email
                        IMember member = ApplicationContext.Current.Services.MemberService.GetByEmail(e.Entity.GetValue<string>("email"));
                        if (member == null)
                        {
                            return;
                            // throw new CustomException.NotFoundEmailException("邮箱不存在");
                        }
                        string oldbodycontent = Helpers.SendmailHelper.Replace(content.GetValue<string>("bodytext"), member.Key, e.Entity.Id);
                        string managerEmail =SystemSettingsHelper.GetSystemSettingsByKey("manager:email");
                        Helpers.SendmailHelper.SendEmail(content.GetValue<string>("title"), oldbodycontent, managerEmail);
                    }
                }
            }
            catch (Exception ex)
            {

                return;
            }
            #endregion
        }


        void MemberService_Created(IMemberService sender, Umbraco.Core.Events.NewEventArgs<Umbraco.Core.Models.IMember> e)
        {

            try
            {
                int tplid;
                string langCode = e.Entity.ContentTypeAlias.Replace("member_", "").ToLower();
           
                string managerTeplateId = SystemSettingsHelper.GetSystemSettingsByKey("manager:register:tplid" );
                string rgisterTemplateId = SystemSettingsHelper.GetSystemSettingsByKey("member:register:tplid:" + langCode);
                string managerEmail = SystemSettingsHelper.GetSystemSettingsByKey("manager:email");

                if (int.TryParse(managerTeplateId, out tplid))
                {
                    //用户创建成功给管理员和用户自己发邮件
                    IMedia managercontent =
                        ApplicationContext.Current.Services.MediaService.GetById(tplid);
                    string managerbodycontent = Helpers.SendmailHelper.Replace(managercontent.GetValue<string>("bodytext"),
                        e.Entity.Key, -1);
                    Helpers.SendmailHelper.SendEmail(managercontent.GetValue<string>("title"), managerbodycontent,
                        managerEmail);
                }
                if (int.TryParse(rgisterTemplateId, out tplid))
                {
                    //给用户发邮件
                    IMedia membercontent =
                        ApplicationContext.Current.Services.MediaService.GetById(tplid);
                    string memberbodycontent = Helpers.SendmailHelper.Replace(membercontent.GetValue<string>("bodytext"),
                        e.Entity.Key, -1);
                    Helpers.SendmailHelper.SendEmail(membercontent.GetValue<string>("title"), memberbodycontent,
                        e.Entity.Email);
                }


                //查找所有订阅信息的用户
                IContentType subscribeType = ApplicationContext.Current.Services.ContentTypeService.GetContentType("Subscribemember");
                IEnumerable<IContent> subscribeMembers = ApplicationContext.Current.Services.ContentService.GetContentOfContentType(subscribeType.Id);
                  foreach (var member in subscribeMembers)
                  {
                      if (member.GetValue<string>("email").Trim()==e.Entity.Email.Trim())
                      {
                          ApplicationContext.Current.Services.ContentService.Delete(member);
                      }
                  }
           
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}