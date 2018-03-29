using Aub.Xamarin.Toolkit.ViewModel;
using Othello.Main.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Othello.Main.Model
{
    public class CellModel : NotifyBase
    {
        public CellModel(int column, int row)
        {
            Column = column;
            Row = row;
        }


        public int Row { get; private set; }
        public int Column { get; private set; }
        //public CellStateEnum State
        //{
        //    get
        //    {
        //        return Disc == null ? CellStateEnum.Empty : (CellStateEnum)Disc.DiscColor;
        //    }
        //}
        public DiscModel Disc { get; set; }
        public bool IsPending { get; set; }

        private bool _isPlaying;

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                NotifyPropertyChanged();
            }
        }


        public override string ToString()
        {
            var d = Disc == null ? "empty" : Disc.ToString();
            return $"C={Column} R={Row} Disc={d}";
        }
    }
}
