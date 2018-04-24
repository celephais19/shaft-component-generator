using System.Collections.Generic;
using InventorShaftGenerator.Infrastructure;

namespace InventorShaftGenerator.ViewModels
{
    public sealed class BuildErrorsViewModel : ViewModelBase
    {
        private List<FeatureConstructionError> buildErrors;

        public List<FeatureConstructionError> BuildErrors
        {
            get => this.buildErrors;
            set => SetProperty(ref this.buildErrors, value);
        }
    }
}