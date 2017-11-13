using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace Aub.Xamarin.Toolkit.ViewModel
{
    public class ObservableCollectionSafe<T> : ObservableCollection<T>
    {
        private SynchronizationContext _syncContext = ViewModelBase.SynchronizationContext; //SynchronizationContext.Current;

        public ObservableCollectionSafe()
        {

        }

        public ObservableCollectionSafe(IEnumerable<T> list) : base(list)
        {

        }

        private void ExecuteOnSyncContext(Action action)
        {
            if (SynchronizationContext.Current == _syncContext && _syncContext!=null)
                action();
            else
                _syncContext.Send(_ => action(), null);
        }

        protected override void InsertItem(int index, T item)
        {
            ExecuteOnSyncContext(() => base.InsertItem(index, item));
        }

        protected override void RemoveItem(int index)
        {
            ExecuteOnSyncContext(() => base.RemoveItem(index));
        }

        protected override void SetItem(int index, T item)
        {
            ExecuteOnSyncContext(() => base.SetItem(index, item));
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            ExecuteOnSyncContext(() => base.MoveItem(oldIndex, newIndex));
        }

        protected override void ClearItems()
        {
            ExecuteOnSyncContext(() => base.ClearItems());
        }

        public void AddRange(IEnumerable<T> list)
        {
            foreach (var item in list)
            {
                base.Add(item);
            }
        }

    }
}
