using Xamarin.Forms;
using Aub.Xamarin.Toolkit.ViewModel;

using Othello.Main.Enum;
using Othello.Main.Factories;

namespace Othello.Main.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        readonly BoardViewModelFactory _boardViewModelFactory;
        public MainViewModel(BoardViewModelFactory boardViewModelFactory)
        {
            _boardViewModelFactory = boardViewModelFactory;

        }

        public void Initialize()
        {
            Board = _boardViewModelFactory.Create(8,8);

            ToggleCellCommand = AddNewCommand(new Command(OnToggleCell));
        }

        public Command ToggleCellCommand { get; set; }


        public BoardViewModel Board { get; set; }

        void OnToggleCell()
        {
            foreach (var cell in Board.Cells)
            {
                ToggleCellState(cell);
            }
        }

        void ToggleCellState(CellViewModel cell)
        {
            if (cell.CellState == CellStateEnum.Off)
                cell.CellState = CellStateEnum.White;
            else if (cell.CellState == CellStateEnum.White)
                cell.CellState = CellStateEnum.Black;
            else if (cell.CellState == CellStateEnum.Black)
                cell.CellState = CellStateEnum.White;
        }


    }
}
