using Othello.Main.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Othello.Main.Model
{
    public class CellModel
    {
        public CellModel(int column, int row) : this(column, row, CellStateEnum.Off)
        {
        }

        public CellModel(int column, int row, CellStateEnum state)
        {
            Column = column;
            Row = row;
            State = state;
        }


        public int Row { get; private set; }
        public int Column { get; private set; }
        public CellStateEnum State { get; set; }

        public override string ToString()
        {
            return $"C={Column} R={Row} State={State.ToString()}";
        }
    }
}
