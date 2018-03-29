using Aub.Xamarin.Toolkit.ViewModel;
using Othello.Main.Enum;
using Othello.Main.Model;
using Xamarin.Forms;

namespace Othello.Main.ViewModel
{
    public class CellViewModel : ViewModelBase
    {
        CellModel _cellModel;

        public void Initialize(CellModel cellModel)
        {
            _cellModel = cellModel; ;
        }

        DiscViewModel _disc;
        public DiscViewModel Disc
        {
            get
            {
                return _disc;
            }
            set
            {
                SetValue(ref _disc, value, () => Disc);
            }
        }

        public CellModel Model => _cellModel;

        private bool _isPending;

        public bool IsPending
        {
            get { return _isPending; }
            set
            {
                SetValue(ref _isPending, value, () => IsPending);
            }
        }

        private bool _isPlaying;

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                SetValue(ref _isPlaying, value, () => IsPlaying);
            }
        }

        public override string ToString()
        {
            return Model.ToString();
        }
    }
}
        