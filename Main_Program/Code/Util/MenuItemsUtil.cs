using System.Collections;
using EnerlifeCN.Code.Model;
using SAPbouiCOM;

namespace EnerlifeCN.Code.Util
{
    public class MenuItemsUtil
    {
        private static readonly Application Application = Global.Application;

        public static void AddMenuItems()
        {
        }

        public static void AddMenuItems(ArrayList menuItemList, string topMenuItemId)
        {
            if (Application.Menus.Exists(topMenuItemId))
            {
                var topMenu = Application.Menus.Item(topMenuItemId);
                if (topMenu != null && topMenu.Type == BoMenuType.mt_POPUP)
                {
                    var oMenus = topMenu.SubMenus;
                    foreach (OMenuItem menuItem in menuItemList)
                    {
                        if (!Application.Menus.Exists(menuItem.UniqueId) && menuItem.FUniqueId == topMenuItemId)
                        {
                            var oCreationPackage =
                                Global.Application.CreateObject(BoCreatableObjectType.cot_MenuCreationParams) as
                                    MenuCreationParams;

                            if (oCreationPackage != null)
                            {
                                oCreationPackage.Type = menuItem.Type;
                                oCreationPackage.UniqueID = menuItem.UniqueId;
                                oCreationPackage.String = menuItem.Caption;
                                oCreationPackage.Enabled = menuItem.Enabled;

                                oCreationPackage.Position = oMenus.Count + 1;
                                oMenus.AddEx(oCreationPackage);
                            }

                            AddMenuItems(menuItemList, menuItem.UniqueId);
                        }
                    }
                }
            }
        }
    }
}