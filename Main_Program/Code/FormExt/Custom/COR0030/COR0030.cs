using System;
using System.Collections.Generic;
using EnerlifeCN.Code.Util;
using SAPbouiCOM;

namespace EnerlifeCN.Code.FormExt.Custom.COR0030
{
    public class COR0030 : SwBaseForm
    {
        private readonly SortedList<string, string> _validValueList = new SortedList<string, string>();
        private Button _ioBtnOk, _ioBtnCancle;
        private DBDataSource _ioDbCor0030, _ioDbCor0031;
        private DataTable _ioDocT;
        private Matrix _ioMtx;
        private Item _ioRec;
        private ComboBox _ioSeries;
        private ComboBox _ioZone;
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
            _ioZone = MyForm.Items.Item("7").Specific as ComboBox;
            _ioRec = MyForm.Items.Item("4");
            _ioSeries = MyForm.Items.Item("1000001").Specific as ComboBox;


            if (_ioSeries != null)
            {
                var validValues = _ioSeries.ValidValues;
                CommonUtil.SeriesValidValues(validValues, MyForm);

                if (_ioZone != null) validValues = _ioZone.ValidValues;

                var sql = "SELECT Code,Name FROM dbo.[@COR0130]";
                Global.ORecordSet.DoQuery(sql);
                while (Global.ORecordSet.EoF == false)
                {
                    validValues.Add(Global.ORecordSet.Fields.Item(0).Value as string,
                        Global.ORecordSet.Fields.Item(1).Value as string);
                    Global.ORecordSet.MoveNext();
                }
            }


            _ioDbCor0030 = MyForm.DataSources.DBDataSources.Item("@COR0030");
            _ioDbCor0031 = MyForm.DataSources.DBDataSources.Item("@COR0031");
            Fieldlist.Add("@COR0031", "U_ItemCode");
            _ioDbCor0031.RemoveRecord(0);
            CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0031, true);
            _ioDocT = MyForm.DataSources.DataTables.Add("ioDocT");
            MyFormResize();
        }

        public override void ItemEventHandler(string formUId, ref ItemEvent pVal, ref bool bubbleEvent)
        {
            var licRow = pVal.Row;
            //-------------------------------------------------------------------------------------------

            if (!pVal.BeforeAction && pVal.EventType == BoEventTypes.et_FORM_RESIZE)
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
                            _ioDbCor0030.SetValue("U_CardCode", 0, result.GetValue("CardCode", 0) as string);
                            _ioDbCor0030.SetValue("U_CardName", 0, result.GetValue("CardName", 0) as string);
                            if (MyForm.Mode == BoFormMode.fm_OK_MODE)
                            {
                                MyForm.Mode = BoFormMode.fm_UPDATE_MODE;
                            }
                        }
                        if (pVal.ColUID == "C1" || pVal.ColUID == "C6")
                        {
                            try
                            {
                                _ioDbCor0031.Offset = oCflEvento.Row - 1;
                                _ioMtx.GetLineData(oCflEvento.Row);
                                switch (pVal.ColUID)
                                {
                                    case "C1":
                                        _ioDbCor0031.SetValue("U_ItemCode", oCflEvento.Row - 1,
                                            result.GetValue("ItemCode", 0) as string);
                                        _ioDbCor0031.SetValue("U_ItemName", oCflEvento.Row - 1,
                                            result.GetValue("ItemName", 0) as string);
                                        _ioDbCor0031.SetValue("U_MinQty", oCflEvento.Row - 1, "1");
                                        _ioDbCor0031.SetValue("U_BuyQty", oCflEvento.Row - 1, "1");
                                        break;
                                    case "C6":
                                        _ioDbCor0031.SetValue("U_FreeItem", oCflEvento.Row - 1,
                                            result.GetValue("ItemCode", 0) as string);
                                        _ioDbCor0031.SetValue("U_FItemName", oCflEvento.Row - 1,
                                            result.GetValue("ItemName", 0) as string);
                                        _ioDbCor0031.SetValue("U_Qty", oCflEvento.Row - 1, "1");
                                        break;
                                }

                                _ioMtx.SetLineData(oCflEvento.Row);

                                if (oCflEvento.Row == _ioMtx.VisualRowCount)
                                {
                                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0031, false);
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
                        CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0031, false);
                    }
                }
                else
                {
                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0031, false);
                }
            }


            //-------------------------------------------------点击时重置ValidValues-------------------------------------------------------------------------------------------------------------------------
            if (pVal.BeforeAction && pVal.EventType == BoEventTypes.et_CLICK &&
                (pVal.ColUID == "C5" || pVal.ColUID == "C9") && licRow > 0 &&
                (currentRow != licRow || currentColumn != pVal.ColUID))
            {
                currentRow = licRow;
                currentColumn = pVal.ColUID;
                _validValueList.Clear(); //清空数组
                _ioMtx.SelectRow(licRow, true, false);
                _ioDbCor0031.Offset = licRow - 1;
                _ioMtx.GetLineData(licRow);
                var lsItemCode = string.Empty;
                switch (pVal.ColUID)
                {
                    case "C5":
                        lsItemCode = _ioDbCor0031.GetValue("U_ItemCode", licRow - 1);
                        break;
                    case "C9":
                        lsItemCode = _ioDbCor0031.GetValue("U_FreeItem", licRow - 1);
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
                (pVal.ColUID == "C5" || pVal.ColUID == "C9") && licRow > 0)
            {
                _ioDbCor0031.Offset = licRow - 1;
                _ioMtx.GetLineData(licRow);
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
                                    _ioDbCor0031.SetValue("U_UomEntry", licRow - 1, svalue);
                                    _ioDbCor0031.SetValue("U_UoM", licRow - 1, value);
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
                                    _ioDbCor0031.SetValue("U_FUomEntry", licRow - 1, svalue);
                                    _ioDbCor0031.SetValue("U_FUoM", licRow - 1, value);
                                    break;
                                }
                            }
                        }
                    }
                }
                _ioMtx.SetLineData(licRow);
            }
            //---------------------------------------------------------------------
            if (!pVal.BeforeAction && pVal.ItemUID == "7" && pVal.EventType == BoEventTypes.et_COMBO_SELECT &&
                pVal.ItemChanged)
            {
                var desc = _ioZone.ValidValues.Item(_ioZone.Value.Trim()).Description;
                _ioDbCor0030.SetValue("U_ZoneName", 0, desc);
            }
        }

        public override void MenuEventHandler(ref MenuEvent pVal, ref bool bubbleEvent)
        {
            if (!pVal.BeforeAction)
            {
                var oMenuEvent = pVal;
                if (oMenuEvent.MenuUID == "1282")
                {
                    _ioDbCor0031.RemoveRecord(0);
                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0031, true);
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