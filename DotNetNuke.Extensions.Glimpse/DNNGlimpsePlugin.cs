using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using Glimpse.Core.Extensibility;

namespace DotNetNuke.Extensions.Glimpse
{
    /// <summary>
    /// DotNetNuke Glimpse Plugin.
    /// </summary>
    [GlimpsePlugin]
    public class DNNGlimpsePlugin : IGlimpsePlugin
    {
        public string Name
        {
            get { return "DotNetNuke"; }
        }

        /// <summary>
        /// Gets the data to send to the Glimpse client.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Data to send the the Glimpse client.</returns>
        public object GetData(HttpContextBase context)
        {
            try
            {
                // get variables we'll need to output
                var portalSettings = PortalSettings.Current;
                var tabCreatedByUser = UserController.GetUserById(-1, portalSettings.ActiveTab.CreatedByUserID);
                var tabModifiedByUser = UserController.GetUserById(-1, portalSettings.ActiveTab.LastModifiedByUserID);
                var portalAliases = new PortalAliasController().GetPortalAliasArrayByPortalID(portalSettings.PortalId)
                    .Cast<PortalAliasInfo>()
                    .Select(p => p.HTTPAlias);
                var contextItems = new List<object[]> { new object[] { "Key", "Value" } };
                foreach (var itemKey in context.Items.Keys)                
                    contextItems.Add(new object[] { itemKey.ToString(), context.Items[itemKey].ToString() });                
                
                // add to data to send
                var data = new List<object[]> { new object[] { "Property", "Value" } };
                data.Add(new object[] { "PortalID", portalSettings.PortalId });
                data.Add(new object[] { "Portal Name", portalSettings.PortalName });
                data.Add(new object[] { "Portal Aliases", portalAliases.ToArray() });
                data.Add(new object[] { "Portal SSL Enabled", portalSettings.SSLEnabled });
                data.Add(new object[] { "Portal SSL Enforced", portalSettings.SSLEnforced });
                data.Add(new object[] { "User ID", portalSettings.UserId });
                data.Add(new object[] { "User Name", portalSettings.UserInfo.Username });
                data.Add(new object[] { "User Roles", portalSettings.UserInfo.Roles });
                data.Add(new object[] { "Tab ID", portalSettings.ActiveTab.TabID });
                data.Add(new object[] { "Tab Name", portalSettings.ActiveTab.TabName });
                data.Add(new object[] { "Tab Title", portalSettings.ActiveTab.Title });
                data.Add(new object[] { "Tab Path", portalSettings.ActiveTab.TabPath });
                data.Add(new object[] { "Tab SSL Enabled", portalSettings.ActiveTab.IsSecure });
                data.Add(new object[] { "Tab Created By", (tabCreatedByUser == null) ? null : tabCreatedByUser.Username });
                data.Add(new object[] { "Tab Created Date", portalSettings.ActiveTab.CreatedOnDate });
                data.Add(new object[] { "Tab Modified By", (tabModifiedByUser == null) ? null : tabModifiedByUser.Username });
                data.Add(new object[] { "Tab Modified Date", portalSettings.ActiveTab.LastModifiedOnDate });
                data.Add(new object[] { "Tab Skin Path", portalSettings.ActiveTab.SkinPath });
                data.Add(new object[] { "Tab Skin Source", portalSettings.ActiveTab.SkinSrc });
                data.Add(new object[] { "Context Items", contextItems });                                

                return data;
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                return null;
            }
        }        

        public void SetupInit()
        {
            // nothing to do here right now
        }
    }
}
