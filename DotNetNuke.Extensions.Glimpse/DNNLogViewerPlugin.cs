using System;
using System.Collections.Generic;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Log.EventLog;
using Glimpse.Core.Extensibility;

namespace DotNetNuke.Extensions.Glimpse
{
    /// <summary>
    /// DotNetNuke Glimpse Plugin for DNN Log Viewer.
    /// </summary>
    [GlimpsePlugin]
    public class DNNLogViewerPlugin : IGlimpsePlugin
    {
        public string Name
        {
            get { return "DotNetNuke Log Viewer"; }
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

                // get the logs
                int totalRecords = 0;
                LogInfoArray logs = new LogController().GetLog(portalSettings.PortalId, 15, 1, ref totalRecords);

                // bail if we don't have any
                if (logs.Count == 0)
                    return null;

                // add to data to send
                var data = new List<object[]> {new object[] {"Created Date", "Log Type", "UserName", "Content"}};
                for (int i = 0; i < logs.Count; i++)
                {
                    var log = logs.GetItem(i);

                    // get log properties 
                    var logProperties = new List<object[]> {new object[] {"Property", "Value"}};
                    for (int j = 0; j < log.LogProperties.Count; j++)
                    {
                        var logDetail = log.LogProperties[j] as LogDetailInfo;
                        logProperties.Add(new object[] {logDetail.PropertyName, logDetail.PropertyValue});
                    }

                    data.Add(new object[] {log.LogCreateDate, log.LogTypeKey, log.LogUserName, logProperties});
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
