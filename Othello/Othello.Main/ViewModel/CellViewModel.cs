using Aub.Xamarin.Toolkit.ViewModel;
using Othello.Main.Enum;
using Othello.Main.Model;
using Xamarin.Forms;

namespace Othello.Main.ViewModel
{
    public class CellViewModel : ViewModelBase
    {
        public CellViewModel()
        {

        }

        public void Initialize(CellModel cell)
        {
            Cell = cell;
        }

        public CellStateEnum CellState
        {
            get { return Cell.State; }
        }

        public bool InUse => CellState != CellStateEnum.Empty;

        public CellModel Cell { get; private set; }

        private bool _isPending;

        public bool IsPending
        {
            get { return _isPending; }
            set
            {
                SetValue(ref _isPending, value, () => IsPending);
                NotifyPropertyChanged(() => InUse);
            }
        }

        private bool _isPlaying;

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                SetValue(ref _isPlaying, value, () => IsPlaying);
                NotifyPropertyChanged(() => InUse);
            }
        }

        private Rectangle _location;
        public Rectangle Location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public void NotifyChanged()
        {
            NotifyAllPropertiesChanged();
        }

    }
}
        