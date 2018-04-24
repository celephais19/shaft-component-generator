using InventorShaftGenerator.Models.SubFeatures;

namespace InventorShaftGenerator.ViewModels.SectionSubFeatureViewModels
{
    public interface ISubFeatureViewModel
    {
        ISectionSubFeature SubFeature { get; set; }
    }
}