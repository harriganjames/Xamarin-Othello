using System;
using System.Collections.Generic;
using System.Text;

namespace Othello.Main.Model
{
    public class PlaySetModel
    {
        public PlaySetModel(List<CellTransitionModel> cellTransitions, bool isUndo)
        {
            CellTransitions = cellTransitions;
            IsUndo = isUndo;
        }

        public List<CellTransitionModel> CellTransitions { get; private set; }
        public bool IsUndo { get; private set; }
    }
}
