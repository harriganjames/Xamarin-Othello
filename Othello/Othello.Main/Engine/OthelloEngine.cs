using Othello.Main.Enum;
using Othello.Main.Model;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Othello.Main.Tools;

namespace Othello.Main.Engine
{
    public class OthelloEngine
    {
        CellModel[] _board;
        List<CellTransitionModel> _lastSequence;


        public OthelloEngine()
        {
            _lastSequence = new List<CellTransitionModel>();
        }

        public void Initialize(IEnumerable<CellModel> cells)
        {
            _board = new CellModel[cells.Count()];
            foreach (var cell in cells)
            {
                _board[8 * cell.Row + cell.Column] = cell;
            }
        }

        public CellStateEnum Turn { get; private set; }

        public int WhitePoints { get; private set; }
        public int BlackPoints { get; private set; }
        public int WhitePointsPending { get; private set; }
        public int BlackPointsPending { get; private set; }

        public List<CellTransitionModel> Sequence => _lastSequence;

        public void NewGame()
        {
            _lastSequence.Clear();
            for (int column = 0; column < 8; column++)
            {
                for (int row = 0; row < 8; row++)
                {
                    SetCellState(column, row, CellStateEnum.Off);
                }
            }
            SetCellState(3, 3, CellStateEnum.White);
            SetCellState(3, 4, CellStateEnum.Black);
            SetCellState(4, 3, CellStateEnum.Black);
            SetCellState(4, 4, CellStateEnum.White);
            Turn = CellStateEnum.White;
            WhitePoints = 2;
            BlackPoints = 2;
            WhitePointsPending = 0;
            BlackPointsPending = 0;
        }

        public bool PlayCell(CellModel playCell)
        {
            if (playCell.State != CellStateEnum.Off)
                return false;

            var acs = GetAdjacentCells(playCell);

            var starts = acs.Where(c => c.State.IsOpposite(Turn)).ToList();

            if (!starts.Any())
                return false;

            var spokes = GetSpokes(playCell, starts);
            if (!spokes.Any())
                return false;

            _lastSequence.Clear();
            SetCellState(playCell, Turn);
            foreach (var spoke in spokes)
            {
                foreach (var cell in spoke)
                {
                    ToggleCell(cell);
                }
            }
            Turn = Turn.GetOppositeState();

            return true;
        }

        public void Confirm()
        {
            WhitePoints += WhitePointsPending;
            BlackPoints += BlackPointsPending;
            ResetPendingPoints();
        }

        public void UndoLastSequence()
        {
            foreach (var ct in _lastSequence)
            {
                ct.Cell.State = ct.PreviousCellState;
            }
            ResetPendingPoints();
            Turn = Turn.GetOppositeState();
        }

        void ResetPendingPoints()
        {
            WhitePointsPending = 0;
            BlackPointsPending = 0;
        }

        CellModel GetCell(int column, int row)
        {
            return _board[8 * row + column];

        }

        void SetCellState(int column, int row, CellStateEnum state)
        {
            var cell = _board[8 * row + column];
            SetCellState(cell, state);
        }

        void SetCellState(CellModel cell, CellStateEnum state)
        {
            if (cell.State == state)
                return;

            if (state == CellStateEnum.White)
                WhitePointsPending++;
            else
                BlackPointsPending++;

            if (cell.State==CellStateEnum.White)
                WhitePointsPending--;
            if (cell.State == CellStateEnum.Black)
                BlackPointsPending--;

            var previousState = cell.State;
            cell.State = state;
            _lastSequence.Add(new CellTransitionModel(cell,previousState));
        }

        void ToggleCell(CellModel cell)
        {
            SetCellState(cell, cell.State.GetOppositeState());
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
                    if (col >= 8 || row >= 8)
                        break;
                    var incCell = GetCell(col, row);
                    if (incCell.State==Turn)
                    {
                        isValidSpoke = true;
                        break;
                    }
                    if (incCell.State == CellStateEnum.Off)
                        break;
                    spoke.Add(incCell);
                }
                if (isValidSpoke)
                    spokes.Add(spoke);
            }
            return spokes;
        }

    }
}
