using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using umbraco;
using umbraco.businesslogic;
using umbraco.BusinessLogic.Actions;
using umbraco.interfaces;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace Bytefunds.Cms.Logic.CustomSection
{

    [Application("SystemSettings", "SystemSettings", "icon-settings-alt", 14)]
    public class SystemSettingsApplication : IApplication { }

    [PluginController("SystemSettings")]
    [Umbraco.Web.Trees.Tree("SystemSettings", "SystemSettings", "SystemSettings", iconClosed: "icon-settings-alt", iconOpen: "icon-settings-alt")]
    public class SystemSettingsTreeController : TreeController
    {
        private IContentType contentType_settings;
        private ServiceContext Services;
        public SystemSettingsTreeController()
        {
            Services = ApplicationContext.Current.Services;
            contentType_settings = Services.ContentTypeService.GetContentType("Setting");


        }
        protected override Umbraco.Web.Models.Trees.MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            MenuItemCollection menuItemCollection = new MenuItemCollection();
            if (id == "-1")
            {
                menuItemCollection.Items.Add<ActionNew>(ui.Text("actions", ActionNew.Instance.Alias));
            }
            else
            {
                menuItemCollection.Items.Add<ActionDelete>(ui.Text("delete", ActionDelete.Instance.Alias));
            }
            menuItemCollection.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true, null);

            return menuItemCollection;
        }

        protected override Umbraco.Web.Models.Trees.TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            TreeNodeCollection currentTreeNodes = new TreeNodeCollection();
            if (id == "-1")
            {
                IEnumerable<IContent> allSettings = Services.ContentService.GetContentOfContentType(contentType_settings.Id).Where(r => r.Trashed.Equals(false));
                foreach (IContent setting in allSettings)
                {
                    TreeNode node = this.CreateTreeNode(setting.Id.ToString(), id, queryStrings, setting.Name, contentType_settings.Icon, false);
                    currentTreeNodes.Add(node);
                }
            }

            return currentTreeNodes;
        }


    }
}