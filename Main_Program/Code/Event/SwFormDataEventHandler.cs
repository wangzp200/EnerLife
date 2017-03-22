using System;
using SAPbouiCOM;
using StatusBar = SwissAddonFramework.Messaging.StatusBar;

namespace EnerlifeCN.Code.Event
{
    internal class SwFormDataEventHandler
    {
        public static void FormDataEventHandler(ref BusinessObjectInfo businessobjectinfo, ref bool bubbleevent)
        {
            try
            {
                foreach (var entry in Global.SwFormsList)
                {
                    var key = entry.Key;
                    if (key == businessobjectinfo.FormUID)
                    {
                        var swForm = entry.Value;
                        swForm.FormDataEventHandler(ref businessobjectinfo, ref bubbleevent);
                        switch (businessobjectinfo.EventType)
                        {
                            case BoEventTypes.et_FORM_DATA_ADD:
                                swForm.FormDataAdd(ref businessobjectinfo, ref bubbleevent);
                                break;
                            case BoEventTypes.et_FORM_DATA_UPDATE:
                                swForm.FormDataUpUpdate(ref businessobjectinfo, ref bubbleevent);
                                break;
                            case BoEventTypes.et_FORM_DATA_DELETE:
                                swForm.FormDataDelete(ref businessobjectinfo, ref bubbleevent);
                                break;
                            case BoEventTypes.et_FORM_DATA_LOAD:
                                swForm.FormDataLoad(ref businessobjectinfo, ref bubbleevent);
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                StatusBar.WriteError("SwFormDataEventHandler:" + ex.Message, StatusBar.MessageTime.Short);
            }
        }
    }
}