using System;
using System.Collections.Generic;
using System.Text;

namespace Othello.Main.Enum
{
    public enum GameStateEnum
    {
        NotStarted,
        WaitingPlayerPlay,
        WaitingPlayerConfirm,
        WaitingDevice,
        Ended,
        GameOver
    }
}
