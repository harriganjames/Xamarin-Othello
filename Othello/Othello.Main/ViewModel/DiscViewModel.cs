using Aub.Xamarin.Toolkit.ViewModel;
using Othello.Main.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Othello.Main.ViewModel
{
    public class DiscViewModel : ViewModelBase
    {
        public DiscViewModel()
        {

        }

        public void Initialize(DiscStateEnum state)
        {
            _state = state;
        }

        DiscStateEnum _state;
        public DiscStateEnum State
        {
            get { return _state; }
            set
            {
                SetValue(ref _state, value, () => State);
            }
        }

    }
}
