using Aub.Xamarin.Toolkit.ViewModel;
using Othello.Main.Factories;
using Xamarin.Forms;

namespace Othello.Main.ViewModel
{
    public class BoardViewModel : ViewModelBase
    {
        readonly CellViewModelFactory _cellViewModelFactory;

        public BoardViewModel(CellViewModelFactory cellViewModelFactory)
        {
            _cellViewModelFactory = cellViewModelFactory;

            Cells = new ObservableCollectionSafe<CellViewModel>();
            CellTappedCommand = AddNewCommand(new Command(OnCellClicked));
        }


        public void Initialize(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            CellSpacing = 6.0;
            for (int i = 0; i < Rows * Columns; i++)
            {
                Cells.Add(_cellViewModelFactory.Create(i));
            }
        }

        public Command CellTappedCommand { get; private set; }

        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public ObservableCollectionSafe<CellViewModel> Cells { get; set; }
        public double CellSpacing { get; set; }

        void OnCellClicked(object param)
        {
            var cell = param as CellViewModel;
            if (cell == null)
                return;

            cell.ToggleState();
        }


    }
}
