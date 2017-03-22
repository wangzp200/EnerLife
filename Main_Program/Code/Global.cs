using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EnerlifeCN.Code.FormExt;
using EnerlifeCN.Code.Model;
using EnerlifeCN.Code.Util;
using SAPbobsCOM;
using SAPbouiCOM;
using Company = SAPbobsCOM.Company;
using DataTable = System.Data.DataTable;

namespace EnerlifeCN.Code
{
    public class Global
    {
        public static Application Application;
        public static Company OCompany;
        public static Form CurrentForm;
        public static Recordset ORecordSet;
        public static readonly SortedList<string, SwBaseForm> SwFormsList = new SortedList<string, SwBaseForm>();
        public static readonly Random Random = new Random();
        public static DataTable FormSizeInfo;
        public static readonly Dictionary<string, string> FormCache = new Dictionary<string, string>();
        public static readonly string DllPath = Assembly.GetExecutingAssembly().Location;
        public static readonly string MyFormTmp = DllPath.Replace("\\EnerlifeCN.dll", "") + "\\EnerlifeCNTmp";
        public static readonly string FormSizeInfoPath = MyFormTmp + "\\formSizeInfo.xml";
        public static readonly string VersionPath = MyFormTmp + "\\version.txt";
        public static Company ScCompany;

        public static void GlobalIntial()
        {
            var menuItemList = new ArrayList();
            var topMenuItemId = "2048";
            var oMenuItem = new OMenuItem(BoMenuType.mt_POPUP, "COR00", "Free Goods Master Data", true, "", 19,
                topMenuItemId);
            menuItemList.Add(oMenuItem);
            oMenuItem = new OMenuItem(BoMenuType.mt_STRING, "COR0010", "By Customer by Product", true, "", 19, "COR00");
            menuItemList.Add(oMenuItem);
            oMenuItem = new OMenuItem(BoMenuType.mt_STRING, "COR0020", "By Channel by Product", true, "", 19, "COR00");
            menuItemList.Add(oMenuItem);

            oMenuItem = new OMenuItem(BoMenuType.mt_STRING, "COR0030", "By Zone by Product", true, "", 19, "COR00");
            menuItemList.Add(oMenuItem);

            oMenuItem = new OMenuItem(BoMenuType.mt_POPUP, "COR10", "Discount Master Data", true, "", 19, topMenuItemId);
            menuItemList.Add(oMenuItem);


            oMenuItem = new OMenuItem(BoMenuType.mt_STRING, "COR0040", "By Customer by Product", true, "", 19, "COR10");
            menuItemList.Add(oMenuItem);

            oMenuItem = new OMenuItem(BoMenuType.mt_STRING, "COR0050", "By Channel by Product", true, "", 19, "COR10");
            menuItemList.Add(oMenuItem);

            oMenuItem = new OMenuItem(BoMenuType.mt_STRING, "COR0060", "By Channel by Division", true, "", 19, "COR10");
            menuItemList.Add(oMenuItem);

            oMenuItem = new OMenuItem(BoMenuType.mt_STRING, "COR0160", "By Zone by Channel", true, "", 19, "COR10");
            menuItemList.Add(oMenuItem);

            oMenuItem = new OMenuItem(BoMenuType.mt_STRING, "COR0230", "By Channel By Number Of Item", true, "", 19,
                "COR10");
            menuItemList.Add(oMenuItem);


            oMenuItem = new OMenuItem(BoMenuType.mt_POPUP, "COR20", "Pricing Master Data", true, "", 19, topMenuItemId);
            menuItemList.Add(oMenuItem);

            oMenuItem = new OMenuItem(BoMenuType.mt_STRING, "COR0070", "By Customer By Product", true, "", 19, "COR20");
            menuItemList.Add(oMenuItem);

            oMenuItem = new OMenuItem(BoMenuType.mt_STRING, "COR0090", "By Channel By Product", true, "", 19, "COR20");
            menuItemList.Add(oMenuItem);

            oMenuItem = new OMenuItem(BoMenuType.mt_STRING, "COR0100", "By Zone By Product", true, "", 19, "COR20");
            menuItemList.Add(oMenuItem);

            oMenuItem = new OMenuItem(BoMenuType.mt_STRING, "COR0170", "By Chain by Product", true, "", 19, "COR20");
            menuItemList.Add(oMenuItem);


            oMenuItem = new OMenuItem(BoMenuType.mt_STRING, "COR0180", "By Type of Channel by Product", true, "", 19,
                "COR20");
            menuItemList.Add(oMenuItem);

            MenuItemsUtil.AddMenuItems(menuItemList, topMenuItemId);


            for (var i = 0; i < Application.Forms.Count; i++)
            {
                var formCmdCenter = Application.Forms.Item(i);
                if (formCmdCenter.Type == 169)
                {
                    formCmdCenter.Update();
                    formCmdCenter.Refresh();
                }
            }
            if (!Directory.Exists(MyFormTmp))
            {
                Directory.CreateDirectory(MyFormTmp);
            }
            var newVersion = Assembly.LoadFile(DllPath).GetName().Version;

            if (File.Exists(VersionPath))
            {
                var oldVersion = new Version(CommonUtil.ReadText(VersionPath));
                if (oldVersion < newVersion)
                {
                    if (Directory.Exists(MyFormTmp))
                    {
                        CommonUtil.DeleteFolder(MyFormTmp);
                    }
                    CommonUtil.SaveAsFile(newVersion.ToString(), VersionPath);
                }
            }
            else
            {
                if (Directory.Exists(MyFormTmp))
                {
                    CommonUtil.DeleteFolder(MyFormTmp);
                }
                CommonUtil.SaveAsFile(newVersion.ToString(), VersionPath);
            }


            if (File.Exists(FormSizeInfoPath))
            {
                FormSizeInfo = XmlAndTdHelper.GetInstance().XmlToDataTable(FormSizeInfoPath);
            }
            else
            {
                FormSizeInfo = new DataTable();
                FormSizeInfo.Columns.Add("FormTypeEx", typeof (string));
                FormSizeInfo.Columns.Add("Left", typeof (int));
                FormSizeInfo.Columns.Add("Top", typeof (int));
                FormSizeInfo.Columns.Add("Width", typeof (int));
                FormSizeInfo.Columns.Add("Height", typeof (int));
            }
        }

        public static void MenusAdd()
        {
        }
    }
}