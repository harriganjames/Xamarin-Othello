using Othello.Main.Enum;
using Othello.Main.Model;
using System.Linq;
using System.Collections.Generic;
using Othello.Main.Tools;

namespace Othello.Main.Engine
{
    public class OthelloEngine
    {
        CellModel[] _board;
        List<DiscModel> _discs;
        PlaySetModel _playSet;

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
        }

        public OthelloColor Turn { get; private set; }

        public int WhitePoints { get; private set; }
        public int BlackPoints { get; private set; }
        public int WhitePointsPending { get; private set; }
        public int BlackPointsPending { get; private set; }
        public bool IsPlaying { get; private set; }

        public PlaySetModel PlaySet => _playSet;

        public IEnumerable<CellModel> Cells => _board;
        public IEnumerable<DiscModel> Discs => _discs;

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


        public void NewGame()
        {
            MoveNextDiscToCell(OthelloColor.White, GetCell(3, 3));
            MoveNextDiscToCell(OthelloColor.Black, GetCell(3, 4));
            MoveNextDiscToCell(OthelloColor.Black, GetCell(4, 3));
            MoveNextDiscToCell(OthelloColor.White, GetCell(4, 4));

            Turn = OthelloColor.White;
            WhitePoints = 2;
            BlackPoints = 2;
            WhitePointsPending = 0;
            BlackPointsPending = 0;
        }

        public bool PlayCell(CellModel playCell)
        {
            if (playCell.Disc!=null)
                return false;

            if (IsPlaying)
                return false;

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
            Turn = Turn.GetOpposite();
            IsPlaying = true;
            return true;
        }

        public void Confirm()
        {
            foreach (var cell in _playSet.Cells)
            {
                cell.IsPending = false;
                cell.IsPlaying = false;
            }
            WhitePoints += WhitePointsPending;
            BlackPoints += BlackPointsPending;
            ResetPendingPoints();
            IsPlaying = false;
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
            Turn = Turn.GetOpposite();
            IsPlaying = false;
        }

        void FlipCellDisc(CellModel cell)
        {
            if (cell.Disc == null) return;
            FlipDisc(cell.Disc);
            _playSet.Cells.Add(cell);
            _playSet.Discs.Add(cell.Disc);
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
            if (disc == null || cell == null)
                return;
            cell.Disc = disc;
            cell.Disc.DiscColor = color;
            disc.Cell = cell;
            _playSet.Cells.Add(cell);
            _playSet.Discs.Add(disc);
        }

        void MoveDiscFromCell(CellModel cell)
        {
            if (cell.Disc == null)
                return;
            _playSet.Discs.Add(cell.Disc);
            _playSet.Cells.Add(cell);
            cell.Disc.DiscColor = cell.Disc.InitialColor;
            cell.Disc.Cell = null;
            cell.Disc = null;
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
                    if (incCell.State == CellStateEnum.Empty)
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

    }
}
