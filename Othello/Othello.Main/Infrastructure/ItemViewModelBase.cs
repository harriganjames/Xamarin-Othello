using System;

namespace Othello.Main.Infrastructure
{
    public abstract class ItemViewModelBase : ViewModelBase
    {
        bool _isSelected;

        public event EventHandler<BooleanResultEventArgs> SelectedChanged;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnSelectedChanged(value);
                    NotifyPropertyChanged();
                }
            }
        }

        public virtual int Id
        {
            get { return 0;  }
        }

        protected void OnSelectedChanged(bool selectedChanged)
        {
            if (this.SelectedChanged != null)
                SelectedChanged(this, new BooleanResultEventArgs(selectedChanged));
        }

    }
}
