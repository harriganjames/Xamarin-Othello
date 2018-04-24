using Othello.Main.Enum;
using Othello.Main.Model;
using System.Linq;
using System.Collections.Generic;
using Othello.Main.Tools;
using System;
using Xamarin.Forms;
using System.Diagnostics;

namespace Othello.Main.Engine
{
    public class OthelloEngine
    {
        CellModel[] _board;
        List<DiscModel> _discs;
        PlaySetModel _playSet;
        GameOptions _gameOptions;

        public OthelloEngine()
        {
            _discs = new List<DiscModel>();
            _playSet = new PlaySetModel();
        }

        public void Initialize()
        {
            _board = new CellModel[8*8];

            for (int column = 0; column < 8; column++)
            {
                for (int row = 0; row < 8; row++)
                {
                    _board[8 * row + column] = new CellModel(column,row);
                }
            }

            for (int d = 0; d < 64; d++)
            {
                DiscModel disc;
                if (d%2==0)
                {
                    disc = new DiscModel(OthelloColor.White);
                }
                else
                {
                    disc = new DiscModel(OthelloColor.Black);
                }
                _discs.Add(disc);
            }

            GameState = GameStateEnum.NotStarted;
        }

        public OthelloColor Turn { get; private set; }

        public int WhitePoints { get; private set; }
        public int BlackPoints { get; private set; }
        public int WhitePointsPending { get; private set; }
        public int BlackPointsPending { get; private set; }
        //public bool IsPlaying { get; private set; }
        public GameStateEnum GameState { get; private set; }

        public PlaySetModel PlaySet => _playSet;

        public IEnumerable<CellModel> Cells => _board;
        public IEnumerable<DiscModel> Discs => _discs;

        //public object Debug { get; private set; }

        public event EventHandler DevicePlayed;
        public event EventHandler DeviceConfirmed;

        public void Reset()
        {
            _playSet.Reset();
            foreach (var cell in _board)
            {
                cell.Disc = null;
                cell.IsPending = false;
                cell.IsPlaying = false;
            }
            foreach (var disc in _discs)
            {
                disc.Cell = null;
                disc.DiscColor = disc.InitialColor;
            }
        }


        public void NewGame(GameOptions gameOptions)
        {
            _playSet.Reset();
            _gameOptions = gameOptions;

            foreach (var cell in Cells.Where(c=>c.Disc!=null))
            {
                Debug.WriteLine($"NewGame clear cell C={cell.Column} R={cell.Row}");
                MoveDiscFromCell(cell);
            }

            MoveNextDiscToCell(OthelloColor.White, GetCell(3, 3));
            MoveNextDiscToCell(OthelloColor.Black, GetCell(3, 4));
            MoveNextDiscToCell(OthelloColor.Black, GetCell(4, 3));
            MoveNextDiscToCell(OthelloColor.White, GetCell(4, 4));

            Turn = OthelloColor.White;
            WhitePoints = 2;
            BlackPoints = 2;
            WhitePointsPending = 0;
            BlackPointsPending = 0;
            //IsPlaying = false;

            GameState = GameStateEnum.WaitingPlayerPlay;
        }

        public GameProgress GetGameProgress()
        {
            return new GameProgress()
            {
                GameOptions = _gameOptions,
                Turn = Turn,
                BlackPoints = BlackPoints,
                WhitePoints = WhitePoints,
                Cells = new List<GameProgress.Cell>(Cells
                                                    .Where(c => c.Disc != null)
                                                    .Select(c => new GameProgress.Cell()
                                                    {
                                                        Column = c.Column,
                                                        Row = c.Row,
                                                        DiscColor = c.Disc.DiscColor
                                                    }))
            };
        }

        public void RestoreFromGameProgress(GameProgress progress)
        {
            Reset();
            Turn = progress.Turn;
            BlackPoints = progress.BlackPoints;
            WhitePoints = progress.WhitePoints;
            _gameOptions = progress.GameOptions;
            MoveNextDiscToCell(OthelloColor.White, GetCell(3, 3));
            MoveNextDiscToCell(OthelloColor.Black, GetCell(3, 4));
            MoveNextDiscToCell(OthelloColor.Black, GetCell(4, 3));
            MoveNextDiscToCell(OthelloColor.White, GetCell(4, 4));
            foreach (var progressCell in progress.Cells)
            {
                var cell = GetCell(progressCell.Column, progressCell.Row);
                Debug.WriteLine($"Restore cell R{progressCell.Row},C{progressCell.Column} {progressCell.DiscColor}");
                if (cell.Disc == null)
                    MoveNextDiscToCell(progressCell.DiscColor, cell);
                else if (cell.Disc.DiscColor != progressCell.DiscColor)
                    FlipCellDisc(cell);
            }
            if (_gameOptions.IsSinglePlayer && Turn == OthelloColor.Black)
            {
                GameState = GameStateEnum.WaitingDevice;
                Device.StartTimer(TimeSpan.FromMilliseconds(1000), DevicePlayCallback);
            }
            else
            {
                GameState = GameStateEnum.WaitingPlayerPlay;
            }
        }

        public bool PlayerPlayCell(CellModel playCell)
        {
            //if (GameState == GameStateEnum.WaitingDevice && Turn == OthelloColor.Black)
            //{
            //    DevicePlay();
            //    return true;
            //}



            if (playCell.Disc != null)
                return false;

            if (GameState != GameStateEnum.WaitingPlayerPlay)
                return false;

            return PlayCell(playCell);
        }

        bool PlayCell(CellModel playCell)
        {
            PrintCells("PlayCell.Start");

            var acs = GetAdjacentCells(playCell);

            var starts = acs.Where(c => c.Disc!=null && c.Disc.DiscColor==Turn.GetOpposite()).ToList();

            if (!starts.Any())
                return false;

            var spokes = GetSpokes(playCell, starts);
            if (!spokes.Any())
                return false;

            _playSet.Reset();
            playCell.IsPlaying = true;
            MoveNextDiscToCell(Turn, playCell);
            IncrementPendingPoints(Turn);
            foreach (var spoke in spokes)
            {
                foreach (var cell in spoke)
                {
                    cell.IsPending = true;
                    FlipCellDisc(cell);
                    IncrementPendingPoints(Turn);
                    DecrementPendingPoints(Turn.GetOpposite());
                }
            }
            //IsPlaying = true;
            if(Turn==OthelloColor.White || !_gameOptions.IsSinglePlayer)
                GameState = GameStateEnum.WaitingPlayerConfirm;

            PrintCells("PlayCell.End");

            return true;
        }

        public void Confirm()
        {
            if (GameState != GameStateEnum.WaitingPlayerConfirm && GameState!=GameStateEnum.WaitingDevice)
                return;

            foreach (var cell in _playSet.Cells)
            {
                cell.IsPending = false;
                cell.IsPlaying = false;
            }
            WhitePoints += WhitePointsPending;
            BlackPoints += BlackPointsPending;
            Turn = Turn.GetOpposite();
            ResetPendingPoints();
            //IsPlaying = false;
            if (!IsPlayPossible(Turn))
                GameState = GameStateEnum.GameOver;
            else if (_gameOptions.IsSinglePlayer && Turn==OthelloColor.Black)
            {
                GameState = GameStateEnum.WaitingDevice;
                Device.StartTimer(TimeSpan.FromMilliseconds(1000), DevicePlayCallback);
            }
            else
                GameState = GameStateEnum.WaitingPlayerPlay;

            PrintCells("Confirm.End");

        }

        public void UndoLastSequence()
        {
            var prevCells = new List<CellModel>();
            prevCells.AddRange(_playSet.Cells);
            var prevDiscs = new List<DiscModel>();
            _playSet.Reset();
            foreach (var cell in prevCells)
            {
                if(cell.IsPlaying)
                    MoveDiscFromCell(cell);
                else
                    FlipCellDisc(cell);
                cell.IsPending = false;
                cell.IsPlaying = false;
            }
            ResetPendingPoints();
            //Turn = Turn.GetOpposite();
            //IsPlaying = false;
            GameState = GameStateEnum.WaitingPlayerPlay;

            PrintCells("Undo.End");
        }


        bool DevicePlayCallback()
        {
            DevicePlay();
            DevicePlayed?.Invoke(this, new EventArgs());
            return false;
        }
        void DevicePlay()
        {
            var cellRatings = GetPlayableCells(Turn);

            var cr = cellRatings.OrderByDescending(r => r.Points).FirstOrDefault();

            PlayCell(cr.Cell);

            Device.StartTimer(TimeSpan.FromSeconds(3), DeviceConfirmCallBack);
        }


        bool DeviceConfirmCallBack()
        {
            Confirm();
            DeviceConfirmed?.Invoke(this, new EventArgs());
            return false;
        }



        bool IsPlayPossible(OthelloColor color)
        {
            foreach (var cell in _board.Where(c=>c.Disc==null))
            {
                var acs = GetAdjacentCells(cell);

                var starts = acs.Where(c => c.Disc != null && c.Disc.DiscColor == color.GetOpposite()).ToList();

                if (!starts.Any())
                    continue;

                var spokes = GetSpokes(cell, starts);
                if (spokes.Any())
                    return true;

            }
            return false;
        }

        List<CellRatingModel> GetPlayableCells(OthelloColor color)
        {
            // new class e.g. CellRating?
            var cells = new List<CellRatingModel>();

            foreach (var cell in _board.Where(c => c.Disc == null))
            {
                var acs = GetAdjacentCells(cell);

                var starts = acs.Where(c => c.Disc != null && c.Disc.DiscColor == color.GetOpposite()).ToList();

                if (!starts.Any())
                    continue;

                var spokes = GetSpokes(cell, starts);
                if (!spokes.Any())
                    continue;

                // get zone
                // get points - (length of spokes * 2) + 1 ?

                var points = spokes.Sum(s => s.Count * 2) + 1;

                var cr = new CellRatingModel() { Cell = cell, Points = points };

                cells.Add(cr);

            }
            return cells;

        }

        void FlipCellDisc(CellModel cell)
        {
            if (cell.Disc == null) return;
            FlipDisc(cell.Disc);
            _playSet.AddCell(cell);
            _playSet.AddDisc(cell.Disc);
        }

        void FlipDisc(DiscModel disc)
        {
            disc.Flip();
        }

        void MoveNextDiscToCell(OthelloColor color, CellModel cell)
        {
            MoveDiscToCell(GetNextDisc(color), color, cell);
        }

        void MoveDiscToCell(DiscModel disc, OthelloColor color, CellModel cell)
        {
            Debug.WriteLine($"MoveDiscToCell C={cell.Column} R={cell.Row} Disc={disc}");
            if (disc == null || cell == null)
                return;
            cell.Disc = disc;
            cell.Disc.DiscColor = color;
            disc.Cell = cell;
            _playSet.AddCell(cell);
            _playSet.AddDisc(disc);
        }

        void MoveDiscFromCell(CellModel cell)
        {
            Debug.WriteLine($"MoveDiscFromCell C={cell.Column} R={cell.Row}");
            if (cell.Disc == null)
                return;
            _playSet.AddDisc(cell.Disc);
            _playSet.AddCell(cell);
            cell.Disc.DiscColor = cell.Disc.InitialColor;
            cell.Disc.Cell = null;
            cell.Disc = null;
            cell.IsPending = false;
            cell.IsPlaying = false;
        }


        DiscModel GetNextDisc(OthelloColor color)
        {
            DiscModel disc = _discs.LastOrDefault(d => d.Cell == null && d.InitialColor == color);
            return disc;
        }

        void ResetPendingPoints()
        {
            WhitePointsPending = 0;
            BlackPointsPending = 0;
        }

        void IncrementPendingPoints(OthelloColor color)
        {
            AddPendingPoints(color, 1);
        }

        void DecrementPendingPoints(OthelloColor color)
        {
            AddPendingPoints(color, -1);
        }
        void AddPendingPoints(OthelloColor color, int points)
        {
            if (color == OthelloColor.White)
                WhitePointsPending += points;
            else
                BlackPointsPending += points;
        }


        CellModel GetCell(int column, int row)
        {
            return _board[8 * row + column];

        }

        List<CellModel> GetAdjacentCells(CellModel cell)
        {
            var cells = new List<CellModel>();

            for (int c = cell.Column - 1; c <= cell.Column + 1; c++)
            {
                for (int r = cell.Row-1; r <= cell.Row + 1; r++)
                {
                    if ((c != cell.Column || r != cell.Row) && c >= 0 && c < 8 && r >= 0 && r < 8)
                        cells.Add(GetCell(c, r));
                }
            }
            return cells;
        }

        List<List<CellModel>> GetSpokes(CellModel start, List<CellModel> cells)
        {
            var spokes = new List<List<CellModel>>();

            foreach (var cell in cells)
            {
                var spoke = new List<CellModel>();
                spoke.Add(cell);
                int rowIncrement = cell.Row - start.Row;
                int colIncrement = cell.Column - start.Column;
                int col = cell.Column;
                int row = cell.Row;
                bool isValidSpoke = false;
                while(true)
                {
                    col += colIncrement;
                    row += rowIncrement;
                    if (col >= 8 || row >= 8 || col<0 || row<0)
                        break;
                    var incCell = GetCell(col, row);
                    if (incCell.Disc == null)
                        break;
                    if (incCell.Disc.DiscColor==Turn)
                    {
                        isValidSpoke = true;
                        break;
                    }
                    spoke.Add(incCell);
                }
                if (isValidSpoke)
                    spokes.Add(spoke);
            }
            return spokes;
        }

        void PrintCells(string prefix)
        {
            foreach (var cell in _board)
            {
                if(cell.Disc!=null)
                {
                    Debug.WriteLine($"{prefix} - Cell {cell}");
                }
            }
        }


    }
}
