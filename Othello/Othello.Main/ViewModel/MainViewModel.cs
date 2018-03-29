using Xamarin.Forms;
using Aub.Xamarin.Toolkit.ViewModel;

using Othello.Main.Enum;
using Othello.Main.Factories;
using Aub.Xamarin.Toolkit.Service;
using Othello.Main.Model;
using System.Text;

namespace Othello.Main.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        readonly GameViewModelFactory _gameViewModelFactory;
        readonly IUserInterfaceService _userInterfaceService;

        public MainViewModel(GameViewModelFactory gameViewModelFactory, IUserInterfaceService userInterfaceService)
        {
            _gameViewModelFactory = gameViewModelFactory;
            _userInterfaceService = userInterfaceService;
        }

        public void Initialize()
        {
            Game = _gameViewModelFactory.Create();
            TestViewModel = new TestViewModel();
            NewGameCommand = AddNewCommand(new Command(OnNewGame));
        }

        public Command NewGameCommand { get; set; }


        public GameViewModel Game { get; set; }

        public string Title
        {
            get
            {
                var sb = new StringBuilder("Othello");
                if(Game!=null && Game.GameOptions!=null)
                {
                    var mode = Game.GameOptions.IsSinglePlayer ? "Player v Device" : "Two Players";
                    sb.Append($" - {mode}");
                }
                return sb.ToString();
            }
        }

        public TestViewModel TestViewModel { get; set; }

        async void OnNewGame()
        {
            var opt = new GameOptions() { IsSinglePlayer=Game.GameOptions.IsSinglePlayer };
            var vm = new NewGameViewModel();

            vm.Initialize(opt);

            var result = await _userInterfaceService.ShowView(vm);

            if (result == DialogResultEnum.Yes)
                Game.NewGame(vm.GameOptions);
            NotifyPropertyChanged(() => Title);
        }

    }
}
