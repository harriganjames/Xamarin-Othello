using Aub.Xamarin.Toolkit.ViewModel;
using Othello.Main.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Othello.Main.ViewModel
{
    public class PlayerModeViewModel : ViewModelBase
    {
        public PlayerModeViewModel()
        {

        }

        public PlayerModeViewModel Initialize(PlayerModeEnum mode, string caption)
        {
            PlayerMode = mode;
            Caption = caption;
            return this;
        }

        public PlayerModeEnum PlayerMode { get; private set; }

        public string Caption { get; private set; }

    }
}
