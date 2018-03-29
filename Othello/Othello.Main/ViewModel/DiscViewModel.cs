using Aub.Xamarin.Toolkit.ViewModel;
using Othello.Main.Enum;
using Othello.Main.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Othello.Main.ViewModel
{
    public class DiscViewModel : ViewModelBase
    {
        DiscModel _discModel;

        public void Initialize(DiscModel disc)
        {
            _discModel = disc;
        }

        bool _inUse;
        public bool InUse
        {
            get { return _inUse; }
            set { SetValue(ref _inUse, value, () => InUse); }
        }

        OthelloColor _discColor;
        public OthelloColor DiscColor
        {
            get { return _discColor; }
            set { SetValue(ref _discColor, value, () => DiscColor); }
        }

        public OthelloColor InitialColor => _discModel.InitialColor;

        public override string ToString()
        {
            return _discModel.ToString();
        }

    }
}
