using System;
using System.Collections.ObjectModel;
using System.Drawing;
using Inventor;
using InventorShaftGenerator.Extensions;
using InventorShaftGenerator.Infrastructure.Helpers;
using InventorShaftGenerator.Models;

namespace InventorShaftGenerator.Infrastructure
{
    public sealed class Shaft
    {
        public static Application Application { get; } = StandardAddInServer.InventorApp;

        static Shaft()
        {
            Sections.ItemPropertyChanged += (sender, args) =>
            {
                SectionsParametersChanged?.Invoke(typeof(Shaft), EventArgs.Empty);
            };
            BoreOnTheLeft.ItemPropertyChanged += (sender, args) =>
            {
                BoreOnTheLeftParametersChanged?.Invoke(typeof(Shaft), EventArgs.Empty);
            };
            BoreOnTheRight.ItemPropertyChanged += (sender, args) =>
            {
                BoreOnTheRightParametersChanged?.Invoke(typeof(Shaft), EventArgs.Empty);
            };
            ShaftFeaturesErrors.CollectionChanged += (sender, args) =>
            {
                StandardAddInServer.MainWindow.UpdateBindingSources();
                ErrorsCollectionChanged?.Invoke(typeof(Shaft), EventArgs.Empty);
            };
        }

        public static ObservableCollectionEx<ShaftSection> Sections { get; } =
            new ObservableCollectionEx<ShaftSection>();

        public static ObservableCollectionEx<ShaftSection> BoreOnTheLeft { get; } =
            new ObservableCollectionEx<ShaftSection>();

        public static ObservableCollectionEx<ShaftSection> BoreOnTheRight { get; } =
            new ObservableCollectionEx<ShaftSection>();

        public static ObservableCollection<ShaftSectionFeatureError> ShaftFeaturesErrors { get; } =
            new ObservableCollection<ShaftSectionFeatureError>();

        public static SketchLineSimple RevolveAxis { get; set; } = new SketchLineSimple(PointF.Empty, PointF.Empty);

        public static SketchLineSimple BoreOnTheLeftRevolveAxis { get; set; } =
            new SketchLineSimple(PointF.Empty, PointF.Empty);

        public static SketchLineSimple BoreOnTheRightRevolveAxis { get; set; } =
            new SketchLineSimple(PointF.Empty, PointF.Empty);

        public static event EventHandler SectionsParametersChanged;
        public static event EventHandler ErrorsCollectionChanged;
        public static event EventHandler BoreOnTheLeftParametersChanged;
        public static event EventHandler BoreOnTheRightParametersChanged;
    }
}