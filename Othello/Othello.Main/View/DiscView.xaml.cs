using Othello.Main.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Othello.Main.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DiscView : ContentView
	{
		public DiscView()
		{
			InitializeComponent();
		}

        public CellStateEnum State
        {
            get { return (CellStateEnum)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly BindableProperty StateProperty =
            BindableProperty.Create("State", typeof(CellStateEnum), typeof(DiscView), CellStateEnum.Off, propertyChanged:OnStatePropertyChanged);

        public CellStateEnum TransitionedState
        {
            get { return (CellStateEnum)GetValue(TransitionedStateProperty); }
            set { SetValue(TransitionedStateProperty, value); }
        }

        public static readonly BindableProperty TransitionedStateProperty =
            BindableProperty.Create("TransitionedState", typeof(CellStateEnum), typeof(DiscView), CellStateEnum.Off);


        static async void OnStatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var discView = (DiscView)bindable;
            var oldState = (CellStateEnum)oldValue;
            var newState = (CellStateEnum)newValue;
            bool flip = false;

            if(oldState!=CellStateEnum.Off)
            {
                flip = true;

                await discView.RotateYTo(90);
                discView.TransitionedState = newState;
                await discView.RotateYTo(0);
            }
            else
            {
                discView.TransitionedState = newState;
            }


        }



    }
}