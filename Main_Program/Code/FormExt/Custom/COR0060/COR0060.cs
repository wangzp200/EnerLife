using System;
using System.Collections.Generic;
using EnerlifeCN.Code.Util;
using SAPbouiCOM;

namespace EnerlifeCN.Code.FormExt.Custom.COR0060
{
    public class COR0060 : SwBaseForm
    {
        private readonly SortedList<string, string> _validValueList = new SortedList<string, string>();
        private Button _ioBtnOk, _ioBtnCancle;
        private ComboBox _ioChannel;
        private DBDataSource _ioDbCor0060, _ioDbCor0061;
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
            _ioChannel = MyForm.Items.Item("7").Specific as ComboBox;
            _ioRec = MyForm.Items.Item("4");
            _ioSeries = MyForm.Items.Item("1000001").Specific as ComboBox;


            var validValues = _ioSeries.ValidValues;
            CommonUtil.SeriesValidValues(validValues, MyForm);
            _ioChannel = MyForm.Items.Item("7").Specific as ComboBox;
            validValues = _ioChannel.ValidValues;


            var sql = "SELECT Code,Name FROM dbo.[@COR0110]";
            Global.ORecordSet.DoQuery(sql);
            while (Global.ORecordSet.EoF == false)
            {
                validValues.Add(Global.ORecordSet.Fields.Item(0).Value as string,
                    Global.ORecordSet.Fields.Item(1).Value as string);
                Global.ORecordSet.MoveNext();
            }


            validValues = _ioMtx.Columns.Item("C15").ValidValues;

            sql = "SELECT Code,Name FROM dbo.[@COR0210]";
            Global.ORecordSet.DoQuery(sql);
            while (Global.ORecordSet.EoF == false)
            {
                validValues.Add(Global.ORecordSet.Fields.Item(0).Value as string,
                    Global.ORecordSet.Fields.Item(1).Value as string);
                Global.ORecordSet.MoveNext();
            }

            _ioDbCor0060 = MyForm.DataSources.DBDataSources.Item("@COR0060");
            _ioDbCor0061 = MyForm.DataSources.DBDataSources.Item("@COR0061");
            Fieldlist.Add("@COR0061", "U_Division");
            _ioDbCor0061.RemoveRecord(0);
            CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0061, true);

            sql = @"SELECT CAST(UomEntry AS NVARCHAR(10)),UomCode FROM dbo.OUOM";
            Global.ORecordSet.DoQuery(sql);

            validValues = _ioMtx.Columns.Item("C5").ValidValues;

            while (Global.ORecordSet.EoF == false)
            {
                var key = Global.ORecordSet.Fields.Item(0).Value as string;
                var value = Global.ORecordSet.Fields.Item(1).Value as string;
                _validValueList.Add(value, key);
                validValues.Add(value, value);
                Global.ORecordSet.MoveNext();
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
            if (!pVal.BeforeAction && pVal.EventType == BoEventTypes.et_MATRIX_LOAD && pVal.ActionSuccess &&
                pVal.ItemUID == "10")
            {
                if (_ioMtx.VisualRowCount > 0)
                {
                    var value = ((ComboBox) _ioMtx.Columns.Item("C15").Cells.Item(_ioMtx.VisualRowCount).Specific).Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0061, false);
                    }
                }
                else
                {
                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0061, false);
                }
            }

            //----------------------------------------------对UomEntry赋值----------------------------------------------------------------------
            if (!pVal.BeforeAction && pVal.EventType == BoEventTypes.et_COMBO_SELECT && pVal.ItemChanged &&
                pVal.ColUID == "C5" && licRow > 0)
            {
                try
                {
                    MyForm.Freeze(true);
                    _ioMtx.SetLineData(licRow);
                    _ioDbCor0061.Offset = licRow - 1;
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
                                        _ioDbCor0061.SetValue("U_UomEntry", licRow - 1, svalue);
                                        _ioDbCor0061.SetValue("U_UoM", licRow - 1, value);
                                        break;
                                    }
                                }
                            }
                            break;
                    }

                    _ioMtx.SetLineData(licRow);
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

            //---------------------------------------------------------------------
            if (!pVal.BeforeAction && pVal.ItemUID == "7" && pVal.EventType == BoEventTypes.et_COMBO_SELECT &&
                pVal.ItemChanged)
            {
                var desc = _ioChannel.ValidValues.Item(_ioChannel.Value.Trim()).Description;
                _ioDbCor0060.SetValue("U_ChanName", 0, desc);
            }

            //---------------------------------------------------------------------
            if (!pVal.BeforeAction && pVal.ColUID == "C15" && pVal.EventType == BoEventTypes.et_COMBO_SELECT &&
                pVal.ItemChanged && pVal.ItemUID == "10")
            {
                var value = ((ComboBox) _ioMtx.Columns.Item("C15").Cells.Item(pVal.Row).Specific).Value;
                var desc = _ioMtx.Columns.Item("C15").ValidValues.Item(value).Description;
                _ioDbCor0061.Offset = pVal.Row - 1;

                _ioMtx.GetLineData(pVal.Row);

                _ioDbCor0061.SetValue("U_DivName", pVal.Row - 1, desc);
                _ioMtx.SetLineData(pVal.Row);
                if (pVal.Row == _ioMtx.VisualRowCount)
                {
                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0061, false);
                }
            }
        }

        public override void MenuEventHandler(ref MenuEvent pVal, ref bool bubbleEvent)
        {
            if (!pVal.BeforeAction)
            {
                var oMenuEvent = pVal;
                if (oMenuEvent.MenuUID == "1282")
                {
                    _ioDbCor0061.RemoveRecord(0);
                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0061, true);
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