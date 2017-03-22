using System;
using EnerlifeCN.Code.Util;
using SAPbouiCOM;
using StatusBar = SwissAddonFramework.Messaging.StatusBar;

namespace EnerlifeCN.Code.Event
{
    internal class SwMenuEventHandler
    {
        public static void MenuEventHandler(ref MenuEvent pval, ref bool bubbleevent)
        {
            try
            {
                if (!pval.BeforeAction)
                {
                    var formType = pval.MenuUID;

                    var swForm = CreateNewFormUtil.CreateNewForm(formType, -1, -1);
                    if (swForm != null)
                    {
                        Global.CurrentForm = swForm;
                    }
                }
                if (pval.BeforeAction)
                {
                    Global.CurrentForm = Global.Application.Forms.ActiveForm;
                }

                foreach (var entry in Global.SwFormsList)
                {
                    var swForm = entry.Value;
                    if (Global.CurrentForm.UniqueID == swForm.MyFormUid)
                    {
                        if (swForm.MyForm == null)
                            swForm.MyForm = Global.CurrentForm;
                        swForm.MenuEventHandler(ref pval, ref bubbleevent);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                StatusBar.WriteError("SwMenuEventHandler" + ex.Message, StatusBar.MessageTime.Short);
            }
        }
    }
}