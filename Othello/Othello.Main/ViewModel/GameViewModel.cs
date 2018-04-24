using Aub.Xamarin.Toolkit.ViewModel;
using Othello.Main.Engine;
using Othello.Main.Enum;
using Othello.Main.Factories;
using Othello.Main.Model;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Aub.Xamarin.Toolkit.Service;
using System;
using Othello.Main.Tools;

namespace Othello.Main.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        readonly BoardViewModelFactory _boardViewModelFactory;
        readonly OthelloEngineFactory _othelloEngineFactory;
        readonly IPersistenceService _persistenceService;

        GameOptions _gameOptions;
        OthelloEngine _othelloEngine;
        GameProgress _gameProgress;

        public GameViewModel(BoardViewModelFactory boardViewModelFactory,
            OthelloEngineFactory othelloEngineFactory,
            IPersistenceService persistenceService)
        {
            _boardViewModelFactory = boardViewModelFactory;
            _othelloEngineFactory = othelloEngineFactory;
            _persistenceService = persistenceService;

            ConfirmCommand = AddNewCommand(new Command(OnConfirm));
            UndoCommand = AddNewCommand(new Command(OnUndo));
        }


        public void Initialize()
        {
            _othelloEngine = _othelloEngineFactory.Create();
            Board = _boardViewModelFactory.Create(_othelloEngine.Cells, _othelloEngine.Discs, OnCellClick);

            _othelloEngine.DevicePlayed += OnDevicePlayed;
            _othelloEngine.DeviceConfirmed += OnDeviceConfirmed;

            if(!RestoreFromGameProgress())
                NewGame(new GameOptions());
        }

        public Command ConfirmCommand { get; set; }
        public Command UndoCommand { get; set; }
        public BoardViewModel Board { get; private set; }

        private string _whiteScore;
        public string WhiteScore
        {
            get { return _whiteScore; }
            set
            {
                SetValue(ref _whiteScore, value, ()=>WhiteScore);
            }
        }

        private string _blackScore;
        public string BlackScore
        {
            get { return _blackScore; }
            set
            {
                SetValue(ref _blackScore, value, () => BlackScore);
            }
        }

        private OthelloColor _turn;
        public OthelloColor Turn
        {
            get { return _turn; }
            set
            {
                if (_turn != value)
                {
                    _turn = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //private bool _isPending;
        //public bool IsPending
        //{
        //    get { return _isPending; }
        //    set
        //    {
        //        SetValue(ref _isPending, value, () => IsPending);
        //    }
        //}

        private bool _isGameOver;
        public bool IsGameOver
        {
            get { return _isGameOver; }
            set
            {
                SetValue(ref _isGameOver, value, () => IsGameOver);
            }
        }

        public GameOptions GameOptions => _gameOptions;

        private GameStateEnum _gameState;

        public GameStateEnum GameState
        {
            get { return _gameState; }
            set
            {
                SetValue(ref _gameState, value, () => GameState);
            }
        }


        private string _gameOverText;
        public string GameOverText
        {
            get { return _gameOverText; }
            set
            {
                SetValue(ref _gameOverText, value, () => GameOverText);
            }
        }


        public void NewGame(GameOptions gameOptions)
        {
            _gameOptions = gameOptions;
            _othelloEngine.NewGame(gameOptions);
            //GameState = _othelloEngine.GameState;
            //IsPending = false;
            UpdateGame();
        }

        void OnConfirm()
        {
            _othelloEngine.Confirm();
            //GameState = _othelloEngine.GameState;
            //IsPending = false;
            UpdateGame();
            //IsGameOver = _othelloEngine.IsGameOver;
            SaveGameProgress();
        }

        void OnUndo()
        {
            _othelloEngine.UndoLastSequence();
            //GameState = _othelloEngine.GameState;
            //IsPending = false;
            UpdateGame();
        }


        void OnDevicePlayed(object sender, EventArgs e)
        {
            UpdateGame();
        }

        void OnDeviceConfirmed(object sender, EventArgs e)
        {
            UpdateGame();
            SaveGameProgress();
        }

        void UpdateGame()
        {
            Board.UpdateBoard(_othelloEngine.PlaySet);
            WhiteScore = PointsToString(_othelloEngine.WhitePoints,_othelloEngine.WhitePointsPending);
            BlackScore = PointsToString(_othelloEngine.BlackPoints,_othelloEngine.BlackPointsPending);
            Turn = _othelloEngine.Turn;
            GameState = _othelloEngine.GameState;
            UpdateGameOverText();
        }

        void UpdateGameOverText()
        {
            if (GameState == GameStateEnum.GameOver)
            {
                var difference = Math.Abs(_othelloEngine.WhitePoints - _othelloEngine.BlackPoints);
                string result;
                if (difference == 0)
                {
                    result = "a draw!";
                }
                else
                {
                    var points_suffix = difference == 1 ? String.Empty : "s";
                    string winnerString;
                    var winnerTurn = Turn.GetOpposite();
                    if (_gameOptions.IsSinglePlayer)
                    {
                        if (winnerTurn == OthelloColor.White)
                            winnerString = "Player";
                        else
                            winnerString = "Device";
                    }
                    else
                    {
                        winnerString = winnerTurn.ToString();
                    }
                    result = $"{winnerString} wins by {difference} point{points_suffix}!";
                }
                GameOverText = $"Game Over - {result}";
            }
            else
                GameOverText = String.Empty;
        }

        string PointsToString(int points, int pendingPoints)
        {
            var sb = new StringBuilder();
            sb.Append(points.ToString());
            if(pendingPoints!=0)
            {
                sb.Append(pendingPoints < 0 ? " - " : " + ");
                sb.Append(Math.Abs(pendingPoints).ToString());
            }
            return sb.ToString();
        }

        void OnCellClick(CellModel cell)
        {
            if (_othelloEngine.PlayerPlayCell(cell))
            {
                GameState = _othelloEngine.GameState;
                //IsPending = true;
                UpdateGame();
            }
        }


        void SaveGameProgress()
        {
            _gameProgress = _othelloEngine.GetGameProgress();
            _persistenceService.Save("gameProgress",_gameProgress);
        }

        bool RestoreFromGameProgress()
        {
            _gameProgress = _persistenceService.Get<GameProgress>("gameProgress");
            if(_gameProgress!=null)
            {
                _gameOptions = _gameProgress.GameOptions;
                _othelloEngine.RestoreFromGameProgress(_gameProgress);
                UpdateGame();
                return true;
            }
            return false;
        }

    }
}
