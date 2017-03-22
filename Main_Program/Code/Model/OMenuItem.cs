using SAPbouiCOM;

namespace EnerlifeCN.Code.Model
{
    public class OMenuItem
    {
        public OMenuItem()
        {
        }

        public OMenuItem(string uniqueId, string caption, bool enabled, string image, int position, string fUniqueId)
            : this(0, uniqueId, caption, enabled, image, position, fUniqueId)
        {
        }

        public OMenuItem(BoMenuType type, string uniqueId, string caption, bool enabled, string image, int position,
            string fUniqueId)
        {
            Type = type;
            UniqueId = uniqueId;
            Caption = caption;
            Enabled = enabled;
            Image = image;
            Position = position;
            FUniqueId = fUniqueId;
        }

        public BoMenuType Type { get; set; }
        public string UniqueId { get; set; }
        public string Caption { get; set; }
        public bool Enabled { get; set; }
        public string Image { get; set; }
        public int Position { get; set; }
        public string FUniqueId { get; set; }
    }
}