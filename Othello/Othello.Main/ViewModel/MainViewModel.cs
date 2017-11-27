using Xamarin.Forms;
using Aub.Xamarin.Toolkit.ViewModel;

using Othello.Main.Enum;
using Othello.Main.Factories;

namespace Othello.Main.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        readonly GameViewModelFactory _gameViewModelFactory;
        public MainViewModel(GameViewModelFactory gameViewModelFactory)
        {
            _gameViewModelFactory = gameViewModelFactory;
        }

        public void Initialize()
        {
            Game = _gameViewModelFactory.Create();
        }


        public GameViewModel Game { get; set; }


    }
}
