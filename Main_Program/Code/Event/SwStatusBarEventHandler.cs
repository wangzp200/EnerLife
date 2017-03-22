using System;
using SAPbouiCOM;
using StatusBar = SwissAddonFramework.Messaging.StatusBar;

namespace EnerlifeCN.Code.Event
{
    internal class SwStatusBarEventHandler
    {
        public static void StatusBarEventHandler(string text, BoStatusBarMessageType messagetype)
        {
            try
            {
                foreach (var entry in Global.SwFormsList)
                {
                    var key = entry.Key;
                    var swForm = entry.Value;
                    swForm.StatusBarEventHandler(text, messagetype);
                }
            }
            catch (Exception exception)
            {
                StatusBar.WriteError("SwStatusBarEventHandler" + exception.Message, StatusBar.MessageTime.Short);
            }
        }
    }
}