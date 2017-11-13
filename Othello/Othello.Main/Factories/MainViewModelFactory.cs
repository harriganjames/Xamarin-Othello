using System;
using Othello.Main.ViewModel;

namespace Othello.Main.Factories
{
    public class MainViewModelFactory
    {
        readonly Func<MainViewModel> _creator;

        public MainViewModelFactory(Func<MainViewModel> creator)
        {
            _creator = creator;
        }

        public MainViewModel Create()
        {
            var vm = _creator();
            vm.Initialize();
            return vm;
        }
    }
}
