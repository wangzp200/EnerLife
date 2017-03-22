using SAPbouiCOM;

namespace EnerlifeCN.Code.FormExt.Other.MessageInfo
{
    internal class MessageInfo : SwBaseForm
    {
        private Item _ioMessageText;

        public override void FormCreate(string formUId, ref ItemEvent pVal, ref bool bubbleEvent)
        {
            _ioMessageText = MyForm.Items.Item("15");
        }

        public void SetMessage(string message)
        {
            if (_ioMessageText != null)
            {
                var mes = _ioMessageText.Specific as StaticText;
                if (mes != null) mes.Caption = message;
            }
        }
    }
}