using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace InventorShaftGenerator.Extensions
{
    [DebuggerStepThrough]
    public class ObservableCollectionEx<T> : ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        public ObservableCollectionEx()
        {
        }

        public ObservableCollectionEx(List<T> list)
            : base(list != null ? new List<T>(list.Count) : list)
        {
            CopyFrom(list);
        }

        public ObservableCollectionEx(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            CopyFrom(collection);
        }

        private void CopyFrom(IEnumerable<T> collection)
        {
            IList<T> items = this.Items;
            if (collection != null)
            {
                using (IEnumerator<T> enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        items.Add(enumerator.Current);
                    }
                }
            }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            item.PropertyChanged += Item_PropertyChanged;
        }

        protected override void RemoveItem(int index)
        {
            this.Items[index].PropertyChanged -= Item_PropertyChanged;
            base.RemoveItem(index);
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            T removedItem = this[oldIndex];
            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, removedItem);
        }

        protected override void ClearItems()
        {
            foreach (T item in this.Items)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }

            base.ClearItems();
        }

        protected override void SetItem(int index, T item)
        {
            T oldItem = this.Items[index];
            T newItem = item;
            oldItem.PropertyChanged -= Item_PropertyChanged;
            newItem.PropertyChanged += Item_PropertyChanged;
            base.SetItem(index, item);
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var handler = ItemPropertyChanged;
            handler?.Invoke(sender, e);
        }

        public event PropertyChangedEventHandler ItemPropertyChanged;
    }
}