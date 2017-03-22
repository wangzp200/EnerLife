using SAPbouiCOM;

namespace EnerlifeCN.Code.FormExt.System._65308
{
    public class System65308 : SwBaseForm
    {
        public override void FormCreate(string formUId, ref ItemEvent pVal, ref bool bubbleEvent)
        {
            var xml = MyForm.GetAsXML();
        }

        public override void ItemEventHandler(string formUId, ref ItemEvent pVal, ref bool bubbleEvent)
        {
            if (pVal.BeforeAction && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && pVal.ItemUID == "36")
            {
                //var oItem = (Button)MyForm.Items.Item("36").Specific;
                //var oCfl = MyForm.ChooseFromLists.Item(oItem.ChooseFromListUID);
                //var conditions = oCfl.GetConditions();
                //if (conditions.Count > 0)
                //{
                //    conditions.Item(conditions.Count - 1).Relationship = SAPbouiCOM.BoConditionRelationship.cr_OR;
                //}
                //var condition = conditions.Add();
                //condition.BracketOpenNum = 2;
                //condition.Alias = "Confirmed";
                //condition.Operation = BoConditionOperation.co_EQUAL;
                //condition.CondVal = "N";
                //condition.BracketCloseNum = 1;
                //condition.Relationship = BoConditionRelationship.cr_AND;


                ////condition = conditions.Add();
                ////condition.BracketOpenNum = 1;
                ////condition.Alias = "CardCode";
                ////condition.Operation = BoConditionOperation.co_EQUAL;
                ////condition.CondVal = ((EditText)MyForm.Items.Item("4").Specific).Value.Trim();
                ////condition.BracketCloseNum = 1;
                ////condition.Relationship = BoConditionRelationship.cr_AND;

                //condition = conditions.Add();
                //condition.BracketOpenNum = 1;
                //condition.Alias = "DocStatus";
                //condition.Operation = BoConditionOperation.co_EQUAL;
                //condition.CondVal = "O";
                //condition.BracketCloseNum = 2;


                //oCfl.SetConditions(conditions);
            }
        }

        public override void FormDataAdd(ref BusinessObjectInfo businessobjectinfo, ref bool bubbleevent)
        {
            //if (!info.BeforeAction && info.ActionSuccess)
            //{
            //    Documents oOrder = Global.OCompany.GetBusinessObject(BoObjectTypes.oOrders);
            //    var docEntry = info.ObjectKey;
            //    var xmlDoc = new XmlDocument();
            //    xmlDoc.LoadXml(docEntry);
            //    var rootNode = xmlDoc.SelectSingleNode("DocumentParams/DocEntry");
            //    if (rootNode != null)
            //    {
            //        docEntry = rootNode.InnerText;
            //        var sb = new StringBuilder("SELECT DISTINCT BaseEntry FROM dbo.DPI1 t10 INNER JOIN dbo.ORDR t11 ON t10.BaseEntry = t11.DocEntry AND t11.DocStatus = 'O' AND t11.Confirmed = 'Y' ");
            //        sb.Append("WHERE  BaseType = '17' AND t10.DocEntry =").Append(docEntry);

            //        Global.ORecordSet.DoQuery(sb.ToString());
            //        if (Global.ORecordSet.EoF == false)
            //        {
            //            var setp = 1;
            //            var oProgBar = Global.Application.StatusBar.CreateProgressBar("Auto update Purchase Order,Please Waiting....", Global.ORecordSet.RecordCount, false);

            //            while (Global.ORecordSet.EoF == false)
            //            {
            //                oProgBar.Value = setp;
            //                var ordrEntry = int.Parse(Global.ORecordSet.Fields.Item(0).Value.ToString());
            //                Global.Application.SetStatusBarMessage("Update Purchase Order：" + ordrEntry.ToString(), BoMessageTime.bmt_Long, false);
            //                oOrder.GetByKey(ordrEntry);
            //                oOrder.Confirmed = BoYesNoEnum.tNO;
            //                oOrder.Update();
            //                Global.ORecordSet.MoveNext();
            //                setp = setp + 1;
            //            }
            //            Global.Application.SetStatusBarMessage("Update Purchase Order Complated！ ", BoMessageTime.bmt_Long, false);
            //            oProgBar.Stop();
            //            oProgBar = null;
            //        }

            //    }

            //}
        }
    }
}