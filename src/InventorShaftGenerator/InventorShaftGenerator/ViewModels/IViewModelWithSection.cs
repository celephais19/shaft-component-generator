using InventorShaftGenerator.Models;

namespace InventorShaftGenerator.ViewModels
{
    public interface IViewModelWithSection
    {
        ShaftSection Section { get; set; }
    }
}