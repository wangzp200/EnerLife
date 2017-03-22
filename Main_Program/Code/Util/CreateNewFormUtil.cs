using System;
using System.Reflection;
using EnerlifeCN.Code.FormExt;
using SAPbouiCOM;

namespace EnerlifeCN.Code.Util
{
    internal class CreateNewFormUtil
    {
        public static Form CreateNewForm(string formType, int sTops, int sLeft)
        {
            var thisExe = Assembly.GetExecutingAssembly();
            Type typeAssemblyeForm = null;
            Form oForm = null;
            foreach (var type in thisExe.GetTypes())
            {
                var sArray = type.FullName.Split('.');
                if (sArray[sArray.Length - 1].ToLower() == formType.ToLower())
                {
                    if (type.BaseType == typeof (SwBaseForm))
                    {
                        typeAssemblyeForm = type;
                    }
                }
            }
            if (typeAssemblyeForm != null)
            {
                var swSwBaseForm = (SwBaseForm) Activator.CreateInstance(typeAssemblyeForm);
                oForm = LoadModalXmlUtil.Execute(formType, ref sTops, ref sLeft, ref swSwBaseForm);
            }
            return oForm;
        }
    }
}