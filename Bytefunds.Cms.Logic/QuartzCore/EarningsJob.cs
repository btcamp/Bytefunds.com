﻿using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using umbraco;
using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Bytefunds.Cms.Logic.QuartzCore
{
    public class EarningsJob : IJob
    {
        private ServiceContext Services = ApplicationContext.Current.Services;
        public void Execute(IJobExecutionContext context)
        {
            //获取所有已经购买的产品
            IContentType ct = Services.ContentTypeService.GetContentType("PayRecords");
            IEnumerable<IContent> list = Services.ContentService.GetContentOfContentType(ct.Id).Where(e => e.GetValue<bool>("isdeposit") == true && e.GetValue<bool>("isexpired") == false);
            IEnumerable<int> memberids = list.GroupBy(e => e.GetValue<int>("memberPicker")).Select(e => e.Key);
            foreach (int memberid in memberids)
            {
                decimal sumProfit = 0;
                IMember member = Services.MemberService.GetById(memberid);
                StringBuilder sb = new StringBuilder();
                //获取product的收益率
                foreach (IContent content in list.Where(e => e.GetValue<int>("memberPicker") == memberid))
                {
                    //yyyy-MM-dd
                    if (content.GetValue<DateTime>("expirationtime") > DateTime.Now.AddDays(1))
                    {
                        int productId = content.GetValue<int>("buyproduct");
                        IContent product = Services.ContentService.GetById(productId);
                        int number = 0;
                        if (int.TryParse(product.GetValue<string>("rate"), out number))
                        {
                            decimal rate = (decimal)number / (decimal)100;
                            decimal profit = (content.GetValue<decimal>("amountCny") * rate) / (decimal)365;
                            //修改member的最新收益
                            sumProfit += profit;
                            //记录这次计算结果；
                            string earningsName = string.Format("{0} ({1})", content.GetValue<string>("username"), profit.ToString("F2"));
                            IContentType earningsContentType = Services.ContentTypeService.GetContentType("EarningsRecordsElement");
                            IContent earningsContent = Services.ContentService.CreateContent(earningsName, earningsContentType.Id, "EarningsRecordsElement");
                            earningsContent.SetValue("memberid", memberid);
                            earningsContent.SetValue("productid", content.Id);
                            earningsContent.SetValue("earning", profit.ToString("f2"));
                            Services.ContentService.Save(earningsContent);
                            sb.AppendFormat("<p style='orphans: 2; widows: 2; padding: 1px 34px 21px; margin: 0px;'><span style='font-size: 14px; line-height: 21px;'>您购买的产品《{0}》投入了{1}元；昨日的收益是：{2}元</span></p>", product.GetValue<string>("title"), content.GetValue<decimal>("amountCny").ToString("N2"), profit.ToString("N2"));
                        }
                    }
                    else
                    {
                        //到期的 将余额提出到账户余额中
                        decimal assets = member.GetValue<decimal>("assets");
                        member.SetValue("assets", (assets + content.GetValue<decimal>("amountCny")).ToString("f2"));
                        Services.MemberService.Save(member);
                        //修改基金到期
                        content.SetValue("isexpired", true);
                        Services.ContentService.Save(content);
                    }
                }

                member.SetValue("latestearnings", sumProfit.ToString("f2"));
                decimal accumulatedEarnings = member.GetValue<decimal>("accumulatedEarnings");
                member.SetValue("accumulatedEarnings", (accumulatedEarnings + sumProfit).ToString("f2"));
                Services.MemberService.Save(member);
                //发送邮件通知
                if (sb.Length > 0)
                {
                    sb.AppendFormat("<p style='orphans: 2; widows: 2; padding: 1px 34px 21px; margin: 0px;'><span style='font-size: 14px; line-height: 21px;'>您购买的所有产品总收益：{0}</span></p>", sumProfit.ToString("N2"));
                    //获取收益通知邮件模板
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("{{msg}}", sb.ToString());
                    dic.Add("{{name}}", member.Name);
                    Helpers.SendmailHelper.SendEmail(member.Username, "member:newprofit", dic);
                }
            }

        }
    }
}