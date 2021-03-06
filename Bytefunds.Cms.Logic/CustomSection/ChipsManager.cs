﻿using System;
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
    [Application("Chips", "Chips", "icon-coin-dollar", 11)]
    public class ChipsManagerApplication : IApplication { }

    [PluginController("Chips")]
    [Umbraco.Web.Trees.Tree("Chips", "Chips", "众筹管理", iconClosed: "icon-mailbox")]
    public class ChipsManagerTreeController : TreeController
    {
        //设置菜单
        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();
            menu.Items.Add<RefreshNode, ActionRefresh>("刷新节点");
            menu.Items.Add<ActionDelete>("Delete");
            return menu;
        }
        //树节点获取子节点中的数据
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();
            IContentType ct = Services.ContentTypeService.GetContentType("ChipsDepositDocument");
            IEnumerable<IContent> list = Services.ContentService.GetContentOfContentType(ct.Id);
            bool isRefund;
            if (string.Compare(id, "-1") == 0)
            {
                //根节点
                var node = this.CreateTreeNode(ct.Id.ToString(), id, queryStrings, "众筹用户", "icon-folder", true);
                nodes.Add(node);

            }
            else if (id.Equals(ct.Id.ToString()))
            {
                //用户点击购买 后未购买 和已经购买状态
                var listgroup = list.GroupBy(d =>
                    new
                    {
                        IsRefund = d.GetValue<bool>("isRefund")
                    }
                    ).Select(gd =>
                    new { GroupKey = gd.Key, Count = gd.Count() }
                    );
                foreach (var item in listgroup.OrderBy(m => m.GroupKey.IsRefund))
                {
                    if (item.Count > 0)
                    {
                        string title = (item.GroupKey.IsRefund ? "已退款" : "未退款");
                        var node = this.CreateTreeNode(item.GroupKey.IsRefund.ToString(), id, queryStrings, title + " " + item.Count, "icon-folder", true);
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
                    var currentlist = list.Where(d => d.GetValue<bool>("isRefund").Equals(bool.Parse(ids[1])) && d.CreateDate.ToString("yyyy.MM").Equals(ids[0])).OrderByDescending(c => c.Id);
                    foreach (var curitem in currentlist)
                    {
                        var node = this.CreateTreeNode(curitem.Id.ToString(), ids[0], queryStrings, curitem.GetValue<string>("username") + " (" + curitem.GetValue<string>("amount") + "￥ " + curitem.CreateDate.Day + "日)", "icon-umb-users", false);

                        nodes.Add(node);
                    }
                }
            }
            else if (bool.TryParse(id, out isRefund))
            {
                //日期节点 二级节点
                var listdategroup = list.Where(d => d.GetValue<bool>("isRefund").Equals(bool.Parse(id))).OrderByDescending(d => d.CreateDate).GroupBy(d => new { Date = d.CreateDate.ToString("yyyy.MM") }).Select(gd => new { GroupDateKey = gd.Key, Count = gd.Count() });
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