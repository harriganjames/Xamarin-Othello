using Othello.Main.Enum;

using Xamarin.Forms;

namespace Othello.Main.View
{
    public class Disc2View : ContentView
    {
        public Disc2View()
        {

        }



        public bool InUse
        {
            get { return (bool)GetValue(InUseProperty); }
            set { SetValue(InUseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InUse.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty InUseProperty =
            BindableProperty.Create("InUse", typeof(bool), typeof(Disc2View), false);



        public CellStateEnum State
        {
            get { return (CellStateEnum)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly BindableProperty StateProperty =
            BindableProperty.Create("State", typeof(CellStateEnum), typeof(Disc2View), CellStateEnum.Empty, propertyChanged: OnStatePropertyChanged);

        public CellStateEnum TransitionedState
        {
            get { return (CellStateEnum)GetValue(TransitionedStateProperty); }
            set { SetValue(TransitionedStateProperty, value); }
        }

        public static readonly BindableProperty TransitionedStateProperty =
            BindableProperty.Create("TransitionedState", typeof(CellStateEnum), typeof(Disc2View), CellStateEnum.Empty);


        static async void OnStatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var Disc2View = (Disc2View)bindable;
            var oldState = (CellStateEnum)oldValue;
            var newState = (CellStateEnum)newValue;

            if (oldState != CellStateEnum.Empty)
            {
                await Disc2View.RotateYTo(90);
                Disc2View.TransitionedState = newState;
                await Disc2View.RotateYTo(0);
            }
            else
            {
                Disc2View.TransitionedState = newState;
            }


        }



    }
}