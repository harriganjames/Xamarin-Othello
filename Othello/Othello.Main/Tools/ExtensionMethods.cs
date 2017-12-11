using Othello.Main.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Othello.Main.Tools
{
    static class ExtensionMethods
    {
        static public bool IsOpposite(this CellStateEnum thisState, CellStateEnum state)
        {
            if (thisState == CellStateEnum.Black && state == CellStateEnum.White)
                return true;
            if (thisState == CellStateEnum.White && state == CellStateEnum.Black)
                return true;
            return false;
        }
        static public CellStateEnum GetOppositeState(this CellStateEnum thisState)
        {
            CellStateEnum state;
            if (thisState == CellStateEnum.Empty)
                state = CellStateEnum.White;
            else if (thisState == CellStateEnum.White)
                state = CellStateEnum.Black;
            else if (thisState == CellStateEnum.Black)
                state = CellStateEnum.White;
            else
                state = CellStateEnum.Empty;
            return state;
        }
    }
}
