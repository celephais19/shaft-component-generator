using InventorShaftGenerator.Models.SubFeatures;

namespace InventorShaftGenerator.ViewModels
{
    public class RemoveSubFeatureViewModel : RemoveItemViewModel
    {
        private ISectionSubFeature subFeature;

        public ISectionSubFeature SubFeature
        {
            get => this.subFeature;
            set => SetProperty(ref this.subFeature, value);
        }

        protected override void RemoveItem()
        {
            this.Section.SubFeatures.Remove(this.SubFeature);
        }
    }
}