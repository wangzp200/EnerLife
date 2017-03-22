using coresuiteFramework.Loader.Module;
using EnerlifeCN.Code.Event;
using EnerlifeCN.Code.Util;
using SAPbobsCOM;
using SwissAddonFramework;
using SwissAddonFramework.Messaging;
using SwissAddonFramework.UI.Components;
using Global = EnerlifeCN.Code.Global;

namespace EnerlifeCN
{
    public class ProgramIModule : IModule
    {
        public void CompanyChanged()
        {
            //throw new NotImplementedException();
        }

        public void CreateMenu(MenuItem menuItemConfiguration)
        {
            //throw new NotImplementedException();
        }

        public void Install()
        {
            //throw new NotImplementedException();
        }

        public void LanguageChanged()
        {
            //throw new NotImplementedException();
        }

        public string ModuleGuid
        {
            get { return "EnerlifeCN"; }
        }

        public string ModuleInfoLink
        {
            get { return "http://www.coresystems.ch"; }
        }

        public string ModuleName
        {
            get { return "Enerlife China"; }
        }

        public string ModuleVersion
        {
            get { return "2.21.00088"; }
        }

        public bool PreInstall()
        {
            //throw new NotImplementedException();
            return false;
        }

        public void Run()
        {
            StatusBar.WriteSucess("AddOn " + ModuleName + " is running...");
            Global.Application = B1Connector.GetB1Connector().Application;
            Global.OCompany = Global.Application.Company.GetDICompany() as Company;
            if (Global.OCompany != null)
                Global.ORecordSet = Global.OCompany.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;

            Global.GlobalIntial();

            SwissAddonFramework.Global.ItemEvent += SwItemEventHandler.ItemEventHandler;
            SwissAddonFramework.Global.ApplicationEvent += SwApplicationEventHandler.ApplicationEventHandler;
            SwissAddonFramework.Global.FormDataEvent += SwFormDataEventHandler.FormDataEventHandler;
            SwissAddonFramework.Global.FormLoadedEvent += SwFormLoadedEventHandler.FormLoadedEventHandler;
            SwissAddonFramework.Global.MenuEvent += SwMenuEventHandler.MenuEventHandler;
            SwissAddonFramework.Global.PrintEvent += SwPrintEventHandler.PrintEventHandler;
            SwissAddonFramework.Global.ProgressBarEvent += SwProgressBarEvent.ProgressBarEventHandler;
            SwissAddonFramework.Global.ReportDataEvent += SwReportDataEventHandler.ReportDataEventHandler;
            SwissAddonFramework.Global.RightClickEvent += SwRightClickHandler.RightClickHandler;
            SwissAddonFramework.Global.StatusBarEvent += SwStatusBarEventHandler.StatusBarEventHandler;
            Global.Application.LayoutKeyEvent += SwLayoutKeyEventHandler.LayoutKeyEventEventHandler;
        }

        public void Terminate()
        {
            if (Global.FormSizeInfo != null)
            {
                XmlAndTdHelper.GetInstance().DataTableToXml(Global.FormSizeInfo, Global.FormSizeInfoPath);
            }
            SwissAddonFramework.Global.ItemEvent -= SwItemEventHandler.ItemEventHandler;
            SwissAddonFramework.Global.ApplicationEvent -= SwApplicationEventHandler.ApplicationEventHandler;
            SwissAddonFramework.Global.FormDataEvent -= SwFormDataEventHandler.FormDataEventHandler;
            SwissAddonFramework.Global.FormLoadedEvent -= SwFormLoadedEventHandler.FormLoadedEventHandler;
            SwissAddonFramework.Global.MenuEvent -= SwMenuEventHandler.MenuEventHandler;
            SwissAddonFramework.Global.PrintEvent -= SwPrintEventHandler.PrintEventHandler;
            SwissAddonFramework.Global.ProgressBarEvent -= SwProgressBarEvent.ProgressBarEventHandler;
            SwissAddonFramework.Global.ReportDataEvent -= SwReportDataEventHandler.ReportDataEventHandler;
            SwissAddonFramework.Global.RightClickEvent -= SwRightClickHandler.RightClickHandler;
            SwissAddonFramework.Global.StatusBarEvent -= SwStatusBarEventHandler.StatusBarEventHandler;
            Global.Application.LayoutKeyEvent -= SwLayoutKeyEventHandler.LayoutKeyEventEventHandler;
        }
    }
}