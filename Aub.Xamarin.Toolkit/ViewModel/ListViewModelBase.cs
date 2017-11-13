using System.Collections.Generic;
using System.Linq;

namespace Aub.Xamarin.Toolkit.ViewModel
{
    public abstract class ListViewModelBase<T> : ViewModelBase where T : ItemViewModelBase
    {
        protected ObservableCollectionSafe<T> dataList = new ObservableCollectionSafe<T>();

        public ListViewModelBase()
        {
            dataList.CollectionChanged += _dataList_CollectionChanged;
        }

        // Manage the SelectedChanged events for the items in the list
        void _dataList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (ItemViewModelBase vm in e.NewItems)
                    vm.SelectedChanged += item_SelectedChanged;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (ItemViewModelBase vm in e.OldItems)
                    vm.SelectedChanged -= item_SelectedChanged;
        }

        void item_SelectedChanged(object sender, BooleanResultEventArgs e)
        {
            this.ItemSelectionChanged();
        }

        protected virtual void ItemSelectionChanged()
        {
            base.RefreshCommands();
        }



        public ObservableCollectionSafe<T> DataList
        {
            get
            {
                return dataList;
            }
        }

        public int GetQtySelected()
        {
            return dataList.Count(b => b.IsSelected);
        }
        public IEnumerable<T> GetSelectedItems()
        {
            return dataList.Where(b => b.IsSelected);
        }

        protected void ItemSelectionChangedHandler(object o, BooleanResultEventArgs e)
        {
            base.RefreshCommands();
        }


    }
}
