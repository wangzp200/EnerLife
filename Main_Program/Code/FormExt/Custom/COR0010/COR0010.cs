using System;
using System.Collections.Generic;
using EnerlifeCN.Code.Util;
using SAPbouiCOM;

namespace EnerlifeCN.Code.FormExt.Custom.COR0010
{
    public class COR0010 : SwBaseForm
    {
        private readonly SortedList<string, string> _validValueList = new SortedList<string, string>();
        private Button _ioBtnOk, _ioBtnCancle;
        private EditText _ioCardCode;
        private DBDataSource _ioDbCor0010, _ioDbCor0011;
        private DataTable _ioDocT;
        private Matrix _ioMtx;
        private Item _ioRec;
        private ComboBox _ioSeries;
        private string currentColumn;
        private int currentRow;

        public override void FormCreate(string formUId, ref ItemEvent pVal, ref bool bubbleEvent)
        {
            _ioBtnOk = MyForm.Items.Item("1").Specific as Button;
            _ioBtnCancle = MyForm.Items.Item("2").Specific as Button;
            _ioMtx = MyForm.Items.Item("10").Specific as Matrix;
            if (_ioMtx != null)
            {
                _ioMtx.SelectionMode = BoMatrixSelect.ms_Auto;
                _ioMtx.LoadFromDataSource();
            }
            _ioCardCode = MyForm.Items.Item("7").Specific as EditText;
            _ioRec = MyForm.Items.Item("4");
            _ioSeries = MyForm.Items.Item("1000001").Specific as ComboBox;


            if (_ioSeries != null)
            {
                var validValues = _ioSeries.ValidValues;
                CommonUtil.SeriesValidValues(validValues, MyForm);
            }

            _ioDbCor0010 = MyForm.DataSources.DBDataSources.Item("@COR0010");
            _ioDbCor0011 = MyForm.DataSources.DBDataSources.Item("@COR0011");
            Fieldlist.Add("@COR0011", "U_ItemCode");
            _ioDbCor0011.RemoveRecord(0);
            CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0011, true);
            _ioDocT = MyForm.DataSources.DataTables.Add("ioDocT");
            MyFormResize();
        }

        public override void ItemEventHandler(string formUId, ref ItemEvent pVal, ref bool bubbleEvent)
        {
            var rowNum = pVal.Row;


            //-------------------------------------------------------------------------------------------

            if (!pVal.BeforeAction && pVal.EventType == BoEventTypes.et_FORM_RESIZE && Active)
            {
                MyFormResize();
            }

            //-------------------------------------------------------------------------------------------
            if (!pVal.BeforeAction && pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST &&
                MyForm.Mode != BoFormMode.fm_FIND_MODE)
            {
                currentRow = -1;
                var oCflEvento = pVal as ChooseFromListEvent;
                if (oCflEvento != null)
                {
                    var result = oCflEvento.SelectedObjects;
                    if (result == null) return;

                    if (result.Rows.Count > 0)
                    {
                        if (pVal.ItemUID == "7")
                        {
                            _ioDbCor0010.SetValue("U_CardCode", 0, result.GetValue("CardCode", 0) as string);
                            _ioDbCor0010.SetValue("U_CardName", 0, result.GetValue("CardName", 0) as string);
                            if (MyForm.Mode == BoFormMode.fm_OK_MODE)
                            {
                                MyForm.Mode = BoFormMode.fm_UPDATE_MODE;
                            }
                        }
                        if (pVal.ColUID == "C1" || pVal.ColUID == "C6")
                        {
                            try
                            {
                                _ioDbCor0011.Offset = oCflEvento.Row - 1;
                                _ioMtx.GetLineData(oCflEvento.Row);
                                if (pVal.ColUID == "C1")
                                {
                                    _ioDbCor0011.SetValue("U_ItemCode", oCflEvento.Row - 1,
                                        result.GetValue("ItemCode", 0) as string);
                                    _ioDbCor0011.SetValue("U_ItemName", oCflEvento.Row - 1,
                                        result.GetValue("ItemName", 0) as string);
                                    _ioDbCor0011.SetValue("U_MinQty", oCflEvento.Row - 1, "1");
                                    _ioDbCor0011.SetValue("U_BuyQty", oCflEvento.Row - 1, "1");
                                }
                                else if (pVal.ColUID == "C6")
                                {
                                    _ioDbCor0011.SetValue("U_FreeItem", oCflEvento.Row - 1,
                                        result.GetValue("ItemCode", 0) as string);
                                    _ioDbCor0011.SetValue("U_FItemName", oCflEvento.Row - 1,
                                        result.GetValue("ItemName", 0) as string);
                                    _ioDbCor0011.SetValue("U_Qty", oCflEvento.Row - 1, "1");
                                }

                                _ioMtx.SetLineData(oCflEvento.Row);

                                if (oCflEvento.Row == _ioMtx.VisualRowCount)
                                {
                                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0011, false);
                                }

                                if (MyForm.Mode == BoFormMode.fm_OK_MODE)
                                {
                                    MyForm.Mode = BoFormMode.fm_UPDATE_MODE;
                                }
                            }
                            catch (Exception e)
                            {
                                Global.Application.SetStatusBarMessage(e.ToString(), BoMessageTime.bmt_Short);
                            }
                            finally
                            {
                            }
                        }
                    }
                }
            }


            //-------------------------------------------------------------------------------------------
            if (!pVal.BeforeAction && pVal.EventType == BoEventTypes.et_MATRIX_LOAD && pVal.ActionSuccess &&
                pVal.ItemUID == "10")
            {
                if (_ioMtx.VisualRowCount > 0)
                {
                    var value = ((EditText) _ioMtx.Columns.Item("C1").Cells.Item(_ioMtx.VisualRowCount).Specific).Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0011, false);
                    }
                }
                else
                {
                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0011, false);
                }
            }


            //-------------------------------------------------点击时重置ValidValues-------------------------------------------------------------------------------------------------------------------------
            if (pVal.BeforeAction && pVal.EventType == BoEventTypes.et_CLICK &&
                (pVal.ColUID == "C5" || pVal.ColUID == "C9") && rowNum > 0 &&
                (currentRow != rowNum || currentColumn != pVal.ColUID))
            {
                currentRow = rowNum;
                currentColumn = pVal.ColUID;
                _validValueList.Clear(); //清空数组
                _ioMtx.SelectRow(rowNum, true, false);
                _ioDbCor0011.Offset = rowNum - 1;
                _ioMtx.GetLineData(rowNum);
                var lsItemCode = string.Empty;
                switch (pVal.ColUID)
                {
                    case "C5":
                        lsItemCode = _ioDbCor0011.GetValue("U_ItemCode", rowNum - 1);
                        break;
                    case "C9":
                        lsItemCode = _ioDbCor0011.GetValue("U_FreeItem", rowNum - 1);
                        break;
                }
                var sql =
                    @"SELECT CAST(t11.UomEntry AS NVARCHAR(10)),t11.UomCode FROM dbo.UGP1 t10 INNER JOIN dbo.OUOM t11 ON t10.UomEntry=t11.UomEntry INNER JOIN dbo.OITM t12 ON t10.UgpEntry=t12.UgpEntry WHERE t12.ItemCode='" +
                    lsItemCode.Trim() + "' ORDER BY t10.LineNum ASC";
                Global.ORecordSet.DoQuery(sql);

                var validValues = _ioMtx.Columns.Item(pVal.ColUID).ValidValues;

                try
                {
                    MyForm.Freeze(true);
                    while (validValues.Count > 0)
                    {
                        validValues.Remove(0, BoSearchKey.psk_Index);
                    }

                    while (Global.ORecordSet.EoF == false)
                    {
                        var key = Global.ORecordSet.Fields.Item(0).Value as string;
                        var value = Global.ORecordSet.Fields.Item(1).Value as string;
                        _validValueList.Add(value, key);
                        validValues.Add(value, value);
                        Global.ORecordSet.MoveNext();
                    }
                }
                catch (Exception e)
                {
                }
                finally
                {
                    MyForm.Freeze(false);
                }
            }

            //-----------------------------------------------------------------对UomEntry赋值---------------------------------------------------
            if (!pVal.BeforeAction && pVal.EventType == BoEventTypes.et_COMBO_SELECT && pVal.ItemChanged &&
                (pVal.ColUID == "C5" || pVal.ColUID == "C9") && rowNum > 0)
            {
                _ioDbCor0011.Offset = rowNum - 1;
                _ioMtx.GetLineData(rowNum);
                var popUp = pVal.PopUpIndicator;
                var validValues = _ioMtx.Columns.Item(pVal.ColUID).ValidValues;
                var value = validValues.Item(popUp).Value;

                if (!string.IsNullOrEmpty(value))
                {
                    if (pVal.ColUID == "C5")
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            foreach (var entry in _validValueList)
                            {
                                var svalue = entry.Value;
                                var skey = entry.Key;
                                if (skey == value)
                                {
                                    _ioDbCor0011.SetValue("U_UomEntry", rowNum - 1, svalue);
                                    _ioDbCor0011.SetValue("U_UoM", rowNum - 1, value);
                                    break;
                                }
                            }
                        }
                    }
                    else if (pVal.ColUID == "C9")
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            foreach (var entry in _validValueList)
                            {
                                var svalue = entry.Value;
                                var skey = entry.Key;
                                if (skey == value)
                                {
                                    _ioDbCor0011.SetValue("U_FUomEntry", rowNum - 1, svalue);
                                    _ioDbCor0011.SetValue("U_FUoM", rowNum - 1, value);
                                    break;
                                }
                            }
                        }
                    }
                }
                _ioMtx.SetLineData(rowNum);
            }
        }

        public override void MenuEventHandler(ref MenuEvent pVal, ref bool bubbleEvent)
        {
            if (!pVal.BeforeAction)
            {
                var oMenuEvent = pVal;
                if (oMenuEvent.MenuUID == "1282")
                {
                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0011, true);
                }
            }
        }

        private void MyFormResize()
        {
            _ioRec.Width = MyForm.Items.Item("10").Width + 8;
            _ioRec.Height = MyForm.Items.Item("10").Height + 8;
        }
    }
}