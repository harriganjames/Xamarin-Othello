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

        public BoardViewModel Create(int rows, int columns)
        {
            var vm = _creator();
            vm.Initialize(rows,columns);
            return vm;
        }

    }
}
