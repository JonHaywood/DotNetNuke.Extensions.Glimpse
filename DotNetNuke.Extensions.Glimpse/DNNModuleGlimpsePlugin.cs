using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using Glimpse.Core.Extensibility;
using System;

namespace DotNetNuke.Extensions.Glimpse
{
    /// <summary>
    /// DotNetNuke Glimpse Plugin for DNN modules.
    /// </summary>
    [GlimpsePlugin]
    public class DNNModuleGlimpsePlugin : IGlimpsePlugin
    {
        public string Name
        {
            get { return "DotNetNuke Modules"; }
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
                var portalSettings = PortalSettings.Current;

                // if for some reason we don't have a tab ID, bail
                if (portalSettings.ActiveTab.TabID <= 0)
                    return null;
                // get modules on the page
                var modules = new ModuleController().GetTabModules(portalSettings.ActiveTab.TabID).Values.ToArray();

                // add to data to send
                var data = new List<object[]> { new object[] { "Module Name", "Module Properties" } };
                foreach (var module in modules)
                {
                    var moduleData = new List<object[]> { new object[] { "Property", "Value" } };
                    moduleData.Add(new object[] { "Module ID", module.ModuleID });
                    moduleData.Add(new object[] { "On all Tabs", module.AllTabs });
                    moduleData.Add(new object[] { "Cache Time", module.CacheTime });
                    moduleData.Add(new object[] { "Container Path", module.ContainerPath });
                    moduleData.Add(new object[] { "Container Src", module.ContainerSrc });
                    moduleData.Add(new object[] { "Header", module.Header });
                    moduleData.Add(new object[] { "Footer", module.Footer });
                    moduleData.Add(new object[] { "Inherit View Permissions", module.InheritViewPermissions });
                    moduleData.Add(new object[] { "Is Premium", module.DesktopModule.IsPremium });
                    moduleData.Add(new object[] { "Control Key", module.ModuleControl.ControlKey });
                    moduleData.Add(new object[] { "Control Source", module.ModuleControl.ControlSrc });
                    moduleData.Add(new object[] { "Permissions", module.DesktopModule.Permissions });
                    moduleData.Add(new object[] { "Pane", module.PaneName });
                    moduleData.Add(new object[] { "Start Date", module.StartDate });
                    moduleData.Add(new object[] { "End Date", module.EndDate });
                    moduleData.Add(new object[] { "Supports Partial Rendering", module.ModuleControl.SupportsPartialRendering });

                    // get the module settings from the DB
                    var settings = new ModuleController().GetModuleSettings(module.ModuleID);

                    // add to output data
                    var moduleSettings = new List<object[]> { new object[] { "Setting", "Value" } };
                    foreach (var settingKey in settings.Keys)
                        moduleSettings.Add(new object[] { settingKey.ToString(), settings[settingKey].ToString() });
                    moduleData.Add(new object[] { "Settings", moduleSettings });

                    // add to main data
                    data.Add(new object[] { (module.ModuleTitle ?? module.DesktopModule.FriendlyName), moduleData });
                }

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
