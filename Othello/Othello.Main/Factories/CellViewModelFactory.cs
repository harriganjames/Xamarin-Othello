using System;
using Othello.Main.ViewModel;
using Othello.Main.Model;

namespace Othello.Main.Factories
{
    public class CellViewModelFactory
    {
        Func<CellViewModel> _creator;

        public CellViewModelFactory(Func<CellViewModel> creator)
        {
            _creator = creator;
        }

        public CellViewModel Create(CellModel cell)
        {
            var vm = _creator();
            vm.Initialize(cell);
            return vm;
        }

    }
}
