using System;
using System.Collections.Generic;
using EnerlifeCN.Code.Util;
using SAPbouiCOM;

namespace EnerlifeCN.Code.FormExt.Custom.COR0070
{
    public class COR0070 : SwBaseForm
    {
        private readonly SortedList<string, string> _valueList = new SortedList<string, string>();
        private Button _ioBtnOk, _ioBtnCancle;
        private EditText _ioCardCode;
        private DBDataSource _ioDbCor0070, _ioDbCor0071;
        private Matrix _ioMtx;
        private Item _ioRec;
        private ComboBox _ioSeries;
        private int currentRow;

        public override void FormCreate(string formUId, ref ItemEvent pVal, ref bool bubbleEvent)
        {
            _ioBtnOk = MyForm.Items.Item("1").Specific as Button;
            _ioBtnCancle = MyForm.Items.Item("2").Specific as Button;
            _ioMtx = MyForm.Items.Item("10").Specific as Matrix;
            _ioMtx.SelectionMode = BoMatrixSelect.ms_Auto;
            _ioCardCode = MyForm.Items.Item("7").Specific as EditText;
            _ioRec = MyForm.Items.Item("4");
            _ioSeries = MyForm.Items.Item("1000001").Specific as ComboBox;


            var validValues = _ioSeries.ValidValues;
            CommonUtil.SeriesValidValues(validValues, MyForm);

            _ioDbCor0070 = MyForm.DataSources.DBDataSources.Item("@COR0070");
            _ioDbCor0071 = MyForm.DataSources.DBDataSources.Item("@COR0071");
            Fieldlist.Add("@COR0071", "U_ItemCode");
            _ioDbCor0071.RemoveRecord(0);
            CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0071, true);
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
                        if (pVal.ItemUID == "7" && _ioDbCor0070.Size > 0)
                        {
                            _ioDbCor0070.SetValue("U_CardCode", 0, result.GetValue("CardCode", 0) as string);
                            _ioDbCor0070.SetValue("U_CardName", 0, result.GetValue("CardName", 0) as string);
                            if (MyForm.Mode == BoFormMode.fm_OK_MODE)
                            {
                                MyForm.Mode = BoFormMode.fm_UPDATE_MODE;
                            }
                        }
                        if (pVal.ColUID == "C1" && _ioDbCor0071.Size > 0)
                        {
                            try
                            {
                                _ioDbCor0071.Offset = oCflEvento.Row - 1;
                                _ioMtx.GetLineData(oCflEvento.Row);
                                _ioDbCor0071.SetValue("U_ItemCode", oCflEvento.Row - 1,
                                    result.GetValue("ItemCode", 0) as string);
                                _ioDbCor0071.SetValue("U_ItemName", oCflEvento.Row - 1,
                                    result.GetValue("ItemName", 0) as string);

                                _ioMtx.SetLineData(oCflEvento.Row);

                                if (oCflEvento.Row == _ioMtx.VisualRowCount)
                                {
                                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0071, false);
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
                        CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0071, false);
                    }
                }
                else
                {
                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0071, false);
                }
            }


            //-------------------------------------------------------------------------------------
            if (pVal.BeforeAction && pVal.EventType == BoEventTypes.et_CLICK && pVal.ColUID == "C5" && licRow > 0 &&
                currentRow != licRow)
            {
                currentRow = licRow;

                _valueList.Clear();

                _ioMtx.SelectRow(licRow, true, false);

                _ioDbCor0071.Offset = licRow - 1;
                _ioMtx.SetLineData(licRow);

                var lsItemCode = string.Empty;
                if (pVal.ColUID == "C5")
                {
                    lsItemCode = _ioDbCor0071.GetValue("U_ItemCode", licRow - 1);
                }
                else if (pVal.ColUID == "C9")
                {
                    lsItemCode = _ioDbCor0071.GetValue("U_FreeItem", licRow - 1);
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
                        _valueList.Add(value, key);
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

            //--------------------------------------------------------------------------------------------------------------------
            if (!pVal.BeforeAction && pVal.EventType == BoEventTypes.et_COMBO_SELECT && pVal.ItemChanged &&
                pVal.ColUID == "C5" && licRow > 0)
            {
                _ioDbCor0071.Offset = licRow - 1;
                _ioMtx.GetLineData(licRow);
                var popUp = pVal.PopUpIndicator;
                var validValues = _ioMtx.Columns.Item(pVal.ColUID).ValidValues;
                var value = validValues.Item(popUp).Value;
                switch (pVal.ColUID)
                {
                    case "C5":
                        if (!string.IsNullOrEmpty(value))
                        {
                            foreach (var entry in _valueList)
                            {
                                var svalue = entry.Value;
                                var skey = entry.Key;
                                if (skey == value)
                                {
                                    _ioDbCor0071.SetValue("U_UomEntry", licRow - 1, svalue);
                                    _ioDbCor0071.SetValue("U_UoM", licRow - 1, value);
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
                    _ioDbCor0071.RemoveRecord(0);
                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0071, true);
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