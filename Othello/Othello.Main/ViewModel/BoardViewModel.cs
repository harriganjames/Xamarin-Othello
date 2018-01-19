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
        readonly DiscViewModelFactory _discViewModelFactory;

        Dictionary<CellModel, CellViewModel> _cellLookup;
        Dictionary<DiscModel, DiscViewModel> _discLookup;

        Action<CellModel> _cellClickAction;

        public BoardViewModel(CellViewModelFactory cellViewModelFactory, DiscViewModelFactory discViewModelFactory)
        {
            _cellViewModelFactory = cellViewModelFactory;
            _discViewModelFactory = discViewModelFactory;

            CellTappedCommand = AddNewCommand(new Command(OnCellTapped));
        }


        public void Initialize(IEnumerable<CellModel> cells, IEnumerable<DiscModel> discs, Action<CellModel> cellClickAction)
        {
            Cells = new ObservableCollectionSafe<CellViewModel>();
            Discs = new ObservableCollectionSafe<DiscViewModel>();
            _cellLookup = new Dictionary<CellModel, CellViewModel>();
            _discLookup = new Dictionary<DiscModel, DiscViewModel>();
            foreach (var cell in cells)
            {
                var vm = _cellViewModelFactory.Create(cell);
                Cells.Add(vm);
                _cellLookup.Add(cell, vm);
            }
            foreach (var disc in discs)
            {
                var vm = _discViewModelFactory.Create(disc);
                Discs.Add(vm);
                _discLookup.Add(disc, vm);
            }
            _cellClickAction = cellClickAction;
        }

        public Command CellTappedCommand { get; private set; }

        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public ObservableCollectionSafe<CellViewModel> Cells { get; set; }
        public ObservableCollectionSafe<DiscViewModel> Discs { get; set; }

        void OnCellTapped(object param)
        {
            var cell = param as CellViewModel;
            if (cell == null)
                return;

            _cellClickAction?.Invoke(cell.Model);
        }

        public void UpdateBoard(PlaySetModel playSet)
        {
            foreach (var disc in playSet.Discs)
            {
                var discvm = GetDiscViewModel(disc);
                discvm.DiscColor = disc.DiscColor;
            }
            foreach (var cell in playSet.Cells)
            {
                var cellvm = GetCellViewModel(cell);
                DiscViewModel discvm = null;
                cellvm.IsPlaying = cell.IsPlaying;
                cellvm.IsPending = cell.IsPending;
                if (cell.Disc == null)
                {
                    // move disc from cell to stack
                    discvm = cellvm.Disc;
                    if (discvm != null)
                    {
                        discvm.InUse = false;
                    }
                    discvm = null;
                }
                else
                {
                    // move disc from stack to cell
                    discvm = GetDiscViewModel(cell.Disc);
                    discvm.InUse = true;
                }
                cellvm.Disc = discvm;
            }
        }

        CellViewModel GetCellViewModel(CellModel cell)
        {
            CellViewModel vm;
            _cellLookup.TryGetValue(cell, out vm);
            return vm;
        }

        DiscViewModel GetDiscViewModel(DiscModel disc)
        {
            DiscViewModel vm;
            _discLookup.TryGetValue(disc, out vm);
            return vm;
        }

    }
}
