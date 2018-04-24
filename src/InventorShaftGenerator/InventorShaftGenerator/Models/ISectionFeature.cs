using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace InventorShaftGenerator.Models
{
    public interface ISectionFeature : INotifyPropertyChanged
    {
        Guid Id { get; }
        ObservableCollection<ShaftSectionFeatureError> FeatureErrors { get; }
        bool ShouldBeBuilt { get; set; }
        ShaftSection LinkedSection { get; }
        void UpdateFeatureParameters();
        string DisplayName { get; }
        
        void InitializeInAccordanceWithSectionParameters(ShaftSection section,
            EdgeFeaturePosition? edgeFeaturePosition = null);
    }
}