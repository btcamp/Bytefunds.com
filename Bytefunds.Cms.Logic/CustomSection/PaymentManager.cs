using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using umbraco.businesslogic;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Umbraco.Core;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core.Models;
namespace Bytefunds.Cms.Logic.CustomSection
{
    [Application("PaymentManager", "PaymentManager", "icon-coin-dollar", 11)]
    public class PaymentManagerApplication : IApplication { }

    [PluginController("PaymentManager")]
    [Umbraco.Web.Trees.Tree("PaymentManager", "PaymentManager", "PaymentManager", iconClosed: "icon-mailbox")]
    public class PaymentManagerTreeController : TreeController
    {
        //设置菜单
        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();
            IContentType ct = Services.ContentTypeService.GetContentType("PayRecords");
            //if (ct.Id.ToString().Equals(id))
            //{
            //    menu.Items.Add(new MenuItem("create", "创建入金记录"));
            //}
            //menu.Items.Add(new MenuItem("SendMail", "重发入金成功邮件"));
            //menu.Items.Add(new MenuItem("Approved", "审核入金"));
            if (id.ToLower() == "false")
            {
                menu.Items.Add(new MenuItem("clear", "清除未购买数据"));
            }
            menu.Items.Add<RefreshNode, ActionRefresh>("刷新节点");
            menu.Items.Add<ActionDelete>("Delete");
            return menu;
        }
        //树节点获取子节点中的数据
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();
            bool isdeposit;

            //DocumentType dt = DocumentType.GetByAlias("PayRecords");
            //IEnumerable<Document> list = Document.GetDocumentsOfDocumentType(dt.Id);
            IContentType ct = Services.ContentTypeService.GetContentType("PayRecords");
            IEnumerable<IContent> list = Services.ContentService.GetContentOfContentType(ct.Id);
            if (string.Compare(id, "-1") == 0)
            {
                //根节点
                var node = this.CreateTreeNode(ct.Id.ToString(), id, queryStrings, "产品购买", "icon-folder", true);
                nodes.Add(node);

            }
            else if (id.Equals(ct.Id.ToString()))
            {
                //用户点击购买 后未购买 和已经购买状态
                var listgroup = list.GroupBy(d =>
                    new
                    {
                        IsDeposit = d.GetValue<bool>("isdeposit")
                    }
                    ).Select(gd =>
                    new { GroupKey = gd.Key, Count = gd.Count() }
                    );
                foreach (var item in listgroup.OrderBy(m => m.GroupKey.IsDeposit))
                {
                    if (item.Count > 0)
                    {
                        string title = (item.GroupKey.IsDeposit ? "已购买" : "未购买");
                        var node = this.CreateTreeNode(item.GroupKey.IsDeposit.ToString(), id, queryStrings, title + " " + item.Count, "icon-folder", true);
                        nodes.Add(node);
                    }
                }
            }
            else if (id.Contains(".") && id.Contains("^_^"))
            {
                //用户节点 三级节点
                string[] ids = id.Split(new string[] { "^_^" }, StringSplitOptions.RemoveEmptyEntries);
                if (ids.Length.Equals(2))
                {
                    var currentlist = list.Where(d => d.GetValue<bool>("isdeposit").Equals(bool.Parse(ids[1])) && d.GetValue<DateTime>("rechargeDateTime").ToString("yyyy.MM").Equals(ids[0])).OrderByDescending(c => c.Id);
                    foreach (var curitem in currentlist)
                    {

                        string day = curitem.GetValue("rechargeDateTime") == null ? curitem.CreateDate.Day + "" : curitem.GetValue<DateTime>("rechargeDateTime").Day + "";
                        var node = this.CreateTreeNode(curitem.Id.ToString(), ids[0], queryStrings, curitem.GetValue<string>("username") + " (" + curitem.GetValue<string>("amountCny") + "￥ " + day + "日)", "icon-umb-users", false);
                        node.AdditionalData.Add("depositId", curitem.GetValue<string>("depositImage"));

                        int memberid = 0;
                        if (int.TryParse(curitem.GetValue<string>("memberPicker"), out memberid))
                        {
                            IMember member = Services.MemberService.GetById(memberid);
                            if (member != null)
                            {
                                node.AdditionalData.Add("memberKey", member.Key.ToString());
                            }
                        }
                        nodes.Add(node);
                    }
                }
            }
            else if (bool.TryParse(id, out isdeposit))
            {
                //日期节点 二级节点
                var listdategroup = list.Where(d => d.GetValue<bool>("isdeposit").Equals(bool.Parse(id))).OrderByDescending(d => d.GetValue<DateTime>("rechargeDateTime")).GroupBy(d => new { Date = d.GetValue<DateTime>("rechargeDateTime").ToString("yyyy.MM") }).Select(gd => new { GroupDateKey = gd.Key, Count = gd.Count() });
                foreach (var item in listdategroup)
                {
                    if (item.Count > 0)
                    {
                        var node = this.CreateTreeNode(item.GroupDateKey.Date + "^_^" + id, id, queryStrings, item.GroupDateKey.Date, "icon-folder", true);
                        nodes.Add(node);
                    }
                }
            }
            return nodes;
        }
    }
}