using Aub.Xamarin.Toolkit.ViewModel;
using Othello.Main.Enum;
using Xamarin.Forms;

namespace Othello.Main.ViewModel
{
    public class CellViewModel : ViewModelBase
    {
        public CellViewModel()
        {

        }

        public void Initialize(int index)
        {
            Index = index;
        }

        private CellStateEnum _cellState = CellStateEnum.Off;
        public CellStateEnum CellState
        {
            get { return _cellState; }
            set
            {
                _cellState = value;
                NotifyPropertyChanged();
            }
        }

        public int Index { get; private set; }


        public void ToggleState()
        {
            if (CellState == CellStateEnum.Off)
                CellState = CellStateEnum.White;
            else if (CellState == CellStateEnum.White)
                CellState = CellStateEnum.Black;
            else if (CellState == CellStateEnum.Black)
                CellState = CellStateEnum.White;
        }


    }
}
        