using System.Xml;
using SAPbobsCOM;
using SAPbouiCOM;

namespace EnerlifeCN.Code.FormExt.System._133
{
    public class System133 : SwBaseForm
    {
        public override void FormDataAdd(ref BusinessObjectInfo businessobjectinfo, ref bool bubbleevent)
        {
            //创建成功之后对折扣进行做
            if (!businessobjectinfo.BeforeAction && businessobjectinfo.ActionSuccess)
            {
                var docEntry = businessobjectinfo.ObjectKey;
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(docEntry);
                var rootNode = xmlDoc.SelectSingleNode("DocumentParams/DocEntry");
                if (rootNode != null)
                {
                    docEntry = rootNode.InnerText;

                    var sql =
                        "SELECT TOP 1 t20.U_Debit,t10.AcctCode FROM dbo.INV1 t10 INNER JOIN dbo.[@COR0240] t20 ON t10.AcctCode=t20.U_Credit where DocEntry=" +
                        docEntry;
                    Global.ORecordSet.DoQuery(sql);
                    if (Global.ORecordSet.EoF == false)
                    {
                        var debit = Global.ORecordSet.Fields.Item(0).Value.ToString();
                        var credit = Global.ORecordSet.Fields.Item(1).Value.ToString();

                        sql =
                            "SELECT TOP 1 t12.TransId,t10.DiscSum,t10.DocNum FROM dbo.OINV t10 INNER JOIN dbo.OJDT t11 ON t10.DocNum=t11.BaseRef AND t10.ObjType=t11.TransType INNER JOIN dbo.JDT1 t12 ON  t11.TransId=t12.TransId AND t11.TransType=t12.TransType WHERE t12.DebCred='C' AND t12.VatLine='N' AND t10.DocEntry=" +
                            docEntry + " and t12.Account='" + credit + "'";
                        Global.ORecordSet.DoQuery(sql);
                        if (Global.ORecordSet.EoF == false)
                        {
                            var transId = Global.ORecordSet.Fields.Item(0).Value.ToString();
                            var discSum = Global.ORecordSet.Fields.Item(1).Value.ToString();
                            var docNum = Global.ORecordSet.Fields.Item(2).Value.ToString();
                            var vIn = Global.OCompany.GetBusinessObject(BoObjectTypes.oJournalEntries) as JournalEntries;
                            if (vIn != null)
                            {
                                vIn.GetByKey(int.Parse(transId));

                                sql =
                                    "SELECT SUM (U_DiscountTotal/(1+VatPrcnt/100.0)) AS DiscountTotal FROM dbo.INV1 t10 WHERE DocEntry=" +
                                    docEntry;
                                Global.ORecordSet.DoQuery(sql);
                                var total = double.Parse(Global.ORecordSet.Fields.Item(0).Value.ToString());

                                var vJe =
                                    Global.OCompany.GetBusinessObject(BoObjectTypes.oJournalEntries) as JournalEntries;
                                if (vJe != null)
                                {
                                    vJe.ReferenceDate = vIn.ReferenceDate;
                                    vJe.DueDate = vIn.DueDate;
                                    vJe.TaxDate = vIn.TaxDate;
                                    vJe.Reference3 = docNum;


                                    vJe.Lines.SetCurrentLine(0);
                                    vJe.Lines.AccountCode = debit;
                                    vJe.Lines.ContraAccount = credit;
                                    vJe.Lines.Debit = total + double.Parse(discSum);
                                    vJe.Lines.Credit = 0;
                                    vJe.Lines.DueDate = vIn.DueDate;
                                    vJe.Lines.ReferenceDate1 = vIn.ReferenceDate;
                                    vJe.Lines.TaxDate = vIn.TaxDate;

                                    vJe.Lines.ShortName = debit;
                                    vJe.Lines.Add();

                                    vJe.Lines.SetCurrentLine(1);
                                    vJe.Lines.AccountCode = credit;
                                    vJe.Lines.ContraAccount = debit;
                                    vJe.Lines.Credit = total + double.Parse(discSum);
                                    vJe.Lines.Debit = 0;
                                    vJe.Lines.DueDate = vIn.DueDate;
                                    vJe.Lines.ReferenceDate1 = vIn.ReferenceDate;
                                    vJe.Lines.TaxDate = vIn.TaxDate;
                                    vJe.Lines.ShortName = credit;

                                    long retVal = vJe.Add();
                                    if (retVal != 0)
                                    {
                                        var errCode = 0;
                                        string errMsg;
                                        Global.OCompany.GetLastError(out errCode, out errMsg);
                                        Global.Application.SetStatusBarMessage(errMsg + " errorCode:" + errCode,
                                            BoMessageTime.bmt_Short);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}