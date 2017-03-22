using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using EnerlifeCN.Code.FormExt.Other.MessageInfo;
using EnerlifeCN.Code.Util;
using SAPbobsCOM;
using SAPbouiCOM;
using ChooseFromList = SAPbouiCOM.ChooseFromList;
using DataTable = System.Data.DataTable;

namespace EnerlifeCN.Code.FormExt.System._139
{
    public class System139 : SwBaseForm
    {
        private EditText _availPer;
        private string _conditionsXml;
        private DataTable _dtTmp;
        private bool _hasAuthority;
        private Button _ioBt;
        private Matrix _ioMtx;
        private DBDataSource _ioOrdr, _ioRdr1;
        private UserDataSource _ioUdbsTotal, _ioUdbsDueDate;
        private ChooseFromList _owerChooseFromList;
        private ComboBox _positionBox;
        private decimal tmpDiscValue;

        public override void FormCreate(string formUId, ref ItemEvent pVal, ref bool bubbleEvent)
        {
            _owerChooseFromList =
                MyForm.ChooseFromLists.Item(((EditText) MyForm.Items.Item("222").Specific).ChooseFromListUID);
            var conditions = _owerChooseFromList.GetConditions();
            _conditionsXml = conditions.GetAsXML();
            _ioOrdr = MyForm.DataSources.DBDataSources.Item("ORDR");
            _ioRdr1 = MyForm.DataSources.DBDataSources.Item("RDR1");
            _ioMtx = MyForm.Items.Item("38").Specific as Matrix;
            var uid = "Calculate";
            _ioBt = MyForm.Items.Add(uid, BoFormItemTypes.it_BUTTON).Specific as Button;
            if (_ioBt != null)
            {
                _ioBt.Caption = "Calculate";
                MyForm.Items.Item(uid)
                    .SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
                MyForm.Items.Item(uid)
                    .SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 1, BoModeVisualBehavior.mvb_True);
                MyForm.Items.Item(uid)
                    .SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 2, BoModeVisualBehavior.mvb_True);
            }
            uid = "COdpi";
            _ioBt = MyForm.Items.Add(uid, BoFormItemTypes.it_BUTTON).Specific as Button;
            if (_ioBt != null) _ioBt.Caption = "Create DownPayment";
            MyForm.Items.Item(uid)
                .SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, -1, BoModeVisualBehavior.mvb_False);
            MyForm.Items.Item(uid)
                .SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable, 1, BoModeVisualBehavior.mvb_True);

            uid = "CTotal";
            _ioUdbsTotal = MyForm.DataSources.UserDataSources.Add(uid, BoDataType.dt_SUM);
            var oItem1 = (EditText) MyForm.Items.Add(uid, BoFormItemTypes.it_EDIT).Specific;
            oItem1.DataBind.SetBound(true, "", uid);
            MyForm.Items.Item(uid).RightJustified = true;
            MyForm.Items.Item(uid).AffectsFormMode = false;

            MyForm.Items.Item(uid).BackColor = Color.FromArgb(144, 238, 144).ToArgb();
            var luid2 = uid + "L";

            var oitm2 = MyForm.Items.Add(luid2, BoFormItemTypes.it_STATIC);
            ((StaticText) oitm2.Specific).Caption = "DownPayment Total";
            oitm2.LinkTo = uid;

            uid = "PostDate";
            _ioUdbsDueDate = MyForm.DataSources.UserDataSources.Add(uid, BoDataType.dt_DATE);
            var oItem3 = (EditText) MyForm.Items.Add(uid, BoFormItemTypes.it_EDIT).Specific;
            oItem3.DataBind.SetBound(true, "", uid);
            MyForm.Items.Item(uid).RightJustified = true;
            MyForm.Items.Item(uid).AffectsFormMode = false;
            MyForm.Items.Item(uid).BackColor = Color.FromArgb(144, 238, 144).ToArgb();
            var luid = uid + "L";

            var oitm4 = MyForm.Items.Add(luid, BoFormItemTypes.it_STATIC);
            ((StaticText) oitm4.Specific).Caption = "Posting Date";
            oitm4.LinkTo = uid;


            uid = "AvailPer";
            var oitm5 = (EditText) MyForm.Items.Add(uid, BoFormItemTypes.it_EDIT).Specific;
            oitm5.DataBind.SetBound(true, _ioOrdr.TableName, "U_AvailablePeriod");
            MyForm.Items.Item(uid).RightJustified = true;
            oitm5.BackColor = Color.FromArgb(144, 238, 144).ToArgb();
            _availPer = oitm5;
            luid = uid + "L";
            var oitm6 = MyForm.Items.Add(luid, BoFormItemTypes.it_STATIC);
            ((StaticText) oitm6.Specific).Caption = "Available Period";
            oitm6.LinkTo = uid;

            var oitm7 = MyForm.Items.Item("230");
            ((StaticText) oitm7.Specific).Caption = "Employee Sales";


            uid = "position";
            var oitm8 = (ComboBox) MyForm.Items.Add(uid, BoFormItemTypes.it_COMBO_BOX).Specific;
            oitm8.DataBind.SetBound(true, _ioOrdr.TableName, "U_position");
            oitm8.Item.BackColor = Color.FromArgb(144, 238, 144).ToArgb();
            //oitm8.Item.SetAutoManagedAttribute(BoAutoManagedAttr.ama_Editable,-1, BoModeVisualBehavior.mvb_False );
            _positionBox = oitm8;
            oitm8.Item.DisplayDesc = true;

            luid = uid + "L";
            var oitm9 = MyForm.Items.Add(luid, BoFormItemTypes.it_STATIC);
            ((StaticText) oitm9.Specific).Caption = "Position";
            oitm9.LinkTo = uid;

            var sb = new StringBuilder("SELECT posID,name FROM ohps");
            Global.ORecordSet.DoQuery(sb.ToString());
            var validValues = oitm8.ValidValues;
            validValues.Add("", "");
            while (Global.ORecordSet.EoF == false)
            {
                validValues.Add(Global.ORecordSet.Fields.Item(0).Value.ToString(),
                    Global.ORecordSet.Fields.Item(1).Value.ToString());
                Global.ORecordSet.MoveNext();
            }

            sb = new StringBuilder("select U_HasAuthority from ousr where USER_CODE=");
            sb.Append("'").Append(Global.Application.Company.UserName).Append("'");
            Global.ORecordSet.DoQuery(sb.ToString());
            if (Global.ORecordSet.EoF == false)
            {
                if (Global.ORecordSet.Fields.Item(0).Value.Equals("Y"))
                {
                    _hasAuthority = true;
                }
            }

            //指定特定的列显示可编辑
            if (_dtTmp == null)
            {
                _dtTmp = new DataTable("Table_AX");
                _dtTmp.Columns.Add("LineNum", typeof (int));
                _dtTmp.Columns.Add("ItemCode", typeof (string));
                _dtTmp.Columns.Add("Quantity", typeof (decimal));
                _dtTmp.Columns.Add("Price", typeof (decimal));
                _dtTmp.Columns.Add("Uom", typeof (string));
                _dtTmp.Columns.Add("GPBD", typeof (decimal));
                _dtTmp.Columns.Add("RDiscount", typeof (decimal));
                _dtTmp.Columns.Add("EDiscount", typeof (decimal));
                _dtTmp.Columns.Add("SDiscount", typeof (decimal));
                _dtTmp.Columns.Add("IsFree", typeof (string));
                _dtTmp.Columns.Add("DisTotal", typeof (decimal));
                _dtTmp.Columns.Add("UomEntry", typeof (string));
                _dtTmp.Columns.Add("Udiscount", typeof (decimal));
            }


            var c = _ioMtx.Columns.Item("1470002145").ChooseFromListUID;
            if (!string.IsNullOrEmpty(c))
            {
                var cfl = MyForm.ChooseFromLists.Item(c);
            }
        }

        public override void ItemEventHandler(string formUId, ref ItemEvent pVal, ref bool bubbleEvent)
        {
            //------------------------窗体大小变化-------------------------------------------------------------------------

            if (!pVal.BeforeAction && pVal.EventType == BoEventTypes.et_FORM_RESIZE)
            {
                MyFormResize();
            }


            //触发重新计算
            if (!pVal.BeforeAction && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && pVal.ItemUID == "Calculate")
            {
                var status = _ioOrdr.GetValue("DocStatus", 0);

                if (status.Equals("O"))
                {
                    tmpDiscValue = (decimal) 0.0;
                    _dtTmp.Rows.Clear();
                    //首先删除销售订单中的免费商品标准U_IsFreeGood='Y'
                    var deleteRows = new List<int>();
                    var builder = new StringBuilder("(");
                    for (var i = 1; i <= _ioMtx.RowCount; i++)
                    {
                        var itemCode = ((EditText) _ioMtx.Columns.Item("1").Cells.Item(i).Specific).Value;
                        var quantity = ((EditText) _ioMtx.Columns.Item("11").Cells.Item(i).Specific).Value;

                        var isFreeGood = ((ComboBox) _ioMtx.Columns.Item("U_IsFreeGood").Cells.Item(i).Specific).Value;
                        var deliveredQty = ((EditText) _ioMtx.Columns.Item("10000312").Cells.Item(i).Specific).Value;
                        var uom = ((EditText) _ioMtx.Columns.Item("1470002145").Cells.Item(i).Specific).Value;
                        if (!string.IsNullOrEmpty(itemCode) && !string.IsNullOrEmpty(uom) &&
                            decimal.Parse(deliveredQty) <= 0 && isFreeGood == "N")
                        {
                            builder.Append("'").Append(itemCode).Append("'").Append(",");
                        }
                        else if (!string.IsNullOrEmpty(itemCode) && isFreeGood == "Y")
                        {
                            deleteRows.Add(i);
                        }
                    }


                    for (var i = deleteRows.Count - 1; i >= 0; i--)
                    {
                        _ioMtx.SetCellFocus(deleteRows[i], 3);
                        _ioMtx.Columns.Item("0").Cells.Item(deleteRows[i]).Click(BoCellClickType.ct_Regular);
                        var menu = Global.Application.Menus.Item("1293");
                        if (menu.Enabled)
                        {
                            menu.Activate();
                        }
                    }

                    for (var i = 1; i <= _ioMtx.RowCount; i++)
                    {
                        var itemCode = ((EditText) _ioMtx.Columns.Item("1").Cells.Item(i).Specific).Value;
                        var quantity = ((EditText) _ioMtx.Columns.Item("11").Cells.Item(i).Specific).Value;
                        var isFreeGood = ((ComboBox) _ioMtx.Columns.Item("U_IsFreeGood").Cells.Item(i).Specific).Value;
                        var Udiscount = ((ComboBox) _ioMtx.Columns.Item("U_Udiscount").Cells.Item(i).Specific).Value;
                        var deliveredQty = ((EditText) _ioMtx.Columns.Item("10000312").Cells.Item(i).Specific).Value;
                        var uom = ((EditText) _ioMtx.Columns.Item("1470002145").Cells.Item(i).Specific).Value;
                        if (!string.IsNullOrEmpty(itemCode) && !string.IsNullOrEmpty(uom) &&
                            decimal.Parse(deliveredQty) <= 0 && isFreeGood == "N")
                        {
                            var obj = new object[]
                            {
                                i, itemCode, decimal.Parse(quantity), 0, uom, 0, 0, 0, 0, isFreeGood, 0, "",
                                decimal.Parse(Udiscount)
                            };
                            _dtTmp.Rows.Add(obj);
                        }
                    }

                    Form messageForm = null;
                    try
                    {
                        var ouomInfos = new List<OuomInfo>();
                        if (builder.Length > 1)
                        {
                            builder.Remove(builder.Length - 1, 1);
                            builder.Append(")");
                            var sql =
                                "SELECT t10.ItemCode, t11.UomEntry,AltQty,BaseQty,t12.UomCode FROM OITM t10 INNER JOIN UGP1 t11 ON t10.UgpEntry=t11.UgpEntry INNER JOIN	dbo.OUOM t12 ON t12.UomEntry = t11.UomEntry WHERE ItemCode in " +
                                builder;
                            Global.ORecordSet.DoQuery(sql);
                            while (Global.ORecordSet.EoF == false)
                            {
                                var ouomInfo = new OuomInfo
                                {
                                    ItemCode = Global.ORecordSet.Fields.Item(0).Value.ToString(),
                                    UomEntry = int.Parse(Global.ORecordSet.Fields.Item(1).Value.ToString()),
                                    AltQty = decimal.Parse(Global.ORecordSet.Fields.Item(2).Value.ToString()),
                                    BaseQty = decimal.Parse(Global.ORecordSet.Fields.Item(3).Value.ToString()),
                                    UomCode = Global.ORecordSet.Fields.Item(4).Value.ToString()
                                };
                                ouomInfos.Add(ouomInfo);
                                Global.ORecordSet.MoveNext();
                            }
                        }

                        MyForm.Freeze(true);
                        var formType = "MessageInfo";
                        messageForm = CreateNewFormUtil.CreateNewForm(formType, MyForm.Top + MyForm.Height/2 - 22,
                            MyForm.Left + MyForm.Width/2 - 140);
                        var message = "             Please Waiting....";
                        var messageInfo = (MessageInfo) Global.SwFormsList[messageForm.UniqueID];
                        messageInfo.SetMessage(message);

                        Global.Application.SetStatusBarMessage(message, BoMessageTime.bmt_Long, false);

                        CalculateGoodPrices();
                        CalculateFreeGoods(ouomInfos);
                        CalculateDiscount(ouomInfos);
                        message = "Calculated";
                        Global.Application.SetStatusBarMessage(message, BoMessageTime.bmt_Short, false);
                        MyForm.Refresh();
                    }
                    catch (Exception e)
                    {
                        if (messageForm != null)
                        {
                            messageForm.Close();
                        }
                        Global.Application.SetStatusBarMessage(e.Message, BoMessageTime.bmt_Short, true);
                    }
                    finally
                    {
                        MyForm.Freeze(false);
                        if (messageForm != null)
                        {
                            messageForm.Close();
                        }
                    }
                }
            }
            // 审批通过触发事件
            if (pVal.BeforeAction && pVal.EventType == BoEventTypes.et_CLICK && pVal.ItemUID == "136")
            {
                if (!_hasAuthority)
                {
                    var message = "You do not have Authority！";
                    Global.Application.SetStatusBarMessage(message, BoMessageTime.bmt_Short);
                    bubbleEvent = false;
                }
            }

            //---------------------------------------------------------------------------------------------
            if (pVal.BeforeAction && pVal.ItemUID == "COdpi" && pVal.EventType == BoEventTypes.et_ITEM_PRESSED)
            {
                if (MyForm.Mode == BoFormMode.fm_OK_MODE)
                {
                    var select = Global.Application.MessageBox("Are you sure ?", 2, "Yes", "No");
                    if (select == 1)
                    {
                        Global.Application.SetStatusBarMessage("Creating Down Payment,Please waiting....",
                            BoMessageTime.bmt_Long, false);
                        CreateDownPayment();
                    }
                }
                else
                {
                    var message = "Mode Of Form should be OK!";
                    Global.Application.SetStatusBarMessage(message, BoMessageTime.bmt_Short, true);
                }
            }
            //单据选择客户时
            if (!pVal.BeforeAction && pVal.ItemUID == "4" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST &&
                pVal.ActionSuccess)
            {
                var selectObj = ((ChooseFromListEvent) pVal).SelectedObjects;
                if (selectObj != null)
                {
                    if (selectObj.Rows.Count > 0)
                    {
                        var avalibePr = selectObj.GetValue("U_AvailablePeriod", 0);
                        _availPer.Value = avalibePr.ToString();
                        MyForm.Items.Item("4").Click(BoCellClickType.ct_Regular);
                    }
                }
            }
            //选择所有者
            if (!pVal.BeforeAction && pVal.ItemUID == "222" && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST &&
                pVal.ActionSuccess)
            {
                var selectObj = ((ChooseFromListEvent) pVal).SelectedObjects;
                if (selectObj != null)
                {
                    if (selectObj.Rows.Count > 0)
                    {
                        var position = selectObj.GetValue("position", 0).ToString();
                        _positionBox.Select(position, BoSearchKey.psk_ByValue);
                    }
                }
            }
            //销售员
            if (!pVal.BeforeAction && pVal.ItemUID == "20" && pVal.EventType == BoEventTypes.et_COMBO_SELECT &&
                pVal.ActionSuccess)
            {
                if (pVal.ItemChanged)
                {
                    var slpCode = ((ComboBox) MyForm.Items.Item("20").Specific).Value.Trim();
                    var sql =
                        new StringBuilder(
                            "SELECT T1.position,T1.empID,T1.lastName FROM dbo.OSLP T0 INNER JOIN dbo.OHEM T1 ON T1.salesPrson = T0.SlpCode WHERE SlpCode=")
                            .Append(slpCode);
                    Global.ORecordSet.DoQuery(sql.ToString());
                    if (Global.ORecordSet.EoF == false)
                    {
                        try
                        {
                            MyForm.Freeze(true);
                            var empID = Global.ORecordSet.Fields.Item(1).Value.ToString();
                            var lastName = Global.ORecordSet.Fields.Item(2).Value.ToString();
                            var conditions = _owerChooseFromList.GetConditions();
                            conditions.LoadFromXML(_conditionsXml);
                            if (conditions.Count > 0)
                            {
                                conditions.Item(conditions.Count - 1).Relationship = BoConditionRelationship.cr_AND;
                            }
                            var condition = conditions.Add();
                            condition.Alias = "empID";
                            condition.Operation = BoConditionOperation.co_EQUAL;
                            condition.CondVal = empID;
                            _owerChooseFromList.SetConditions(conditions);
                            ((EditText) MyForm.Items.Item("222").Specific).Value = lastName;
                            Global.Application.SendKeys("{TAB}");
                            conditions.LoadFromXML(_conditionsXml);
                            _owerChooseFromList.SetConditions(conditions);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            MyForm.Freeze(false);
                        }
                    }
                }
            }
            if (pVal.BeforeAction && pVal.ItemUID == "1" &&
                (MyForm.Mode == BoFormMode.fm_ADD_MODE || MyForm.Mode == BoFormMode.fm_UPDATE_MODE))
            {
                var doctotal = MyForm.Items.Item("29");

                if (doctotal.Enabled)
                {
                    var udiscountTotal = (decimal) 0.0;
                    var sum = (decimal) 0.0;
                    for (var i = 1; i <= _ioMtx.VisualRowCount; i++)
                    {
                        
                        var value1 = (_ioMtx.Columns.Item("U_MktDisc").Cells.Item(i).Specific as EditText).Value.Trim();
                        var value2 = (_ioMtx.Columns.Item("U_BadStkDisc").Cells.Item(i).Specific as EditText).Value.Trim();
                        var value3= (_ioMtx.Columns.Item("U_ESDisc").Cells.Item(i).Specific as EditText).Value.Trim();
                        if (!string.IsNullOrEmpty(value1))
                        {
                            udiscountTotal = udiscountTotal + decimal.Parse(value1)+decimal.Parse(value2)+decimal.Parse(value3);
                        }
                        else if (!string.IsNullOrEmpty(value2))
                        {
                             udiscountTotal = udiscountTotal + decimal.Parse(value1)+decimal.Parse(value2)+decimal.Parse(value3);
                        }
                        else if (!string.IsNullOrEmpty(value3))
                        {
                            udiscountTotal = udiscountTotal + decimal.Parse(value1) + decimal.Parse(value2) + decimal.Parse(value3);
                        }

                        var gprice = (_ioMtx.Columns.Item("11").Cells.Item(i).Specific as EditText).Value.Trim();
                        var quantity = (_ioMtx.Columns.Item("20").Cells.Item(i).Specific as EditText).Value.Trim();
                        if (!string.IsNullOrEmpty(gprice) && !string.IsNullOrEmpty(quantity))
                        {
                            gprice = Regex.Replace(gprice, @"[^\d||^\\.]*", "");
                            quantity = Regex.Replace(quantity, @"[^\d||^\\.]*", "");
                            sum = sum + decimal.Parse(quantity) * decimal.Parse(gprice);
                        }
                    }
                    udiscountTotal = udiscountTotal + tmpDiscValue;
                    udiscountTotal = sum - udiscountTotal;
                    ((EditText) MyForm.Items.Item("29").Specific).Value = udiscountTotal.ToString();
                }
            }
        }

        //DI 创建收款申请单(次功能处理销售订单在未批准的状态下创建付款申请单)
        private void CreateDownPayment()
        {
            var docStatus = _ioOrdr.GetValue("DocStatus", 0).Trim();
            if (docStatus.Equals("O"))
            {
                var oOrders = Global.OCompany.GetBusinessObject(BoObjectTypes.oOrders) as Documents;
                if (oOrders != null)
                {
                    var updated = false;
                    oOrders.GetByKey(int.Parse(_ioOrdr.GetValue("DocEntry", 0).Trim()));
                    if (oOrders.Confirmed == BoYesNoEnum.tNO)
                    {
                        oOrders.Confirmed = BoYesNoEnum.tYES;
                        updated = true;
                        oOrders.Update();
                    }

                    var oDownPaymentAr = Global.OCompany.GetBusinessObject(BoObjectTypes.oDownPayments) as Documents;
                    if (oDownPaymentAr != null)
                    {
                        oDownPaymentAr.TaxDate = DateTime.Today;
                        oDownPaymentAr.DocDueDate = oOrders.DocDueDate;
                        oDownPaymentAr.DocDate = DateTime.ParseExact(_ioUdbsDueDate.ValueEx, "yyyyMMdd",
                            CultureInfo.CurrentCulture);
                        oDownPaymentAr.CardCode = oOrders.CardCode;
                        oDownPaymentAr.CardName = oOrders.CardName;
                        oDownPaymentAr.ContactPersonCode = oOrders.ContactPersonCode;
                        oDownPaymentAr.DocCurrency = oOrders.DocCurrency;
                        oDownPaymentAr.Address = oOrders.Address;
                        oDownPaymentAr.Address2 = oOrders.Address2;
                        oDownPaymentAr.DownPaymentType = DownPaymentTypeEnum.dptRequest;
                        oDownPaymentAr.DocTotal = double.Parse(_ioUdbsTotal.ValueEx);
                        oDownPaymentAr.Lines.Delete();
                        for (var i = 0; i < oOrders.Lines.Count; i++)
                        {
                            oOrders.Lines.SetCurrentLine(i);

                            oDownPaymentAr.Lines.Add();

                            oDownPaymentAr.Lines.SetCurrentLine(i);

                            oDownPaymentAr.Lines.BaseType = 17;
                            oDownPaymentAr.Lines.BaseEntry = int.Parse(_ioOrdr.GetValue("DocEntry", 0).Trim());
                            oDownPaymentAr.Lines.BaseLine = oOrders.Lines.LineNum;
                            oDownPaymentAr.Lines.ItemCode = oOrders.Lines.ItemCode;
                            oDownPaymentAr.Lines.ItemDescription = oOrders.Lines.ItemDescription;
                            oDownPaymentAr.Lines.PriceAfterVAT = oOrders.Lines.PriceAfterVAT;
                            oDownPaymentAr.Lines.Quantity = oOrders.Lines.Quantity;
                            oDownPaymentAr.Lines.Currency = oOrders.Lines.Currency;
                            oDownPaymentAr.Lines.DiscountPercent = oOrders.Lines.DiscountPercent;
                        }

                        long retVal = oDownPaymentAr.Add();
                        if (retVal != 0)
                        {
                            var errCode = 0;
                            string errMsg;
                            Global.OCompany.GetLastError(out errCode, out errMsg);
                            Global.Application.SetStatusBarMessage(errMsg + " errorCode:" + errCode,
                                BoMessageTime.bmt_Short);
                        }
                        else
                        {
                            var lsKey = Global.OCompany.GetNewObjectKey();
                            Global.Application.MessageBox("Sucessful Create Down Payment,DocNum:" + lsKey, 1, "OK");
                            Global.Application.SetStatusBarMessage("Creating Down Payment", BoMessageTime.bmt_Short,
                                false);
                        }
                    }
                    if (updated)
                    {
                        oOrders.Confirmed = BoYesNoEnum.tNO;
                        oOrders.Update();
                    }
                }
                var reloadData = Global.Application.Menus.Item("1304");
                if (reloadData.Enabled)
                {
                    reloadData.Activate();
                }
            }
        }

        //---------------------------------------------查询商品价格-------------------------------------------------------------------------------------------
        private void CalculateGoodPrices()
        {
            var itemCodePrices = new SortedList<string, decimal>();

            foreach (DataRow row in _dtTmp.Rows)
            {
                var itemCode = row["ItemCode"].ToString();
                var uoM = row["Uom"].ToString();
                var price = (decimal) 0.0;
                if (!itemCodePrices.ContainsKey(itemCode))
                {
                    var sb = new StringBuilder("EXECUTE SearchPrice ");
                    sb.Append("'").Append(itemCode).Append("'").Append(",");
                    sb.Append("'").Append(_ioOrdr.GetValue("CardCode", 0).Trim()).Append("'").Append(",");
                    sb.Append("'").Append(uoM.Trim()).Append("'").Append(",");
                    sb.Append("'").Append(_ioOrdr.GetValue("DocDate", 0).Trim()).Append("'");
                    Global.ORecordSet.DoQuery(sb.ToString());
                    if (Global.ORecordSet.EoF == false)
                    {
                        Global.ORecordSet.MoveFirst();
                        price = decimal.Parse(Global.ORecordSet.Fields.Item(0).Value.ToString());
                        if (price < 0)
                        {
                            price = (decimal) 0.0;
                        }
                    }
                    itemCodePrices.Add(itemCode, price);
                }
                else if (itemCodePrices.ContainsKey(itemCode))
                {
                    price = itemCodePrices[itemCode];
                }
                row["GPBD"] = price;
            }
        }

        /// <summary>
        ///     计算折扣
        /// </summary>
        private void CalculateDiscount(List<OuomInfo> ouomInfos)
        {
            var tmpSumValue = new SortedList<string, decimal>();
            var tmpSumQty = new SortedList<string, decimal>();


            foreach (DataRow row in _dtTmp.Rows)
            {
                var isFreeGood = row["IsFree"].ToString();
                var itemCode = row["ItemCode"].ToString();
                var bPrice = row["GPBD"].ToString();
                if (isFreeGood.Equals("N"))
                {
                    // 汇总数量和金额
                    var qty = decimal.Parse(row["Quantity"].ToString());
                    var uomCode = row["Uom"].ToString();
                    if (tmpSumQty.ContainsKey(itemCode + "~" + uomCode))
                    {
                        tmpSumQty[itemCode + "~" + uomCode] = tmpSumQty[itemCode + "~" + uomCode] + qty;
                    }
                    else
                    {
                        tmpSumQty.Add(itemCode + "~" + uomCode, qty);
                    }


                    if (tmpSumValue.ContainsKey(itemCode + "~" + uomCode))
                    {
                        tmpSumValue[itemCode + "~" + uomCode] = tmpSumValue[itemCode] + qty*decimal.Parse(bPrice);
                    }
                    else
                    {
                        tmpSumValue.Add(itemCode + "~" + uomCode, qty*decimal.Parse(bPrice));
                    }
                }
            }


            //开始计算折扣

            foreach (var entry in tmpSumQty)
            {
                var itemCode = entry.Key.Split('~')[0];
                var uoM = entry.Key.Split('~')[1];
                var qty = entry.Value;
                if (tmpSumValue.ContainsKey(entry.Key))
                {
                    var value = tmpSumValue[entry.Key];
                    if (value > (decimal) 0.0 || qty > (decimal) 0.0)
                    {
                        var sb = new StringBuilder("EXECUTE SearchDiscount ");
                        sb.Append("'").Append(itemCode).Append("'").Append(",");
                        sb.Append("'").Append(uoM).Append("'").Append(",");
                        sb.Append("'").Append(_ioOrdr.GetValue("CardCode", 0).Trim()).Append("'").Append(",");
                        sb.Append("'").Append(_ioOrdr.GetValue("DocDate", 0).Trim()).Append("'").Append(",");
                        sb.Append("'").Append(qty.ToString()).Append("'").Append(",");
                        sb.Append("'").Append(value.ToString()).Append("'");
                        Global.ORecordSet.DoQuery(sb.ToString());

                        if (Global.ORecordSet.EoF == false)
                        {
                            Global.ORecordSet.MoveFirst();
                            var regularDiscount = Global.ORecordSet.Fields.Item(0).Value.ToString();
                            var extraDiscount = Global.ORecordSet.Fields.Item(1).Value.ToString();
                            var specilaDiscount = Global.ORecordSet.Fields.Item(2).Value.ToString();
                            foreach (DataRow row in _dtTmp.Rows)
                            {
                                var isFreeGoodRow = row["IsFree"].ToString();
                                var itemCodeRow = row["ItemCode"].ToString();
                                if (isFreeGoodRow.Equals("N") && itemCode == itemCodeRow)
                                {
                                    row["RDiscount"] = decimal.Parse(regularDiscount) < (decimal) 0
                                        ? 0
                                        : decimal.Parse(regularDiscount);
                                    row["EDiscount"] = decimal.Parse(extraDiscount) < (decimal) 0
                                        ? 0
                                        : decimal.Parse(extraDiscount);
                                    row["SDiscount"] = decimal.Parse(specilaDiscount) < (decimal) 0
                                        ? 0
                                        : decimal.Parse(specilaDiscount);
                                }
                            }
                        }
                    }
                }
            }

            //特殊的额外折扣计算

            var uomInfoTmplist = new List<OuomInfo>();

            var allZeno = true;


            foreach (DataRow row in _dtTmp.Rows)
            {
                var isFreeGoodRow = row["IsFree"].ToString();
                var itemCodeRow = row["ItemCode"].ToString();
                var bPriceRow = row["GPBD"].ToString();
                if (isFreeGoodRow.Equals("N"))
                {
                    var disc = row["EDiscount"].ToString();
                    if (!string.IsNullOrEmpty(disc))
                    {
                        var ediscount = decimal.Parse(disc);
                        if (ediscount > (decimal) 0.0)
                        {
                            allZeno = false;
                        }
                    }
                    var uom = row["Uom"].ToString();
                    if (!string.IsNullOrEmpty(uom))
                    {
                        var qty = row["Quantity"].ToString();
                        var exists = false;
                        foreach (var ouomInfo in uomInfoTmplist)
                        {
                            if (ouomInfo.ItemCode == itemCodeRow && ouomInfo.UomCode == uom)
                            {
                                ouomInfo.Qty = ouomInfo.Qty + decimal.Parse(qty);
                                ouomInfo.Amount = ouomInfo.Amount + decimal.Parse(qty)*decimal.Parse(bPriceRow);
                                exists = true;
                                break;
                            }
                        }
                        if (!exists)
                        {
                            var ouomInfo = new OuomInfo
                            {
                                ItemCode = itemCodeRow,
                                UomCode = uom,
                                BaseQty = decimal.Parse(qty)
                            };
                            uomInfoTmplist.Add(ouomInfo);
                        }
                    }
                }
            }

            var totalDiscount = (decimal) 0.0;
            if (allZeno && uomInfoTmplist.Count > 0)
            {
                var sb =
                    new StringBuilder(
                        "SELECT distinct t11.U_Disc,t11.U_DiscValue,t11.U_MiniQty,t11.U_MiniValue,t11.U_Number,t11.U_UoM");
                sb.Append(
                    " from [@COR0230] t10  INNER JOIN dbo.[@COR0231] t11 ON t11.DocEntry = t10.DocEntry and t10.U_Active='Y' INNER JOIN dbo.OCRD t12 ON t10.U_Channel=t12.U_ChannelCode")
                    .Append(" AND U_PeriodeFrom<='")
                    .Append(_ioOrdr.GetValue("DocDate", 0).Trim())
                    .Append("' AND U_PeriodeTo>='")
                    .Append(_ioOrdr.GetValue("DocDate", 0).Trim())
                    .Append("'")
                    .Append(" ORDER BY U_Number DESC");
                Global.ORecordSet.DoQuery(sb.ToString());
                while (Global.ORecordSet.EoF == false)
                {
                    var uDis = Global.ORecordSet.Fields.Item(0).Value.ToString();
                    var uDiscValue = Global.ORecordSet.Fields.Item(1).Value.ToString();
                    var uMiniQty = Global.ORecordSet.Fields.Item(2).Value.ToString();
                    var uMiniValue = Global.ORecordSet.Fields.Item(3).Value.ToString();
                    var uNumber = Global.ORecordSet.Fields.Item(4).Value.ToString();
                    var uUom = Global.ORecordSet.Fields.Item(5).Value.ToString();
                    var minQty = decimal.MaxValue;
                    var minValue = decimal.MaxValue;
                    var number = 0;

                    foreach (var ouomInfo in ouomInfos)
                    {
                        if (ouomInfo.UomCode == uUom)
                        {
                            if (minQty > ouomInfo.Qty)
                            {
                                minQty = ouomInfo.Qty;
                            }
                            if (minValue > ouomInfo.Amount)
                            {
                                minValue = ouomInfo.Amount;
                            }
                            number = number + 1;
                        }
                    }
                    if ((minQty >= decimal.Parse(uMiniQty) || minValue >= decimal.Parse(uMiniValue)) &&
                        number >= int.Parse(uNumber))
                    {
                        if (decimal.Parse(uDis) > 0)
                        {
                            foreach (DataRow row in _dtTmp.Rows)
                            {
                                var isFreeGoodRow = row["IsFree"].ToString();
                                var rowUom = row["Uom"].ToString();
                                if (isFreeGoodRow.Equals("N") && rowUom == uUom)
                                {
                                    row["EDiscount"] = uDis;
                                }
                            }
                        }
                        else
                        {
                            totalDiscount = totalDiscount + decimal.Parse(uDiscValue);
                        }
                    }
                    Global.ORecordSet.MoveNext();
                }
            }
            tmpDiscValue = totalDiscount;

            foreach (DataRow row in _dtTmp.Rows)
            {
                var isFreeGoodRow = row["IsFree"].ToString();
                var bPriceRow = row["GPBD"].ToString();
                var regularDiscount = row["RDiscount"].ToString();
                var extraDiscount = row["EDiscount"].ToString();
                var specilaDiscount = row["SDiscount"].ToString();
                if (isFreeGoodRow.Equals("N"))
                {
                    var tprice = decimal.Parse(bPriceRow) - decimal.Parse(bPriceRow)*decimal.Parse(regularDiscount)/100;
                    tprice = tprice - tprice*decimal.Parse(extraDiscount)/100;
                    tprice = tprice - tprice*decimal.Parse(specilaDiscount)/100;
                    var discountPrice = decimal.Parse(bPriceRow) - tprice;
                    var quantity = decimal.Parse(row["Quantity"].ToString());
                    var discountTotal = discountPrice*quantity;
                    row["Price"] = tprice;
                    row["DisTotal"] = discountTotal;
                }
            }

            foreach (DataRow row in _dtTmp.Rows)
            {
                var isFreeGoodRow = row["IsFree"].ToString();

                if (isFreeGoodRow.Equals("N"))
                {
                    var lineNum = int.Parse(row["LineNum"].ToString());
                    ((EditText) _ioMtx.Columns.Item("20").Cells.Item(lineNum).Specific).Value = row["Price"].ToString();
                    ((EditText) _ioMtx.Columns.Item("U_GrBeDisPrice").Cells.Item(lineNum).Specific).Value =
                        row["GPBD"].ToString();
                    ((EditText) _ioMtx.Columns.Item("U_Rdiscount").Cells.Item(lineNum).Specific).Value =
                        row["RDiscount"].ToString();
                    ((EditText) _ioMtx.Columns.Item("U_Ediscount").Cells.Item(lineNum).Specific).Value =
                        row["EDiscount"].ToString();
                    ((EditText) _ioMtx.Columns.Item("U_Sdiscount").Cells.Item(lineNum).Specific).Value =
                        row["SDiscount"].ToString();
                    ((EditText) _ioMtx.Columns.Item("U_DiscountTotal").Cells.Item(lineNum).Specific).Value =
                        row["DisTotal"].ToString();
                }
                else
                {
                    var lineNum = _ioMtx.RowCount;
                    ((EditText) _ioMtx.Columns.Item("1").Cells.Item(lineNum).Specific).Value =
                        row["ItemCode"].ToString();
                    ((EditText) _ioMtx.Columns.Item("11").Cells.Item(lineNum).Specific).Value =
                        row["Quantity"].ToString();
                    if (row["UomEntry"].ToString() != "-1")
                        ((EditText) _ioMtx.Columns.Item("1470002145").Cells.Item(lineNum).Specific).Value =
                            row["Uom"].ToString();
                    ((ComboBox) _ioMtx.Columns.Item("U_IsFreeGood").Cells.Item(lineNum).Specific).Select(0,
                        BoSearchKey.psk_Index);
                    ((EditText) _ioMtx.Columns.Item("14").Cells.Item(lineNum).Specific).Value = "0";
                    totalDiscount = +(decimal) row["Udiscount"];
                }
            }
            if (totalDiscount > (decimal) 0.0)
            {
                var total = ((EditText) MyForm.Items.Item("29").Specific).Value;
                total = Regex.Replace(total, @"[^\d||^\\.]*", "");
                totalDiscount = decimal.Parse(total) - totalDiscount;
                ((EditText) MyForm.Items.Item("29").Specific).Value = totalDiscount.ToString();
            }
        }

        /// <summary>
        ///     计算免费商品
        /// </summary>
        private void CalculateFreeGoods(List<OuomInfo> ouomInfos)
        {
            //统计每类商品的数量按最小单位统计
            var tmpSumQty = new SortedList<string, decimal>();
            foreach (DataRow row in _dtTmp.Rows)
            {
                var itemCode = row["ItemCode"].ToString();

                var qty = decimal.Parse(row["Quantity"].ToString());
                var uomCode = row["Uom"].ToString();
                var ouomInfoTmp =
                    ouomInfos.FirstOrDefault(entry => entry.ItemCode == itemCode && entry.UomCode == uomCode);
                if (ouomInfoTmp != null)
                {
                    if (tmpSumQty.ContainsKey(itemCode + "~" + uomCode))
                    {
                        tmpSumQty[itemCode + "~" + uomCode] = tmpSumQty[itemCode + "~" + uomCode] + qty;
                    }
                    else
                    {
                        tmpSumQty.Add(itemCode + "~" + uomCode, qty);
                    }
                }
            }

            //开始添加免费商品
            if (tmpSumQty.Count > 0)
            {
                var freeInfos = new List<FreeInfo>();
                foreach (var entry in tmpSumQty)
                {
                    var itemCode = entry.Key.Split('~')[0];
                    var uoM = entry.Key.Split('~')[1];
                    var qty = entry.Value;
                    if (qty > (decimal) 0.0)
                    {
                        var sb = new StringBuilder("EXECUTE SearchFreeGoods ");
                        sb.Append("'").Append(itemCode).Append("'").Append(",");
                        sb.Append("'").Append(uoM).Append("'").Append(",");
                        sb.Append("'").Append(_ioOrdr.GetValue("CardCode", 0).Trim()).Append("'").Append(",");
                        sb.Append("'").Append(_ioOrdr.GetValue("DocDate", 0).Trim()).Append("'").Append(",");
                        sb.Append(qty.ToString());
                        Global.ORecordSet.DoQuery(sb.ToString());
                        while (Global.ORecordSet.EoF == false)
                        {
                            itemCode = Global.ORecordSet.Fields.Item(0).Value.ToString();
                            var quantity = decimal.Parse(Global.ORecordSet.Fields.Item(1).Value.ToString());
                            var uom = Global.ORecordSet.Fields.Item(2).Value.ToString();
                            var uomEntry = int.Parse(Global.ORecordSet.Fields.Item(3).Value.ToString());
                            var exists = false;
                            foreach (var freeInfo in freeInfos)
                            {
                                if (freeInfo.ItemCode == itemCode && uomEntry == freeInfo.UomEntry)
                                {
                                    freeInfo.Quantity = freeInfo.Quantity;
                                    exists = true;
                                    break;
                                }
                            }
                            if (!exists)
                            {
                                var freeInfo = new FreeInfo
                                {
                                    ItemCode = itemCode,
                                    Quantity = quantity,
                                    Uom = uom,
                                    UomEntry = uomEntry
                                };
                                freeInfos.Add(freeInfo);
                            }
                            Global.ORecordSet.MoveNext();
                        }
                    }
                }

                foreach (var enty in freeInfos)
                {
                    //设置每类免费商品的价格为0.0

                    var obj = new object[]
                    {-1, enty.ItemCode, enty.Quantity, 0, enty.Uom, 0, 0, 0, 0, "Y", 0, enty.UomEntry.ToString()};
                    _dtTmp.Rows.Add(obj);
                }
            }
        }

        /// <summary>
        ///     窗体大小更改
        /// </summary>
        private void MyFormResize()
        {
            MyForm.Items.Item("Calculate").Top = MyForm.Items.Item("2").Top;
            MyForm.Items.Item("Calculate").Height = MyForm.Items.Item("2").Height;
            MyForm.Items.Item("Calculate").Width = MyForm.Items.Item("2").Width;
            MyForm.Items.Item("Calculate").Left = MyForm.Items.Item("2").Left + MyForm.Items.Item("2").Width + 5;


            MyForm.Items.Item("COdpi").Top = MyForm.Items.Item("Calculate").Top;
            MyForm.Items.Item("COdpi").Height = MyForm.Items.Item("Calculate").Height;
            MyForm.Items.Item("COdpi").Width = MyForm.Items.Item("Calculate").Width + 60;
            MyForm.Items.Item("COdpi").Left = MyForm.Items.Item("Calculate").Left + MyForm.Items.Item("Calculate").Width +
                                              5;

            var uid = "CTotal";


            MyForm.Items.Item(uid).Top = MyForm.Items.Item("29").Top + 16;

            MyForm.Items.Item(uid).Width = MyForm.Items.Item("29").Width;
            MyForm.Items.Item(uid).Height = MyForm.Items.Item("29").Height;
            MyForm.Items.Item(uid).Left = MyForm.Items.Item("29").Left;

            uid = uid + "L";
            MyForm.Items.Item(uid).Top = MyForm.Items.Item("30").Top + 16;

            MyForm.Items.Item(uid).Width = MyForm.Items.Item("30").Width;
            MyForm.Items.Item(uid).Height = MyForm.Items.Item("30").Height;
            MyForm.Items.Item(uid).Left = MyForm.Items.Item("30").Left;


            uid = "PostDate";

            MyForm.Items.Item(uid).Top = MyForm.Items.Item("29").Top + 32;

            MyForm.Items.Item(uid).Width = MyForm.Items.Item("29").Width;
            MyForm.Items.Item(uid).Height = MyForm.Items.Item("29").Height;
            MyForm.Items.Item(uid).Left = MyForm.Items.Item("29").Left;

            uid = uid + "L";
            MyForm.Items.Item(uid).Top = MyForm.Items.Item("30").Top + 32;

            MyForm.Items.Item(uid).Width = MyForm.Items.Item("30").Width;
            MyForm.Items.Item(uid).Height = MyForm.Items.Item("30").Height;
            MyForm.Items.Item(uid).Left = MyForm.Items.Item("30").Left;


            uid = "position";

            MyForm.Items.Item(uid).Top = MyForm.Items.Item("222").Top + 16;

            MyForm.Items.Item(uid).Width = MyForm.Items.Item("222").Width;
            MyForm.Items.Item(uid).Height = MyForm.Items.Item("222").Height;
            MyForm.Items.Item(uid).Left = MyForm.Items.Item("222").Left;

            uid = uid + "L";
            MyForm.Items.Item(uid).Top = MyForm.Items.Item("230").Top + 16;

            MyForm.Items.Item(uid).Width = MyForm.Items.Item("230").Width;
            MyForm.Items.Item(uid).Height = MyForm.Items.Item("230").Height;
            MyForm.Items.Item(uid).Left = MyForm.Items.Item("230").Left;


            uid = "AvailPer";

            MyForm.Items.Item(uid).Top = MyForm.Items.Item("222").Top + 32;

            MyForm.Items.Item(uid).Width = MyForm.Items.Item("222").Width;
            MyForm.Items.Item(uid).Height = MyForm.Items.Item("222").Height;
            MyForm.Items.Item(uid).Left = MyForm.Items.Item("222").Left;

            uid = uid + "L";
            MyForm.Items.Item(uid).Top = MyForm.Items.Item("230").Top + 32;

            MyForm.Items.Item(uid).Width = MyForm.Items.Item("230").Width;
            MyForm.Items.Item(uid).Height = MyForm.Items.Item("230").Height;
            MyForm.Items.Item(uid).Left = MyForm.Items.Item("230").Left;
        }

        public override void FormDataLoad(ref BusinessObjectInfo businessobjectinfo, ref bool bubbleevent)
        {
            if (!businessobjectinfo.BeforeAction && businessobjectinfo.ActionSuccess &&
                MyForm.Mode == BoFormMode.fm_OK_MODE)
            {
                _ioUdbsDueDate.ValueEx = DateTime.Today.ToString("yyyyMMdd");
                _ioUdbsTotal.ValueEx = _ioOrdr.GetValue("DocTotal", 0);
            }
        }

        public override void FormDataAdd(ref BusinessObjectInfo businessobjectinfo, ref bool bubbleevent)
        {
            if (!businessobjectinfo.BeforeAction && businessobjectinfo.ActionSuccess)
            {
                var docEntry = businessobjectinfo.ObjectKey;
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(docEntry);
                var rootNode = xmlDoc.SelectSingleNode("DocumentParams/DocEntry");

                if (rootNode != null)
                {
                    docEntry = rootNode.InnerText;
                    var oOrder = Global.OCompany.GetBusinessObject(BoObjectTypes.oOrders) as Documents;
                    if (oOrder != null)
                    {
                        oOrder.GetByKey(int.Parse(docEntry));

                        var pass = oOrder.UserFields.Fields.Item("U_Pass").Value;
                        var confirmed = oOrder.Confirmed;
                        if (pass.Equals("N") && confirmed == BoYesNoEnum.tYES)
                        {
                            oOrder.Confirmed = BoYesNoEnum.tNO;
                            oOrder.Update();
                        }
                    }
                }
            }
        }

        public override void FormDataUpUpdate(ref BusinessObjectInfo businessobjectinfo, ref bool bubbleevent)
        {
            if (!businessobjectinfo.BeforeAction && businessobjectinfo.ActionSuccess)
            {
                var docEntry = businessobjectinfo.ObjectKey;
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(docEntry);
                var rootNode = xmlDoc.SelectSingleNode("DocumentParams/DocEntry");
                if (rootNode != null)
                {
                    docEntry = rootNode.InnerText;
                    var oOrder = Global.OCompany.GetBusinessObject(BoObjectTypes.oOrders) as Documents;
                    if (oOrder != null)
                    {
                        oOrder.GetByKey(int.Parse(docEntry));
                        var pass = oOrder.UserFields.Fields.Item("U_Pass").Value;
                        var confirmed = oOrder.Confirmed;
                        if (pass.Equals("Y") && confirmed == BoYesNoEnum.tNO)
                        {
                            oOrder.Confirmed = BoYesNoEnum.tYES;
                            oOrder.Update();
                        }
                    }
                }
            }
        }

        private class FreeInfo
        {
            public string ItemCode { get; set; }
            public string Uom { get; set; }
            public int UomEntry { get; set; }
            public decimal Quantity { get; set; }
        }

        private class OuomInfo
        {
            public string ItemCode { get; set; }
            public int UomEntry { get; set; }
            public decimal AltQty { get; set; }
            public decimal BaseQty { get; set; }
            public string UomCode { get; set; }
            public decimal Amount { get; set; }
            public decimal Qty { get; set; }
        }
    }
}