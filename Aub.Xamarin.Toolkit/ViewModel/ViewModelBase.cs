using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace Aub.Xamarin.Toolkit.ViewModel
{
    public abstract class ViewModelBase : NotifyBase, IDisposable
    {
        string _title;
        int _busyCount = 0;
        object _busyCountLock = new object();
        List<Command> _commands = new List<Command>();
        TaskScheduler uiScheduler;

        private static SynchronizationContext _syncContext;
        public static SynchronizationContext SynchronizationContext
        {
            get
            {
                if (_syncContext == null)
                    _syncContext = SynchronizationContext.Current;
                return _syncContext;
            }
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelBase()
        {
            if(SynchronizationContext.Current!=null)
                this.uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }


        public TaskScheduler UIScheduler
        {
            get
            {
                return this.uiScheduler;
            }
        }

        protected Command AddNewCommand(Command c)
        {
            _commands.Add(c);
            return c;
        }

        protected List<Command> Commands
        {
            get
            {
                return _commands;
            }
        }

        protected void RefreshCommands()
        {
            _commands.ForEach(c => c.ChangeCanExecute());
        }


        public void IncrementBusy()
        {
            lock (_busyCountLock)
            {
                _busyCount++;
            }
            NotifyPropertyChanged(() => IsBusy);
        }
        public void DecrementBusy()
        {
            lock(_busyCountLock)
            {
                if (_busyCount > 0)
                    _busyCount--;
            }
            NotifyPropertyChanged(() => IsBusy);
        }

        public bool IsBusy
        {
            get
            {
                return _busyCount!=0;
            }
            set
            {
                if (value)
                    IncrementBusy();
                else
                    DecrementBusy();
            }
        }


        public virtual new void Dispose()
        {
            _commands = null;
            base.Dispose();
        }


    }
}
