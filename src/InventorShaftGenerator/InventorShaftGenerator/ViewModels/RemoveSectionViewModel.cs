using InventorShaftGenerator.Infrastructure;
using InventorShaftGenerator.Models;

namespace InventorShaftGenerator.ViewModels
{
    public class RemoveSectionViewModel : RemoveItemViewModel
    {
        protected override void RemoveItem()
        {
            if (this.Section.IsBore)
            {
                if (this.Section.BoreFromEdge == BoreFromEdge.FromLeft)
                {
                    Shaft.BoreOnTheLeft.Remove(this.Section);
                }
                else
                {
                    Shaft.BoreOnTheRight.Remove(this.Section);
                }
            }
            else
            {
                Shaft.Sections.Remove(this.Section);
            }
        }
    }
}