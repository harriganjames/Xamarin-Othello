using Othello.Main.Enum;

namespace Othello.Main.Model
{
    public class DiscModel
    {
        public DiscModel(DiscColor color)
        {
            DiscColor = color;
        }

        public bool InUse { get; set; }
        public DiscColor DiscColor { get; set; }

    }
}
