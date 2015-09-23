using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using umbraco.businesslogic;
using umbraco.BusinessLogic.Actions;
using umbraco.interfaces;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace Bytefunds.Cms.Logic.CustomSection
{

    [Application("Withdraw", "Withdraw", "icon-bill-yen", 15)]
    public class WithdrawApplication : IApplication { }
    [PluginController("Withdraw")]
    [Umbraco.Web.Trees.Tree("Withdraw", "Withdraw", "Withdraw", iconClosed: "icon-bill-yen", iconOpen: "icon-bill-yen")]
    public class WithdrawManagerTreeController : TreeController
    {
        private IContentType contentType_settings;
        private ServiceContext Services;
        public WithdrawManagerTreeController()
        {
            Services = ApplicationContext.Services;
            contentType_settings = Services.ContentTypeService.GetContentType("WithdrawElement");
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            MenuItemCollection menus = new MenuItemCollection();
            menus.Items.Add<ActionDelete>("Delete");
            menus.Items.Add(new MenuItem("Approved", "审核提现"));
            menus.Items.Add<RefreshNode, ActionRefresh>("Refresh Nodes");
            return menus;
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();
            IContentType ct = Services.ContentTypeService.GetContentType("WithdrawElement");
            IEnumerable<IContent> list = Services.ContentService.GetContentOfContentType(ct.Id);
            bool ischeck = false;
            if (string.Compare(id, "-1") == 0)
            {
                //根节点
                var node = this.CreateTreeNode(ct.Id.ToString(), id, queryStrings, "提现申请", "icon-folder", true);
                nodes.Add(node);

            }
            else if (id.Equals(ct.Id.ToString()))
            {
                //用户点击购买 后未购买 和已经购买状态
                var listgroup = list.GroupBy(d =>
                    new
                    {
                        IsCheck = d.GetValue<bool>("isCheck")
                    }
                    ).Select(gd =>
                    new { GroupKey = gd.Key, Count = gd.Count() }
                    );
                foreach (var item in listgroup.OrderBy(m => m.GroupKey.IsCheck))
                {
                    if (item.Count > 0)
                    {
                        string title = (item.GroupKey.IsCheck ? "已审核" : "未审核");
                        var node = this.CreateTreeNode(item.GroupKey.IsCheck.ToString(), id, queryStrings, title + " " + item.Count, "icon-folder", true);
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
                    var currentlist = list.Where(d => d.GetValue<bool>("isCheck").Equals(bool.Parse(ids[1])) && d.CreateDate.ToString("yyyy.MM").Equals(ids[0])).OrderByDescending(c => c.Id);
                    foreach (var curitem in currentlist)
                    {
                        string day = curitem.CreateDate.Day.ToString();
                        var node = this.CreateTreeNode(curitem.Id.ToString(), ids[0], queryStrings, curitem.GetValue<string>("memberName") + " (" + curitem.GetValue<decimal>("amount").ToString("N2") + "￥ " + day + "日)", "icon-umb-users", false);

                        node.AdditionalData.Add("amount", curitem.GetValue<decimal>("amount").ToString("N2"));
                        node.AdditionalData.Add("okassets", curitem.GetValue<decimal>("okassets").ToString("N2"));
                        //int memberid = 0;
                        //if (int.TryParse(curitem.GetValue<string>("memberPicker"), out memberid))
                        //{
                        //    IMember member = Services.MemberService.GetById(memberid);
                        //    if (member != null)
                        //    {
                        //        node.AdditionalData.Add("memberKey", member.Key.ToString());
                        //    }
                        //}
                        nodes.Add(node);
                    }
                }
            }
            else if (bool.TryParse(id, out ischeck))
            {
                //日期节点 二级节点
                var listdategroup = list.Where(d => d.GetValue<bool>("isCheck").Equals(bool.Parse(id))).OrderByDescending(d => d.CreateDate).GroupBy(d => new { Date = d.CreateDate.ToString("yyyy.MM") }).Select(gd => new { GroupDateKey = gd.Key, Count = gd.Count() });
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