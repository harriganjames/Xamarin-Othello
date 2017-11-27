namespace Othello.Main.Enum
{
    public enum DiscStateEnum
    {
        White=1,
        Black=2
    }

    public enum CellStateEnum
    {
        Off=0,
        White=DiscStateEnum.White,
        Black=DiscStateEnum.Black
    }

}
