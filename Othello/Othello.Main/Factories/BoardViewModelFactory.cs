using System;
using Othello.Main.ViewModel;
using System.Collections.Generic;
using Othello.Main.Model;

namespace Othello.Main.Factories
{
    public class BoardViewModelFactory
    {
        Func<BoardViewModel> _creator;

        public BoardViewModelFactory(Func<BoardViewModel> creator)
        {
            _creator = creator;
        }

        public BoardViewModel Create(IEnumerable<CellModel> cells, IEnumerable<DiscModel> discs, Action<CellModel> clickAction)
        {
            var vm = _creator();
            vm.Initialize(cells,discs,clickAction);
            return vm;
        }

    }
}
