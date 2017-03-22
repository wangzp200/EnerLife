using System.Text;
using System.Xml;
using SAPbobsCOM;
using SAPbouiCOM;

namespace EnerlifeCN.Code.FormExt.System._170
{
    public class System170 : SwBaseForm
    {
        public override void FormDataAdd(ref BusinessObjectInfo businessobjectinfo, ref bool bubbleevent)
        {
            if (!businessobjectinfo.BeforeAction && businessobjectinfo.ActionSuccess)
            {
                var oOrder = Global.OCompany.GetBusinessObject(BoObjectTypes.oOrders) as Documents;
                var docEntry = businessobjectinfo.ObjectKey;
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(docEntry);
                var rootNode = xmlDoc.SelectSingleNode("PaymentParams/DocEntry");
                if (rootNode != null)
                {
                    docEntry = rootNode.InnerText;
                    var sb = new StringBuilder("EXECUTE CheckPurchaseOrders ");
                    sb.Append(docEntry);
                    Global.ORecordSet.DoQuery(sb.ToString());
                    if (Global.ORecordSet.EoF == false)
                    {
                        var setp = 1;
                        var oProgBar =
                            Global.Application.StatusBar.CreateProgressBar(
                                "Auto update Purchase Order,Please Waiting....", Global.ORecordSet.RecordCount, false);

                        while (Global.ORecordSet.EoF == false)
                        {
                            oProgBar.Value = setp;
                            var ordrEntry = int.Parse(Global.ORecordSet.Fields.Item(0).Value.ToString());
                            Global.Application.SetStatusBarMessage("Update Purchase Order：" + ordrEntry,
                                BoMessageTime.bmt_Long, false);
                            if (oOrder != null)
                            {
                                oOrder.GetByKey(ordrEntry);
                                oOrder.Confirmed = BoYesNoEnum.tYES;
                                oOrder.Update();
                            }
                            Global.ORecordSet.MoveNext();
                            setp = setp + 1;
                        }
                        Global.Application.SetStatusBarMessage("Update Purchase Order Complated！ ",
                            BoMessageTime.bmt_Long, false);
                        oProgBar.Stop();
                    }
                }
            }
        }
    }
}