using System.Collections.Generic;
using EnerlifeCN.Code.Util;
using SAPbouiCOM;

namespace EnerlifeCN.Code.FormExt.Custom.COR0230
{
    public class COR0230 : SwBaseForm
    {
        private readonly SortedList<string, string> UOMs = new SortedList<string, string>();
        private Button _ioBtnOk, _ioBtnCancle;
        private ComboBox _ioChannel;
        private DBDataSource _ioDbCor0230, _ioDbCor0231;
        private Matrix _ioMtx;
        private Item _ioRec;
        private ComboBox _ioSeries;

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

//--------------------------------------------初始化单位信息---------------------------------------------------------------------------------
            validValues = _ioMtx.Columns.Item("C5").ValidValues;
            sql = "SELECT UomEntry,UomCode FROM dbo.OUOM";
            Global.ORecordSet.DoQuery(sql);
            while (Global.ORecordSet.EoF == false)
            {
                validValues.Add(Global.ORecordSet.Fields.Item(1).Value as string,
                    Global.ORecordSet.Fields.Item(1).Value as string);
                UOMs.Add(Global.ORecordSet.Fields.Item(1).Value.ToString().Trim(),
                    Global.ORecordSet.Fields.Item(0).Value.ToString());
                Global.ORecordSet.MoveNext();
            }

            _ioDbCor0230 = MyForm.DataSources.DBDataSources.Item("@COR0230");
            _ioDbCor0231 = MyForm.DataSources.DBDataSources.Item("@COR0231");
            _ioDbCor0230.SetValue("U_DiscType", 0, "E");
            _ioDbCor0230.SetValue("U_Active", 0, "Y");

            Fieldlist.Add("@COR0231", "U_UoM");
            _ioDbCor0231.RemoveRecord(0);
            CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0231, true);
            MyFormResize();
        }

        public override void ItemEventHandler(string formUId, ref ItemEvent pVal, ref bool bubbleEvent)
        {
            //-------------------------------------------------------------------------------------------

            if (!pVal.BeforeAction && pVal.EventType == BoEventTypes.et_FORM_RESIZE)
            {
                MyFormResize();
            }
            //-----------------------------自动添加行----------------------------------------------
            if (!pVal.BeforeAction &&
                (pVal.EventType == BoEventTypes.et_MATRIX_LOAD || pVal.ItemChanged && pVal.ColUID == "C5") &&
                pVal.ActionSuccess)
            {
                if (_ioMtx.VisualRowCount > 0)
                {
                    var value = ((ComboBox) _ioMtx.Columns.Item("C5").Cells.Item(_ioMtx.VisualRowCount).Specific).Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0231, false);
                    }
                }
                else
                {
                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0231, false);
                }
            }

            if (!pVal.BeforeAction && pVal.EventType == BoEventTypes.et_COMBO_SELECT && pVal.ItemChanged)
            {
                if (pVal.ItemUID == "7")
                {
                    var desc = _ioChannel.ValidValues.Item(_ioChannel.Value.Trim()).Description;
                    _ioDbCor0230.SetValue("U_ChanName", 0, desc);
                }
                else if (pVal.ColUID == "C5")
                {
                    _ioDbCor0231.Offset = pVal.Row - 1;
                    _ioMtx.GetLineData(pVal.Row);
                    var key = ((ComboBox) _ioMtx.Columns.Item("C5").Cells.Item(pVal.Row).Specific).Value.Trim();
                    key = UOMs[key];
                    _ioDbCor0231.SetValue("U_UomEntry", pVal.Row - 1, key);
                    _ioMtx.GetLineData(pVal.Row);
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
                    _ioDbCor0231.RemoveRecord(0);
                    CommonUtil.MtxAddRow(_ioMtx, _ioDbCor0231, true);
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