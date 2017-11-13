using System;
using Othello.Main.ViewModel;

namespace Othello.Main.Factories
{
    public class CellViewModelFactory
    {
        Func<CellViewModel> _creator;

        public CellViewModelFactory(Func<CellViewModel> creator)
        {
            _creator = creator;
        }

        public CellViewModel Create(int index)
        {
            var vm = _creator();
            vm.Initialize(index);
            return vm;
        }

    }
}
