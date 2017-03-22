using System;
using System.Data;
using System.IO;
using System.Reflection;
using EnerlifeCN.Code.FormExt;
using MSXML2;
using SAPbouiCOM;

namespace EnerlifeCN.Code.Util
{
    public class LoadModalXmlUtil
    {
        public static Form Execute(string sFileName, ref int sTops, ref int sLeft, ref SwBaseForm swBaseForm)
        {
            int lHeight = 0, lWidth = 0;
            var oXmlDoc = new DOMDocument();
            var formXml = GetFormXml(sFileName);
            oXmlDoc.loadXML(formXml);
            var uid = GetUid();
            oXmlDoc.selectSingleNode("Application/forms/action/form/@uid").nodeValue = uid.ToString();
            if (sLeft > 0 && sTops > 0)
            {
                oXmlDoc.selectSingleNode("Application/forms/action/form/@left").nodeValue = sLeft.ToString();
                oXmlDoc.selectSingleNode("Application/forms/action/form/@top").nodeValue = sTops.ToString();
            }
            else
            {
                var exists = false;
                foreach (DataRow entry in Global.FormSizeInfo.Rows)
                {
                    if (entry["FormTypeEx"].ToString() == sFileName)
                    {
                        oXmlDoc.selectSingleNode("Application/forms/action/form/@left").nodeValue =
                            entry["Left"].ToString();
                        oXmlDoc.selectSingleNode("Application/forms/action/form/@top").nodeValue =
                            entry["Top"].ToString();
                        lHeight = (int) entry["Height"];
                        lWidth = (int) entry["Width"];
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    var left = Global.Application.Desktop.Width;
                    var height = Global.Application.Desktop.Height;
                    oXmlDoc.selectSingleNode("Application/forms/action/form/@left").nodeValue =
                        (left/2 - int.Parse(oXmlDoc.selectSingleNode("Application/forms/action/form/@width").nodeValue)/
                         2).ToString();
                    oXmlDoc.selectSingleNode("Application/forms/action/form/@top").nodeValue =
                        (height/2 -
                         int.Parse(oXmlDoc.selectSingleNode("Application/forms/action/form/@height").nodeValue)/
                         2).ToString();
                }
            }
            swBaseForm.MyFormUid = uid.ToString();
            Global.SwFormsList.Add(swBaseForm.MyFormUid, swBaseForm);
            Global.Application.LoadBatchActions(oXmlDoc.xml);
            var oForm = Global.Application.Forms.Item(uid.ToString());
            oForm.Visible = true;
            swBaseForm.Event = Global.Application.Forms.GetEventForm(uid.ToString());


            if (lWidth > 0 && lHeight > 0)
            {
                try
                {
                    oForm.Freeze(true);
                    oForm.Resize(lWidth, lHeight);
                }
                catch (Exception exception)
                {
                    Global.Application.SetStatusBarMessage(exception.Message, BoMessageTime.bmt_Short);
                }
                finally
                {
                    oForm.Freeze(false);
                }
            }

            return oForm;
        }

        private static int GetUid()
        {
            var uid = 0;
            while (true)
            {
                uid = Global.Random.Next(-100000, 100000);
                var exists = false;
                for (var i = 0; i < Global.Application.Forms.Count; i++)
                {
                    var oForm = Global.Application.Forms.Item(i);
                    if (oForm.UniqueID == uid.ToString())
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    break;
                }
            }
            return uid;
        }

        private static string GetFormXml(string sFileName)
        {
            var formXml = "";
            if (Global.FormCache.ContainsKey(sFileName))
            {
                formXml = Global.FormCache[sFileName];
            }
            else if (File.Exists(Global.MyFormTmp + "\\" + sFileName + ".ftxt"))
            {
                var sr = new StreamReader(Global.MyFormTmp + "\\" + sFileName + ".ftxt");
                var filestring = sr.ReadToEnd();
                formXml = ZipFileHelper.DecompressString(filestring);
                Global.FormCache.Add(sFileName, formXml);
            }
            else
            {
                string sXmlFile = null;
                var thisExe = Assembly.GetExecutingAssembly();
                foreach (var name in thisExe.GetManifestResourceNames())
                {
                    var sArray = name.Split('.');
                    if (sArray[sArray.Length - 2] == sFileName && sArray[sArray.Length - 1].ToLower() == "xml")
                    {
                        sXmlFile = name;
                        break;
                    }
                }

                if (sXmlFile != null)
                {
                    var file = thisExe.GetManifestResourceStream(sXmlFile);
                    if (file != null)
                    {
                        var sr = new StreamReader(file);
                        formXml = sr.ReadToEnd();
                    }
                }
            }
            return formXml;
        }
    }
}