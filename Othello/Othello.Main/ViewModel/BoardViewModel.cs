using Aub.Xamarin.Toolkit.ViewModel;
using Othello.Main.Factories;
using Othello.Main.Model;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Othello.Main.ViewModel
{
    public class BoardViewModel : ViewModelBase
    {
        readonly CellViewModelFactory _cellViewModelFactory;

        Action<CellViewModel> _cellClickAction;

        public BoardViewModel(CellViewModelFactory cellViewModelFactory)
        {
            _cellViewModelFactory = cellViewModelFactory;

            Cells = new ObservableCollectionSafe<CellViewModel>();
            CellTappedCommand = AddNewCommand(new Command(OnCellTapped));
        }


        public void Initialize(int rows, int columns, Action<CellViewModel> cellClickAction)
        {
            Rows = rows;
            Columns = columns;
            CellSpacing = 6.0;
            _cellClickAction = cellClickAction;
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    var model = new CellModel(column, row);
                    Cells.Add(_cellViewModelFactory.Create(model));
                }
            }
        }

        public Command CellTappedCommand { get; private set; }

        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public ObservableCollectionSafe<CellViewModel> Cells { get; set; }
        public double CellSpacing { get; set; }

        private PlaySetModel _playSet;

        public PlaySetModel PlaySet
        {
            get { return _playSet; }
            set
            {
                _playSet = value;
                NotifyPropertyChanged();
            }
        }


        void OnCellTapped(object param)
        {
            var cell = param as CellViewModel;
            if (cell == null)
                return;

            _cellClickAction?.Invoke(cell);
        }

        public void UpdateBoard(List<CellTransitionModel> cells, bool isPending)
        {
            bool isPlayingSet = false;
            foreach (var cell in cells)
            {
                var vm = GetCellViewModel(cell.Cell);
                if (!isPlayingSet)
                {
                    vm.IsPlaying = isPending;
                    isPlayingSet = true;
                }
                else
                    vm.IsPending = isPending;
                vm.NotifyChanged();
            }
        }

        CellViewModel GetCellViewModel(CellModel cell)
        {
            return Cells[8 * cell.Row + cell.Column];
        }
    }
}
