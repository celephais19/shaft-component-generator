using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures;
using InventorShaftGenerator.Models.EdgeFeatures.ReliefGOSTUnits;
using InventorShaftGenerator.Models.EdgeFeatures.ReliefsDINUnits;
using InventorShaftGenerator.Models.EdgeFeatures.ReliefsSIUnits;

namespace InventorShaftGenerator.ValueConverters
{
    public class EdgeFeatureToImageConverter : IValueConverter
    {
        private static readonly string IconsUri =
            "pack://application:,,,/InventorShaftGenerator;component/Assets/Icons/";

        private static readonly Dictionary<Type, string> Uris = new Dictionary<Type, string>
        {
            {typeof(ChamferEdgeFeature), IconsUri + "chamferIcon.png"},
            {typeof(FilletEdgeFeature), IconsUri + "filletIcon.png"},
            {typeof(ThreadEdgeFeature), IconsUri + "threadIcon.png"},
            {typeof(LockNutGrooveEdgeFeature), IconsUri + "lockNutIcon.png"},
            {typeof(PlainKeywayGrooveEdgeFeature), IconsUri + "plainKeywayGrooveIcon.png"},
            {typeof(KeywayGrooveRoundedEndEdgeFeature), IconsUri + "keywayGrooveRoundEndIcon.png"},
            {typeof(ReliefASIEdgeFeature), IconsUri + "reliefASiIcon.png"},
            {typeof(ReliefBSIEdgeFeature), IconsUri + "reliefBSiIcon.png"},
            {typeof(ReliefFSIEdgeFeature), IconsUri + "reliefFSiIcon.png"},
            {typeof(ReliefGSIEdgeFeature), IconsUri + "reliefGSiIcon.png"},
            {typeof(ReliefADinEdgeFeature), IconsUri + "reliefADinIcon.png"},
            {typeof(ReliefBDinEdgeFeature), IconsUri + "reliefBDinIcon.png"},
            {typeof(ReliefCDinEdgeFeature), IconsUri + "reliefCDinIcon.png"},
            {typeof(ReliefDDinEdgeFeature), IconsUri + "reliefDDinIcon.png"},
            {typeof(ReliefEDinEdgeFeature), IconsUri + "reliefEDinIcon.png"},
            {typeof(ReliefFDinEdgeFeature), IconsUri + "reliefFDinIcon.png"},
            {typeof(ReliefAGostEdgeFeature), IconsUri + "reliefAGostIcon.png"},
            {typeof(ReliefBGostEdgeFeature), IconsUri + "reliefBGostIcon.png"},
            {typeof(ReliefCGostEdgeFeature), IconsUri + "reliefCGostIcon.png"},
            {typeof(ReliefDGostEdgeFeature), IconsUri + "reliefDGostIcon.png"},
            {typeof(ReliefEGostEdgeFeature), IconsUri + "reliefEGostIcon.png"},
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var edgeFeatureType = value?.GetType();
            var edgeFeaturePosition = (EdgeFeaturePosition) parameter;
            if (edgeFeatureType == null)
            {
                var image = edgeFeaturePosition == EdgeFeaturePosition.SecondEdge
                    ? new BitmapImage(new Uri(IconsUri + "noneFeatureIconFlip.png"))
                    : new BitmapImage(new Uri(IconsUri + "noneFeatureIcon.png"));
                return image;
            }

            string imageUri = Uris[edgeFeatureType];

            if (edgeFeaturePosition is EdgeFeaturePosition.SecondEdge)
            {
                var l = imageUri.Length;
                var pngIdx = imageUri.IndexOf(".png");
                imageUri = imageUri.Insert(imageUri.IndexOf(".png", StringComparison.InvariantCultureIgnoreCase),
                    "Flip");
            }

            return new BitmapImage(new Uri(imageUri));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}