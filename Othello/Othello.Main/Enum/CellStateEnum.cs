namespace Othello.Main.Enum
{
    public enum DiscColor
    {
        White = 1,
        Black = 2
    }

    public enum DiscStateEnum
    {
        Stacked=0,
        White=DiscColor.White,
        Black=DiscColor.Black
    }

    public enum CellStateEnum
    {
        Empty=0,
        White=DiscStateEnum.White,
        Black=DiscStateEnum.Black
    }

}
