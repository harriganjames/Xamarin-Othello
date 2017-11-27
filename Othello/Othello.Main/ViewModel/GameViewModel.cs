using Aub.Xamarin.Toolkit.ViewModel;
using Othello.Main.Engine;
using Othello.Main.Enum;
using Othello.Main.Factories;
using Othello.Main.Model;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Othello.Main.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        readonly BoardViewModelFactory _boardViewModelFactory;
        readonly OthelloEngineFactory _othelloEngineFactory;

        OthelloEngine _othelloEngine;

        public GameViewModel(BoardViewModelFactory boardViewModelFactory,
            OthelloEngineFactory othelloEngineFactory)
        {
            _boardViewModelFactory = boardViewModelFactory;
            _othelloEngineFactory = othelloEngineFactory;

            NewGameCommand = AddNewCommand(new Command(OnNewGame));
            ConfirmCommand = AddNewCommand(new Command(OnConfirm));
            UndoCommand = AddNewCommand(new Command(OnUndo));
        }


        public void Initialize()
        {
            Board = _boardViewModelFactory.Create(8, 8, OnCellClick);
            _othelloEngine = _othelloEngineFactory.Create(Board.Cells.Select(c => c.Cell));

            OnNewGame();
        }


        public Command NewGameCommand { get; set; }
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

        private CellStateEnum _turn;
        public CellStateEnum Turn
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

        private bool _isPending;

        public bool IsPending
        {
            get { return _isPending; }
            set
            {
                SetValue(ref _isPending, value, () => IsPending);
            }
        }


        void OnNewGame()
        {
            _othelloEngine.NewGame();
            UpdateBoard();
        }

        void OnConfirm()
        {
            _othelloEngine.Confirm();
            IsPending = false;
            UpdateBoard();
        }

        void OnUndo()
        {
            _othelloEngine.UndoLastSequence();
            IsPending = false;
            UpdateBoard();
        }

        void UpdateBoard()
        {
            Board.UpdateBoard(_othelloEngine.Sequence,IsPending);
            WhiteScore = PointsToString(_othelloEngine.WhitePoints,_othelloEngine.WhitePointsPending);
            BlackScore = PointsToString(_othelloEngine.BlackPoints,_othelloEngine.BlackPointsPending);
            Turn = _othelloEngine.Turn;
        }

        string PointsToString(int points, int pendingPoints)
        {
            var sb = new StringBuilder();
            sb.Append(points.ToString());
            if(pendingPoints!=0)
            {
                sb.Append(pendingPoints < 0 ? " - " : " + ");
                sb.Append(System.Math.Abs(pendingPoints).ToString());
            }
            return sb.ToString();
        }

        void OnCellClick(CellViewModel cell)
        {
            if (_othelloEngine.PlayCell(cell.Cell))
            {
                IsPending = true;
                UpdateBoard();
            }
        }

        void SetCell(CellModel trans)
        {
            SetCell(trans.Column, trans.Row, trans.State);
        }

        void SetCell(int column, int row, CellStateEnum state)
        {
            Board.Cells[8 * row + column].Cell.State = state;
        }

    }
}
