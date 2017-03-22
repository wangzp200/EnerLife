using System;
using System.Collections.Generic;
using EnerlifeCN.Code.Util;
using SAPbouiCOM;

namespace EnerlifeCN.Code.FormExt.Custom.COR0040
{
    public class COR0040 : SwBaseForm
    {
        private readonly SortedList<string, string> _validValueList = new SortedList<string, string>();
        private DBDataSource _ioDbCor0040, _ioDbCor0041;
        private Matrix _ioMtx;
        private Item _ioRec;
        private ComboBox _ioSeries;
        private int currentRow;

        public override void FormCreate(string formUId, ref ItemEvent pVal, ref bool bubbleEvent)
        {
            _ioMtx = MyForm.Items.Item("10").Specific as Matrix;
            if (_ioMtx != null)
            {
                _ioMtx.SelectionMode = BoMatrixSelect.ms_Auto;
                _ioRec = MyForm.Items.Item("4");
                _ioSeries = MyForm.Items.Item("1000001").Specific as ComboBox;
                var validValues = _ioSeries.ValidValues;
                CommonUtil.SeriesValidValues(validValues, MyForm);
                _ioDbCor0040 = MyForm.DataSources.DBDataSources.Item("@COR0040");
                _ioDbCor0041 = MyForm.DataSources.DBDataSources.Item("@COR0041");
                Fieldlist.Add("@COR0041", "U_ItemCode");
                _ioDbCor0041.RemoveRecord(0);
                CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0041, true);
                _ioMtx.LoadFromDataSource();
            }
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
                            _ioDbCor0040.SetValue("U_CardCode", 0, result.GetValue("CardCode", 0) as string);
                            _ioDbCor0040.SetValue("U_CardName", 0, result.GetValue("CardName", 0) as string);
                            if (MyForm.Mode == BoFormMode.fm_OK_MODE)
                            {
                                MyForm.Mode = BoFormMode.fm_UPDATE_MODE;
                            }
                        }
                        if (pVal.ColUID == "C1" || pVal.ColUID == "C6")
                        {
                            try
                            {
                                _ioDbCor0041.Offset = oCflEvento.Row - 1;
                                _ioMtx.GetLineData(oCflEvento.Row);
                                if (pVal.ColUID == "C1")
                                {
                                    _ioDbCor0041.SetValue("U_ItemCode", oCflEvento.Row - 1,
                                        result.GetValue("ItemCode", 0) as string);
                                    _ioDbCor0041.SetValue("U_ItemName", oCflEvento.Row - 1,
                                        result.GetValue("ItemName", 0) as string);
                                    _ioDbCor0041.SetValue("U_FromQty", oCflEvento.Row - 1, "1");
                                    _ioDbCor0041.SetValue("U_ToQty", oCflEvento.Row - 1, "1");
                                }

                                _ioMtx.SetLineData(oCflEvento.Row);

                                if (oCflEvento.Row == _ioMtx.VisualRowCount)
                                {
                                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0041, false);
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
                        CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0041, false);
                    }
                }
                else
                {
                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0041, false);
                }
            }


            //---------------------------------------点击时重新赋值ValidValues----------------------------------------------
            if (pVal.BeforeAction && pVal.EventType == BoEventTypes.et_CLICK && pVal.ColUID == "C5" && licRow > 0 &&
                currentRow != licRow)
            {
                currentRow = licRow;
                _validValueList.Clear();
                _ioMtx.SelectRow(licRow, true, false);
                _ioDbCor0041.Offset = licRow - 1;
                _ioMtx.GetLineData(licRow);
                var lsItemCode = _ioDbCor0041.GetValue("U_ItemCode", licRow - 1);

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

            //----------------------------------------------对UomEntry赋值----------------------------------------------------------------------
            if (!pVal.BeforeAction && pVal.EventType == BoEventTypes.et_COMBO_SELECT && pVal.ItemChanged &&
                pVal.ColUID == "C5" && licRow > 0)
            {
                _ioMtx.SetLineData(licRow);
                _ioDbCor0041.Offset = licRow - 1;
                var popUp = pVal.PopUpIndicator;
                var validValues = _ioMtx.Columns.Item(pVal.ColUID).ValidValues;
                var value = validValues.Item(popUp).Value;
                switch (pVal.ColUID)
                {
                    case "C5":
                        if (!string.IsNullOrEmpty(value))
                        {
                            foreach (var entry in _validValueList)
                            {
                                var svalue = entry.Value;
                                var skey = entry.Key;
                                if (skey == value)
                                {
                                    _ioDbCor0041.SetValue("U_UomEntry", licRow - 1, svalue);
                                    _ioDbCor0041.SetValue("U_UoM", licRow - 1, value);
                                    break;
                                }
                            }
                        }
                        break;
                }

                _ioMtx.SetLineData(licRow);
            }
            //---------------------------------------------------------------------
        }

        public override void MenuEventHandler(ref MenuEvent pVal, ref bool bubbleEvent)
        {
            if (!pVal.BeforeAction)
            {
                var oMenuEvent = pVal;
                if (oMenuEvent.MenuUID == "1282")
                {
                    _ioDbCor0041.RemoveRecord(0);
                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0041, true);
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