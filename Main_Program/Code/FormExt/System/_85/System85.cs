using SAPbouiCOM;

namespace EnerlifeCN.Code.FormExt.System._85
{
    public class System85 : SwBaseForm
    {
        private Button AutoMatch;
        private Button ClearBtn;
        private ComboBox Create;
        private Button GetAll;
        private Matrix mtx;

        public override void ItemEventHandler(string formUid, ref ItemEvent pVal, ref bool bubbleEvent)
        {
            if (pVal.EventType == BoEventTypes.et_FORM_RESIZE && !pVal.BeforeAction)
            {
                AutoMatch.Item.Height = Create.Item.Height;
                AutoMatch.Item.Width = Create.Item.Width;
                AutoMatch.Item.Top = Create.Item.Top;
                AutoMatch.Item.Left = Create.Item.Left - AutoMatch.Item.Width - 5;
            }
            if (pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction && pVal.ItemUID == "automatch")
            {
                if (ClearBtn.Item.Enabled || GetAll.Item.Enabled)
                {
                    ClearBtn.Item.Click();
                    GetAll.Item.Click();
                    for (var i = 1; i <= mtx.VisualRowCount; i++)
                    {
                        var cell = mtx.Columns.Item("19").Cells.Item(i);
                        if (((EditText) cell.Specific).Item.Enabled)
                        {
                            ((EditText) cell.Specific).Value = ((EditText) cell.Specific).Value;
                            Global.Application.SendKeys("^{TAB}");
                            var tform = Global.Application.Forms.ActiveForm;
                            tform.Items.Item("16").Click();
                            if (tform.Mode == BoFormMode.fm_UPDATE_MODE)
                            {
                                tform.Items.Item("1").Click();
                            }
                            tform.Items.Item("1").Click();
                        }
                    }
                }
            }
        }

        public override void FormCreate(string formUId, ref ItemEvent pVal, ref bool bubbleEvent)
        {
            AutoMatch = (Button) MyForm.Items.Add("automatch", BoFormItemTypes.it_BUTTON).Specific;
            AutoMatch.Caption = "自动匹配批次";
            Create = (ComboBox) MyForm.Items.Item("56").Specific;
            ClearBtn = (Button) MyForm.Items.Item("58").Specific;
            GetAll = (Button) MyForm.Items.Item("57").Specific;
            mtx = (Matrix) MyForm.Items.Item("11").Specific;
        }
    }
}