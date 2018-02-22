using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using Autodesk.ADN.Utility.InventorUtils;
using Autodesk.ADN.Utility.WinUtils;
using Inventor;
using InventorShaftGenerator.Properties;
using InventorShaftGenerator.Views;
using Application = Inventor.Application;

namespace InventorShaftGenerator
{
    [Guid("547985E3-4BFD-4C63-92CE-B956945AD8AA")]
    [ComVisible(true)]
    public class StandardAddInServer : ApplicationAddInServer
    {
        public static Application InventorApp { get; set; }

        private ButtonDefinition mainButtonDefinition;
        public static MainWindow MainWindow { get; private set; }

        public void Activate(ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            InventorApp = addInSiteObject.Application;

            AdnInventorUtilities.Initialize(InventorApp, this.GetType());
            AddMainButtonToRibbon();
        }

        private void AddMainButtonToRibbon()
        {
            ControlDefinitions ctrlDefs = InventorApp.CommandManager.ControlDefinitions;
            var currAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Icon icon32 = Resources.shaft_32x32;
            Icon icon16 = Resources.shaft_16x16;
            var pictureDisp32 = PictureDispConverter.ToIPictureDisp(icon32);
            var pictureDisp16 = PictureDispConverter.ToIPictureDisp(icon16);

            mainButtonDefinition = ctrlDefs.AddButtonDefinition(
                DisplayName: "Shaft",
                InternalName: "Autodesk:InventorShaftGenerator:MainCtrl",
                Classification: CommandTypesEnum.kEditMaskCmdType,
                ClientId: AdnInventorUtilities.AddInGuid,
                ToolTipText: "Shaft Component Generator",
                StandardIcon: pictureDisp16,
                LargeIcon: pictureDisp32
            );
            mainButtonDefinition.OnExecute += OnMainButtonExecute;

            Ribbon partRibbon = InventorApp.UserInterfaceManager.Ribbons["Part"];
            RibbonTab tab = partRibbon.RibbonTabs["id_TabTools"];
            RibbonPanel panel;

            try
            {
                panel = tab.RibbonPanels["Autodesk:InventorShaftGenerator:ToolsPanel"];
            }
            catch
            {
                panel = tab.RibbonPanels.Add(
                    DisplayName: "Tools Panel",
                    InternalName: "Autodesk:InventorShaftGenerator:ToolsPanel",
                    ClientId: AdnInventorUtilities.AddInGuid
                );
            }

            panel.CommandControls.AddButton(
                ButtonDefinition: mainButtonDefinition,
                UseLargeIcon: true
            );
        }

        private void OnMainButtonExecute(NameValueMap context)
        {
            if (MainWindow == null)
            {
                MainWindow = new MainWindow();
                MainWindow.Show();
            }
            else
            {
                MainWindow.Focus();
                MainWindow.Visibility = Visibility.Visible;
            }
        }

        public void Deactivate()
        {
            Marshal.ReleaseComObject(InventorApp);
            InventorApp = null;

            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public void ExecuteCommand(int commandId)
        {
            // ignored
        }

        public object Automation { get; } // ignored
    }
}