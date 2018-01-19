namespace Othello.Main.Enum
{
    public enum OthelloColor
    {
        None = 0,
        White = 1,
        Black = 2
    }

    public enum DiscStateEnum
    {
        Stacked=0,
        White=OthelloColor.White,
        Black=OthelloColor.Black
    }

    public enum CellStateEnum
    {
        Empty=0,
        White=DiscStateEnum.White,
        Black=DiscStateEnum.Black
    }

}
