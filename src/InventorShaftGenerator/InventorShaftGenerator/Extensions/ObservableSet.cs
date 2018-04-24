using System.Collections.ObjectModel;

namespace InventorShaftGenerator.Extensions
{
    public class ObservableSet<T> : ObservableCollection<T>
    {
        protected override void InsertItem(int index, T item)
        {
            if (this.Contains(item))
            {
                return;
            }

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            int i = this.IndexOf(item);
            if (i >= 0 && i != index)
            {
                return;
            }

            base.SetItem(index, item);
        }
    }
}