using System;
using System.Linq;
using System.Windows;
using InventorShaftGenerator.Commands;
using InventorShaftGenerator.Infrastructure;
using InventorShaftGenerator.Models;
using InventorShaftGenerator.Models.EdgeFeatures;
using InventorShaftGenerator.Models.EdgeFeatures.ReliefEnums;
using InventorShaftGenerator.Models.Sections;
using InventorShaftGenerator.Models.SubFeatures;
using InventorShaftGenerator.ViewModels.SectionDimensionsViewModels;
using InventorShaftGenerator.ViewModels.SectionSubFeatureViewModels;
using InventorShaftGenerator.Views;
using ISectionFeature = InventorShaftGenerator.Models.ISectionFeature;

namespace InventorShaftGenerator.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool is2DPreviewEnabled = true;
        private bool isDimensionsPanelEnabled = true;
        private string comboboxSelectedItem = "Sections";
        private RelayCommand insertCylinderCommand;
        private RelayCommand insertConeCommand;
        private RelayCommand insertPolygonCommand;
        private RelayCommand insertCylindricalBoreCommand;
        private RelayCommand insertConicalBoreCommand;
        private RelayCommand splitSelectedSectionCommand;
        private RelayCommand expandOrCollapseAllChildrenCommand;
        private RelayCommand sectionClickCommand;
        private RelayCommand setSelectedSubFeatureCommand;
        private float shaftLength;
        private ShaftSection selectedSection;
        private RelayCommand setEdgeFeatureCommand;
        private bool allChildrenExpanded;
        private RelayCommand openDialogViewCommand;
        private RelayCommand setSelectedSectionCommand;
        private RelayCommand buildShaftCommand;
        private RelayCommand setNoneFeatureAtFirstEdgeCommand;
        private RelayCommand setActiveEdgeFeaturePosition;
        private RelayCommand addSubfeatureCommand;
        private EdgeFeaturePosition activeEdgeFeaturePosition;
        private ISectionSubFeature selectedSubFeature;
        private ShaftSection sectionUnderMouse;
        private RelayCommand setSectionUnderMouseCommand;
        private RelayCommand setSubfeatureUnderMouseCommand;
        private RelayCommand replaceSectionCommand;
        private ISectionFeature subFeatureUnderMouse;
        private bool noSectionsHaveBeenSet;
        private bool shaftHasFeatureErrors;
        private float boreLeftLength;
        private float boreRightLength;
        private Type constructionErrorsViewType;

        public MainViewModel()
        {
            Settings.SettingsChanged += OnAppSettingsChanged;
            Shaft.Sections.CollectionChanged += (sender, args) => UpdateShaftLength();
            Shaft.BoreOnTheLeft.CollectionChanged += (sender, args) => UpdateBoreLeftLength();
            Shaft.BoreOnTheRight.CollectionChanged += (sender, args) => UpdateBoreLeftLength();
            Shaft.SectionsParametersChanged += (sender, args) => UpdateShaftLength();
            Shaft.BoreOnTheLeftParametersChanged += (sender, args) => UpdateBoreLeftLength();
            Shaft.BoreOnTheRightParametersChanged += (sender, args) => UpdateBoreRightLength();
            Shaft.ErrorsCollectionChanged += (sender, args) => UpdateShaftLength();

            void UpdateShaftLength() => this.ShaftLength = Shaft.Sections.Sum(section => section.Length);
            void UpdateBoreLeftLength() => this.BoreLeftLength = Shaft.BoreOnTheLeft.Sum(section => section.Length);

            void UpdateBoreRightLength() =>
                this.BoreRightLength = Shaft.BoreOnTheRight.Sum(section => section.Length);
        }

        public ShaftSection SelectedSection
        {
            get => this.selectedSection;
            set => SetProperty(ref this.selectedSection, value);
        }

        public ShaftSection SectionUnderMouse
        {
            get => this.sectionUnderMouse;
            set => SetProperty(ref this.sectionUnderMouse, value);
        }

        public ISectionSubFeature SelectedSubFeature
        {
            get => this.selectedSubFeature;
            set => SetProperty(ref this.selectedSubFeature, value);
        }

        public ISectionFeature SubFeatureUnderMouse
        {
            get => this.subFeatureUnderMouse;
            set => SetProperty(ref this.subFeatureUnderMouse, value);
        }

        public RelayCommand ExpandOrCollapseAllChildrenCommand =>
            this.expandOrCollapseAllChildrenCommand ?? (this.expandOrCollapseAllChildrenCommand =
                new RelayCommand(o => this.AllChildrenExpanded = !this.allChildrenExpanded));

        public RelayCommand InsertCylinderCommand =>
            this.insertCylinderCommand ?? (this.insertCylinderCommand = new RelayCommand(o => InsertCylinder()));

        public RelayCommand InsertConeCommand =>
            this.insertConeCommand ?? (this.insertConeCommand = new RelayCommand(o => InsertCone()));

        public RelayCommand InsertPolygonCommand =>
            this.insertPolygonCommand ?? (this.insertPolygonCommand = new RelayCommand(o => InsertPolygon()));

        public RelayCommand InsertCylindricalBoreCommand => this.insertCylindricalBoreCommand ??
                                                            (this.insertCylindricalBoreCommand = new RelayCommand(o =>
                                                                InsertCylindricalBore((BoreFromEdge) o)));

        public RelayCommand InsertConicalBoreCommand => this.insertConicalBoreCommand ??
                                                        (this.insertConicalBoreCommand = new RelayCommand(o =>
                                                            InsertConicalBore((BoreFromEdge) o)));

        public RelayCommand SplitSelectedSectionCommand =>
            this.splitSelectedSectionCommand ?? (this.splitSelectedSectionCommand =
                new RelayCommand(splitViewType => SplitSelectedSection((Type) splitViewType),
                    CanSplitSelectedSectionCommandExecute));

        public RelayCommand SectionClickCommand =>
            this.sectionClickCommand ??
            (this.sectionClickCommand = new RelayCommand(o => this.SelectedSection = (ShaftSection) o));


        public RelayCommand SetEdgeFeatureCommand =>
            this.setEdgeFeatureCommand ?? (this.setEdgeFeatureCommand =
                new RelayCommand(o =>
                {
                    Type edgeFeatureType = null;
                    switch (o)
                    {
                        case EdgeFeature.None:
                            SetNoneFeature();
                            return;
                        case EdgeFeature edgeFeatureEnumMember:
                            edgeFeatureType = edgeFeatureEnumMember.ToEdgeFeatureType();
                            break;
                        case ReliefSIFeature reliefSiFeatureEnumMember:
                            edgeFeatureType = reliefSiFeatureEnumMember.ToReliefSIEdgeFeatureType();
                            break;
                        case ReliefDINFeature reliefDinFeatureEnumMember:
                            edgeFeatureType = reliefDinFeatureEnumMember.ToReliefDINEdgeFeatureType();
                            break;
                        case ReliefGOSTFeature reliefGostFeatureEnumMember:
                            edgeFeatureType = reliefGostFeatureEnumMember.ToReliefGOSTEdgeFeatureType();
                            break;
                    }

                    SetEdgeFeature(edgeFeatureType);
                }));

        public RelayCommand AddSubFeatureCommand =>
            this.addSubfeatureCommand ?? (this.addSubfeatureCommand =
                new RelayCommand(o => AddSubFeature(((SubFeature) o).ToSubFeatureType())));

        public RelayCommand SetNoneEdgeFeatureCommand =>
            this.setNoneFeatureAtFirstEdgeCommand ?? (this.setNoneFeatureAtFirstEdgeCommand =
                new RelayCommand(o => SetNoneFeature()));

        public RelayCommand OpenDialogViewCommand => this.openDialogViewCommand ??
                                                     (this.openDialogViewCommand =
                                                         new RelayCommand(o => OpenDialogView((Type) o)));

        public RelayCommand SetSelectedSectionCommand =>
            this.setSelectedSectionCommand ?? (this.setSelectedSectionCommand = new RelayCommand(
                o => this.SelectedSection = (ShaftSection) o));

        public RelayCommand SetSectionUnderMouseCommand => this.setSectionUnderMouseCommand ??
                                                           (this.setSectionUnderMouseCommand = new RelayCommand(o =>
                                                               this.SectionUnderMouse = (ShaftSection) o));

        public RelayCommand SetSelectedSubFeatureCommand =>
            this.setSelectedSubFeatureCommand ?? (this.setSelectedSubFeatureCommand =
                new RelayCommand(o => this.SelectedSubFeature = (ISectionSubFeature) o));

        public RelayCommand SetSubFeatureUnderMouseCommand => this.setSubfeatureUnderMouseCommand ??
                                                              (this.setSubfeatureUnderMouseCommand =
                                                                  new RelayCommand(o =>
                                                                      this.SubFeatureUnderMouse = (ISectionFeature) o));

        public RelayCommand SetActiveEdgeFeaturePositionCommand =>
            this.setActiveEdgeFeaturePosition ?? (this.setActiveEdgeFeaturePosition =
                new RelayCommand(o => this.ActiveEdgeFeaturePosition = (EdgeFeaturePosition) o));

        public RelayCommand BuildShaftCommand =>
            this.buildShaftCommand ?? (this.buildShaftCommand =
                new RelayCommand(BuildShaft,
                    BuildShaftCommandCanExecuteEvaluator));

        public RelayCommand ReplaceSectionCommand =>
            this.replaceSectionCommand ?? (this.replaceSectionCommand = new RelayCommand(
                o =>
                {
                    switch (o)
                    {
                        case Section.Cylinder:
                            if (this.selectedSection.IsBore)
                            {
                                InsertCylindricalBore(this.selectedSection.BoreFromEdge.Value, replace: true);
                            }
                            else
                            {
                                InsertCylinder(replace: true);
                            }

                            break;

                        case Section.Cone:
                            if (this.selectedSection.IsBore)
                            {
                                InsertConicalBore(this.selectedSection.BoreFromEdge.Value, replace: true);
                            }
                            else
                            {
                                InsertCone(replace: true);
                            }

                            break;

                        case Section.Polygon:
                            InsertPolygon(replace: true);
                            break;
                    }
                }));

        private bool BuildShaftCommandCanExecuteEvaluator(object o)
        {
            this.ShaftHasFeatureErrors = Shaft.Sections.Any(section =>
                section.FirstEdgeFeatureHasErrors ||
                section.SecondEdgeFeatureHasErrors ||
                section.SubFeatures.Any(feature => feature.FeatureErrors.Any()));
            this.NoSectionsHaveBeenSet = !Shaft.Sections.Any();

            if (this.shaftHasFeatureErrors)
            {
                return false;
            }

            if (this.noSectionsHaveBeenSet)
            {
                return false;
            }

            return true;
        }

        public bool NoSectionsHaveBeenSet
        {
            get => this.noSectionsHaveBeenSet;
            set => SetProperty(ref this.noSectionsHaveBeenSet, value);
        }

        public bool ShaftHasFeatureErrors
        {
            get => this.shaftHasFeatureErrors;
            set => SetProperty(ref this.shaftHasFeatureErrors, value);
        }

        public string ComboboxSelectedItem
        {
            get => this.comboboxSelectedItem;
            set
            {
                bool newValueWasSetted = SetProperty(ref this.comboboxSelectedItem, value);
                if (newValueWasSetted)
                {
                    this.SelectedSection = null;
                }
            }
        }

        public bool Is2DPreviewEnabled
        {
            get => this.is2DPreviewEnabled;
            set => SetProperty(ref this.is2DPreviewEnabled, value);
        }

        public float ShaftLength
        {
            get => this.shaftLength;
            set => SetProperty(ref this.shaftLength, value);
        }

        public float BoreLeftLength
        {
            get => this.boreLeftLength;
            set => SetProperty(ref this.boreLeftLength, value);
        }

        public float BoreRightLength
        {
            get => this.boreRightLength;
            set => SetProperty(ref this.boreRightLength, value);
        }

        public bool IsDimensionsPanelEnabled
        {
            get => this.isDimensionsPanelEnabled;
            set => SetProperty(ref this.isDimensionsPanelEnabled, value);
        }

        private void SplitSelectedSection(Type splitViewType)
        {
            var dialogView = (IDialogView) Activator.CreateInstance(splitViewType);
            var scsvm = (SplitCylinderDimensionsViewModel) dialogView.DataContext;
            var cylinderSection = (CylinderSection) this.selectedSection;
            scsvm.Section = cylinderSection;

            bool? accepted = dialogView.ShowDialog();
            if (accepted is false)
            {
                return;
            }

            SectionManager<CylinderSection>.SplitCylinderSection(cylinderSection, scsvm.MainDiameter1,
                scsvm.MainDiameter2,
                scsvm.SectionLength1,
                scsvm.SectionLength2);
        }

        public bool AllChildrenExpanded
        {
            get => this.allChildrenExpanded;
            set => SetProperty(ref this.allChildrenExpanded, value);
        }

        public EdgeFeaturePosition ActiveEdgeFeaturePosition
        {
            get => this.activeEdgeFeaturePosition;
            set => SetProperty(ref this.activeEdgeFeaturePosition, value);
        }

        public Type ConstructionErrorsViewType
        {
            get => this.constructionErrorsViewType;
            set => SetProperty(ref this.constructionErrorsViewType, value);
        }

        private void BuildShaft(object obj)
        {
            var buildErrors = ShaftBuilder.BuildShaft();
            if (buildErrors.Any())
            {
                var buildErrorsView = (IView) Activator.CreateInstance(this.constructionErrorsViewType);
                var viewModel = (BuildErrorsViewModel) buildErrorsView.DataContext;
                viewModel.BuildErrors = buildErrors;

                buildErrorsView.Show();
            }
            else
            {
                StandardAddInServer.MainWindow.Visibility = Visibility.Collapsed;
            }
        }


        private void OpenDialogView(Type dialogViewType)
        {
            var dialogView = (IDialogView) Activator.CreateInstance(dialogViewType);

            if (dialogView.DataContext is IViewModelWithSection viewModelWithSection)
            {
                switch (viewModelWithSection)
                {
                    case IEdgeFeatureViewModel edgeFeatureViewModel:
                        edgeFeatureViewModel.EdgeFeaturePosition = this.activeEdgeFeaturePosition;
                        break;
                    case ISubFeatureViewModel sectionSubFeatureViewModel:
                        sectionSubFeatureViewModel.SubFeature = this.selectedSubFeature;
                        break;
                }

                viewModelWithSection.Section = this.selectedSection;

                switch (viewModelWithSection)
                {
                    case RemoveSectionViewModel _:
                        if (this.selectedSection.FirstEdgeFeature == null &&
                            this.selectedSection.SecondEdgeFeature == null && !this.selectedSection.SubFeatures.Any())
                        {
                            if (this.selectedSection.IsBore)
                            {
                                if (this.selectedSection.BoreFromEdge == BoreFromEdge.FromLeft)
                                {
                                    Shaft.BoreOnTheLeft.Remove(this.selectedSection);
                                }
                                else
                                {
                                    Shaft.BoreOnTheRight.Remove(this.selectedSection);
                                }
                            }
                            else
                            {
                                Shaft.Sections.Remove(this.selectedSection);
                            }

                            this.SelectedSection = null;
                            dialogView.Close();
                            return;
                        }

                        this.SelectedSection = null;
                        break;

                    case RemoveSubFeatureViewModel removeSubFeatureViewModel:
                        removeSubFeatureViewModel.SubFeature = this.selectedSubFeature;
                        this.SelectedSubFeature = null;
                        break;
                }
            }

            dialogView.ShowDialog();
        }

        private bool CanSplitSelectedSectionCommandExecute(object obj)
        {
            return this.selectedSection is CylinderSection;
        }

        private void SetEdgeFeature(Type edgeFeatureType)
        {
            var newSectionEdgeFeature = (ISectionFeature) Activator.CreateInstance(edgeFeatureType);
            newSectionEdgeFeature.InitializeInAccordanceWithSectionParameters(
                this.selectedSection, this.activeEdgeFeaturePosition);

            if (this.activeEdgeFeaturePosition == EdgeFeaturePosition.FirstEdge)
            {
                this.selectedSection.FirstEdgeFeature = newSectionEdgeFeature;
            }
            else
            {
                this.selectedSection.SecondEdgeFeature = newSectionEdgeFeature;
            }
        }

        private void AddSubFeature(Type subFeatureType)
        {
            var newSubFeature = (ISectionSubFeature) Activator.CreateInstance(subFeatureType);
            newSubFeature.InitializeInAccordanceWithSectionParameters(this.selectedSection);
            this.selectedSection.SubFeatures.Add(newSubFeature);
        }

        private void SetNoneFeature()
        {
            if (this.activeEdgeFeaturePosition == EdgeFeaturePosition.FirstEdge)
            {
                this.selectedSection.FirstEdgeFeature = null;
            }
            else
            {
                this.selectedSection.SecondEdgeFeature = null;
            }
        }

        private void InsertCylinder(bool replace = false)
        {
            SectionManager<CylinderSection>.InstallSection(this.SelectedSection, replace: replace);
        }

        private void InsertCone(bool replace = false)
        {
            SectionManager<ConeSection>.InstallSection(this.SelectedSection, replace: replace);
        }

        private void InsertPolygon(bool replace = false)
        {
            SectionManager<PolygonSection>.InstallSection(this.SelectedSection, replace: replace);
        }

        private void InsertCylindricalBore(BoreFromEdge fromEdge, bool replace = false)
        {
            SectionManager<CylinderSection>.InstallSection(
                this.selectedSection != null && this.selectedSection.IsBore ? this.selectedSection : null,
                isBoreSection: true,
                borePosition: fromEdge, replace: replace);
        }

        private void InsertConicalBore(BoreFromEdge fromEdge, bool replace = false)
        {
            SectionManager<ConeSection>.InstallSection(
                this.selectedSection != null && this.selectedSection.IsBore ? this.selectedSection : null,
                isBoreSection: true, borePosition: fromEdge, replace: replace);
        }

        private void OnAppSettingsChanged(object sender, EventArgs eventArgs)
        {
            this.Is2DPreviewEnabled = Settings.Is2DPreviewEnabled;
            this.IsDimensionsPanelEnabled = Settings.IsDimensionsPanelEnabled;
        }
    }
}