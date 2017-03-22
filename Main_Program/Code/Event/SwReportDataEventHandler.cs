using System;
using SAPbouiCOM;
using StatusBar = SwissAddonFramework.Messaging.StatusBar;

namespace EnerlifeCN.Code.Event
{
    internal class SwReportDataEventHandler
    {
        public static void ReportDataEventHandler(ref ReportDataInfo eventinfo, ref bool bubbleevent)
        {
            try
            {
                foreach (var entry in Global.SwFormsList)
                {
                    var key = entry.Key;
                    if (key == eventinfo.FormUID)
                    {
                        var swForm = entry.Value;
                        swForm.ReportDataEventHandler(ref eventinfo, ref bubbleevent);
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                StatusBar.WriteError("SwReportDataEventHandler" + exception.Message, StatusBar.MessageTime.Short);
            }
        }
    }
}