using System;
using Othello.Main.ViewModel;
using Othello.Main.Model;

namespace Othello.Main.Factories
{
    public class DiscViewModelFactory
    {
        Func<DiscViewModel> _creator;

        public DiscViewModelFactory(Func<DiscViewModel> creator)
        {
            _creator = creator;
        }

        public DiscViewModel Create(DiscModel Disc)
        {
            var vm = _creator();
            vm.Initialize(Disc);
            return vm;
        }

    }
}
