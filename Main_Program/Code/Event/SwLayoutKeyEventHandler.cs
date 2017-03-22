using System;
using SAPbouiCOM;
using StatusBar = SwissAddonFramework.Messaging.StatusBar;

namespace EnerlifeCN.Code.Event
{
    internal class SwLayoutKeyEventHandler
    {
        public static void LayoutKeyEventEventHandler(ref LayoutKeyInfo eventinfo, out bool bubbleevent)
        {
            var bubbleevents = true;
            try
            {
                if (Global.SwFormsList.ContainsKey(eventinfo.FormUID))
                {
                    var swForm = Global.SwFormsList[eventinfo.FormUID];
                    swForm.LayoutKeyEventHandler(ref eventinfo, ref bubbleevents);
                }
            }
            catch (Exception ex)
            {
                StatusBar.WriteError("SwLayoutKeyEventHandler" + ex.Message, StatusBar.MessageTime.Short);
            }
            finally
            {
                bubbleevent = bubbleevents;
            }
        }
    }
}