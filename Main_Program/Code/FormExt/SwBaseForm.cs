using System.Collections.Generic;
using SAPbouiCOM;

namespace EnerlifeCN.Code.FormExt
{
    public class SwBaseForm
    {
        public bool Active;
        public EventForm Event;

        public SwBaseForm()
        {
            Fieldlist = new SortedList<string, string>();
            Active = false;
        }

        public string MyFormUid { get; set; }
        public Form MyForm { get; set; }
        public string MyFatherUid { set; get; }
        public SortedList<string, string> Fieldlist { get; set; }

        public virtual void ApplicationEventHandler(BoAppEventTypes eventType)
        {
        }

        public virtual void FormDataEventHandler(ref BusinessObjectInfo businessObjectInfo, ref bool bubbleEvent)
        {
        }

        public virtual void FormLoadedEventHandler(string formUid, string formTypeEx, object pVal, ref bool bubbleEvent)
        {
        }

        public virtual void ItemEventHandler(string formUid, ref ItemEvent pVal, ref bool bubbleEvent)
        {
        }

        public virtual void MenuEventHandler(ref MenuEvent pVal, ref bool bubbleEvent)
        {
        }

        public virtual void PrintEventHandler(ref PrintEventInfo eventInfo, ref bool bubbleEvent)
        {
        }

        public virtual void ProgressBarEventHandler(ref ProgressBarEvent pVal, ref bool bubbleEvent)
        {
        }

        public virtual void ReportDataEventHandler(ref ReportDataInfo eventinfo, ref bool bubbleEvent)
        {
        }

        public virtual void RightClickHandler(ref ContextMenuInfo eventInfo, ref bool bubbleEvent)
        {
        }

        public virtual void StatusBarEventHandler(string text, BoStatusBarMessageType messageType)
        {
        }

        public virtual void FormDataAdd(ref BusinessObjectInfo businessobjectinfo, ref bool bubbleevent)
        {
        }

        public virtual void FormDataUpUpdate(ref BusinessObjectInfo businessobjectinfo, ref bool bubbleevent)
        {
        }

        public virtual void FormDataDelete(ref BusinessObjectInfo businessobjectinfo, ref bool bubbleevent)
        {
        }

        public virtual void FormDataLoad(ref BusinessObjectInfo businessobjectinfo, ref bool bubbleevent)
        {
        }

        public virtual void FormCreate(string formUId, ref ItemEvent pVal, ref bool bubbleEvent)
        {
        }

        public virtual void LayoutKeyEventHandler(ref LayoutKeyInfo eventinfo, ref bool bubbleevent)
        {
        }
    }
}