using System;
using SAPbouiCOM;
using StatusBar = SwissAddonFramework.Messaging.StatusBar;

namespace EnerlifeCN.Code.Event
{
    internal class SwRightClickHandler
    {
        public static void RightClickHandler(ref ContextMenuInfo eventinfo, ref bool bubbleevent)
        {
            try
            {
                foreach (var entry in Global.SwFormsList)
                {
                    var key = entry.Key;
                    if (key == eventinfo.FormUID)
                    {
                        var swForm = entry.Value;
                        swForm.RightClickHandler(ref eventinfo, ref bubbleevent);
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                StatusBar.WriteError("SwRightClickHandler" + exception.Message, StatusBar.MessageTime.Short);
            }
        }
    }
}