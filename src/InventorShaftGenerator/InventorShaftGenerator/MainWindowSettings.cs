using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using InventorShaftGenerator.Annotations;

namespace InventorShaftGenerator
{
    class MainWindowSettings
    {
        public static bool Is3DPreviewVisible { get; set; } = true;
        public static bool Is2DPreviewPanelVisible { get; set; } = true;
        public static bool IsDimensionsPanelVisible { get; set; } = true;
    }
}
