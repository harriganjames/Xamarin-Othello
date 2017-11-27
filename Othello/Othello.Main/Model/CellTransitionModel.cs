using Othello.Main.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Othello.Main.Model
{
    public class CellTransitionModel
    {
        public CellTransitionModel(CellModel cell, CellStateEnum previousState)
        {
            Cell = cell;
            PreviousCellState = previousState;
        }

        public CellStateEnum PreviousCellState { get; private set; }
        public CellModel Cell { get; private set; }
    }
}
