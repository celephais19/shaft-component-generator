using InventorShaftGenerator.Infrastructure;

namespace InventorShaftGenerator.ViewModels
{
    public class RemoveAllViewModel : RemoveItemViewModel
    {
        protected override void RemoveItem()
        {
            Shaft.BoreOnTheLeft.Clear();
            Shaft.BoreOnTheRight.Clear();
            Shaft.Sections.Clear();
        }
    }
}
