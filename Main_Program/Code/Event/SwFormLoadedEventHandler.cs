using System;
using SwissAddonFramework.Messaging;

namespace EnerlifeCN.Code.Event
{
    internal class SwFormLoadedEventHandler
    {
        public static void FormLoadedEventHandler(string formuid, string formtypeex, object pval, ref bool bubbleevent)
        {
            try
            {
                foreach (var entry in Global.SwFormsList)
                {
                    var key = entry.Key;
                    if (key == formuid)
                    {
                        var swForm = entry.Value;
                        swForm.FormLoadedEventHandler(formuid, formtypeex, pval, ref bubbleevent);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                StatusBar.WriteError("SwFormLoadedEventHandler" + ex.Message, StatusBar.MessageTime.Short);
            }
        }
    }
}