namespace InventorShaftGenerator
{
    public class Settings
    {
        public static bool Is3DPreviewEnabled
        {
            get => Properties.Settings.Default.Is3DPreviewEnabled;
            set => Properties.Settings.Default.Is3DPreviewEnabled = value;
        }

        public static bool Is2DPreviewEnabled
        {
            get => Properties.Settings.Default.Is2DPreviewEnabled;
            set => Properties.Settings.Default.Is2DPreviewEnabled = value;
        }

        public static bool IsDimensionsPanelEnabled
        {
            get => Properties.Settings.Default.IsDimensionsPanelEnabled;
            set => Properties.Settings.Default.IsDimensionsPanelEnabled = value;
        }
    }
}