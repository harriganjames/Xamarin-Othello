using System;
using System.Collections.Generic;
using System.Text;

namespace Othello.Main.Model
{
    public class PlaySetModel
    {
        public PlaySetModel()
        {
            Cells = new List<CellModel>();
            Discs = new List<DiscModel>();
        }

        public List<CellModel> Cells { get; private set; }
        public List<DiscModel> Discs { get; private set; }

        public void Reset()
        {
            Cells.Clear();
            Discs.Clear();
            //IsUndo = false;
        }
    }
}
