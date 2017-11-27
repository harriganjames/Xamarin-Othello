using System;
using Othello.Main.ViewModel;

namespace Othello.Main.Factories
{
    public class GameViewModelFactory
    {
        Func<GameViewModel> _creator;

        public GameViewModelFactory(Func<GameViewModel> creator)
        {
            _creator = creator;
        }

        public GameViewModel Create()
        {
            var vm = _creator();
            vm.Initialize();
            return vm;
        }

    }
}
