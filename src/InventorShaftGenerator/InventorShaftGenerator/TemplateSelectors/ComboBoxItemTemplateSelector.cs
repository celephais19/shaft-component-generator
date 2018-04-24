using System.Windows;
using System.Windows.Controls;
using InventorShaftGenerator.Extensions;

namespace InventorShaftGenerator.TemplateSelectors
{
    public class ComboBoxItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SelectedTemplate { get; set; }
        public DataTemplate DropDownTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ComboBoxItem comboBoxItem = container.FindAncestor<ComboBoxItem>();
            return comboBoxItem == null ? this.SelectedTemplate : this.DropDownTemplate;
        }
    }
}
