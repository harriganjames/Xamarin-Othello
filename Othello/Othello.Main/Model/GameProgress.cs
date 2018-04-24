using Othello.Main.Enum;
using System.Collections.Generic;

namespace Othello.Main.Model
{
    public class GameProgress
    {
        public OthelloColor Turn { get; set; }
        public List<GameProgress.Cell> Cells { get; set; }
        public GameOptions GameOptions { get; set; }
        public int WhitePoints { get; set; }
        public int BlackPoints { get; set; }

        public class Cell
        {
            public int Row { get; set; }
            public int Column { get; set; }
            public OthelloColor DiscColor { get; set; }
        }
    }
}
