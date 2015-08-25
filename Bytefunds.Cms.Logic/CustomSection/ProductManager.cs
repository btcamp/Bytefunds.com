using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using umbraco.businesslogic;
using umbraco.BusinessLogic.Actions;
using umbraco.interfaces;
using Umbraco.Core.Models;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace Bytefunds.Cms.Logic.CustomSection
{
    [Application("Product", "Product", "icon-plane", 8)]
    public class ProductManagerApplication : IApplication { }

    [PluginController("Product")]
    [Umbraco.Web.Trees.Tree("Product", "Product", "Product", iconClosed: "icon-plane", iconOpen: "icon-plane", initialize: true, sortOrder: 8)]
    public class ProductManagerTreeController : TreeController
    {
        protected override Umbraco.Web.Models.Trees.MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();
            if (id == "-1")
            {
                menu.DefaultMenuAlias = ActionNew.Instance.Alias;
                menu.Items.Add<ActionNew>("Create");
                menu.Items.Add<ActionRefresh>("Refresh nodes");
            }
            menu.Items.Add<ActionDelete>("Delete");
            return menu;
        }
        protected override Umbraco.Web.Models.Trees.TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();
            IContentType ct = Services.ContentTypeService.GetContentType("ProductElement");
            IEnumerable<IContent> contents = Services.ContentService.GetContentOfContentType(ct.Id).Where(c => c.Status != ContentStatus.Trashed);
            foreach (IContent item in contents)
            {
                nodes.Add(CreateTreeNode(item.Id.ToString(), ct.Id.ToString(), queryStrings, item.Name, ct.Icon, false));
            }
            return nodes;
        }
    }
}