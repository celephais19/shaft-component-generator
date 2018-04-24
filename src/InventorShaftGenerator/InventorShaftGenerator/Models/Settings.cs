using System;

namespace InventorShaftGenerator.Models
{
    public class Settings
    {
        public static bool Is3DPreviewEnabled
        {
            get => Properties.Settings.Default.Is3DPreviewEnabled;
            set
            {
                Properties.Settings.Default.Is3DPreviewEnabled = value;
                RaiseSettingsChanged();
            }
        }

        public static bool Is2DPreviewEnabled
        {
            get => Properties.Settings.Default.Is2DPreviewEnabled;
            set
            {
                Properties.Settings.Default.Is2DPreviewEnabled = value; 
                RaiseSettingsChanged();
            }
        }

        public static bool IsDimensionsPanelEnabled
        {
            get => Properties.Settings.Default.IsDimensionsPanelEnabled;
            set
            {
                Properties.Settings.Default.IsDimensionsPanelEnabled = value; 
                RaiseSettingsChanged();
            }
        }

        public static event EventHandler SettingsChanged;
    
        private static void RaiseSettingsChanged()
        {
            SettingsChanged?.Invoke(typeof(Settings), EventArgs.Empty);
        }
    }
}