using System;
using Othello.Main.ViewModel;

namespace Othello.Main.Factories
{
    public class BoardViewModelFactory
    {
        Func<BoardViewModel> _creator;

        public BoardViewModelFactory(Func<BoardViewModel> creator)
        {
            _creator = creator;
        }

        public BoardViewModel Create(int rows, int columns, Action<CellViewModel> clickAction)
        {
            var vm = _creator();
            vm.Initialize(rows,columns,clickAction);
            return vm;
        }

    }
}
