using Othello.Main.Enum;
using Othello.Main.Tools;

namespace Othello.Main.Model
{
    public class DiscModel
    {
        public DiscModel(OthelloColor color)
        {
            InitialColor = color;
            DiscColor = color;
        }

        public bool InUse { get; set; }
        public OthelloColor DiscColor { get; set; }
        public OthelloColor InitialColor { get; private set; }

        public CellModel Cell { get; set; }

        public void Flip()
        {
            DiscColor = DiscColor.GetOpposite();
        }

        public override string ToString()
        {
            return $"Color={DiscColor.ToString()}";
        }
    }
}
