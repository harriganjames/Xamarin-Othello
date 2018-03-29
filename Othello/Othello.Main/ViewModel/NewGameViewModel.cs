using Aub.Xamarin.Toolkit.Service;
using Aub.Xamarin.Toolkit.ViewModel;
using Othello.Main.Enum;
using Othello.Main.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Othello.Main.ViewModel
{
    public class NewGameViewModel : ViewModelBase, IViewModelDialog
    {
        Action<DialogResultEnum> _requestClose;
        bool _startNewGame = false;
        GameOptions _gameOptions;// = new GameOptions();
        ObservableCollectionSafe<PlayerModeViewModel> _playerModeList = new ObservableCollectionSafe<PlayerModeViewModel>();
        PlayerModeViewModel _playerMode;
        public NewGameViewModel()
        {

        }

        public void Initialize(GameOptions gameOptions)
        {
            _gameOptions = gameOptions;
            StartNewGameCommand = AddNewCommand(new Command(OnStartNewGame));
            InitializePlayerModes();
            SyncProperties();
        }

        void InitializePlayerModes()
        {
            PlayerModes.Add(new PlayerModeViewModel().Initialize(PlayerModeEnum.PlayerPlayer,"Two Players"));
            PlayerModes.Add(new PlayerModeViewModel().Initialize(PlayerModeEnum.PlayerDevice,"Single Player"));
            SelectedPlayerMode = PlayerModes[1];
        }


        public Command StartNewGameCommand { get; set; }

        private bool _isPlayerPlayer;

        public bool IsPlayerPlayer
        {
            get { return _isPlayerPlayer; }
            set
            {
                SetValue(ref _isPlayerPlayer, value, () => IsPlayerPlayer);

            }
        }

        private bool _isPlayerDevice;

        public bool IsPlayerDevice
        {
            get { return _isPlayerDevice; }
            set
            {
                SetValue(ref _isPlayerDevice, value, () => IsPlayerDevice);
                SyncGameOptions();
            }
        }

        public ObservableCollectionSafe<PlayerModeViewModel> PlayerModes => _playerModeList;

        PlayerModeViewModel _selectedPlayerMode;
        public PlayerModeViewModel SelectedPlayerMode
        {
            get
            {
                return _selectedPlayerMode;
            }
            set
            {
                SetValue(ref _selectedPlayerMode, value, () => SelectedPlayerMode);
            }
        }

        public GameOptions GameOptions => _gameOptions;

        public bool StartNewGame => _startNewGame;

        void OnStartNewGame()
        {
            _startNewGame = true;
            _requestClose?.Invoke(DialogResultEnum.Yes);
        }


        void SyncGameOptions()
        {
            _gameOptions.IsSinglePlayer = _isPlayerDevice;

            //if (_isPlayerDevice)
            //    _gameOptions.PlayerMode = PlayerModeEnum.PlayerDevice;
            //if (_isPlayerPlayer)
            //    _gameOptions.PlayerMode = PlayerModeEnum.PlayerPlayer;
        }

        void SyncProperties()
        {
            _isPlayerDevice = _gameOptions.IsSinglePlayer;
            _isPlayerPlayer = !_gameOptions.IsSinglePlayer;
            //_isPlayerPlayer = _gameOptions.PlayerMode == PlayerModeEnum.PlayerPlayer;
            //_isPlayerDevice = _gameOptions.PlayerMode == PlayerModeEnum.PlayerDevice;
        }


        #region IViewModelDialog
        public Action<DialogResultEnum> RequestClose { set => _requestClose = value; }
        public string PageTitle => "New Game";
        #endregion
    }


}
