using System;
using SAPbouiCOM;
using StatusBar = SwissAddonFramework.Messaging.StatusBar;

namespace EnerlifeCN.Code.Event
{
    internal class SwPrintEventHandler
    {
        public static void PrintEventHandler(ref PrintEventInfo eventinfo, ref bool bubbleevent)
        {
            try
            {
                foreach (var entry in Global.SwFormsList)
                {
                    var key = entry.Key;
                    if (key == eventinfo.FormUID)
                    {
                        var swForm = entry.Value;
                        swForm.PrintEventHandler(ref eventinfo, ref bubbleevent);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                StatusBar.WriteError("SwPrintEventHandler" + ex.Message, StatusBar.MessageTime.Short);
            }
        }
    }
}