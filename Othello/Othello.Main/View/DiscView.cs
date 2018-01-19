using Othello.Main.Enum;

using Xamarin.Forms;

namespace Othello.Main.View
{
    public class DiscView : ContentView
        {
        public DiscView()
        {

        }



        public bool InUse
        {
            get { return (bool)GetValue(InUseProperty); }
            set { SetValue(InUseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InUse.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty InUseProperty =
            BindableProperty.Create("InUse", typeof(bool), typeof(DiscView), false);



        public CellStateEnum State
        {
            get { return (CellStateEnum)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly BindableProperty StateProperty =
            BindableProperty.Create("State", typeof(CellStateEnum), typeof(DiscView), CellStateEnum.Empty, propertyChanged: OnStatePropertyChanged);

        public CellStateEnum TransitionedState
        {
            get { return (CellStateEnum)GetValue(TransitionedStateProperty); }
            set { SetValue(TransitionedStateProperty, value); }
        }

        public static readonly BindableProperty TransitionedStateProperty =
            BindableProperty.Create("TransitionedState", typeof(CellStateEnum), typeof(DiscView), CellStateEnum.Empty);






        static async void OnStatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            //var Disc2View = (DiscView)bindable;
            //var oldState = (CellStateEnum)oldValue;
            //var newState = (CellStateEnum)newValue;

            //if (oldState != CellStateEnum.Empty)
            //{
            //    await Disc2View.RotateYTo(90);
            //    Disc2View.TransitionedState = newState;
            //    await Disc2View.RotateYTo(0);
            //}
            //else
            //{
            //    Disc2View.TransitionedState = newState;
            //}


        }

        //Color


        public OthelloColor InitialColor
        {
            get { return (OthelloColor)GetValue(InitialColorProperty); }
            set { SetValue(InitialColorProperty, value); }
        }

        public static readonly BindableProperty InitialColorProperty =
            BindableProperty.Create("InitialColor", typeof(OthelloColor), typeof(DiscView), OthelloColor.Black);

        public OthelloColor DiscColor
        {
            get { return (OthelloColor)GetValue(DiscColorProperty); }
            set { SetValue(DiscColorProperty, value); }
        }

        public static readonly BindableProperty DiscColorProperty =
            BindableProperty.Create("DiscColor", typeof(OthelloColor), typeof(DiscView), OthelloColor.Black);

        public OthelloColor ActualColor
        {
            get { return (OthelloColor)GetValue(ActualColorProperty); }
            set { SetValue(ActualColorProperty, value); }
        }

        public static readonly BindableProperty ActualColorProperty =
            BindableProperty.Create("ActualColor", typeof(OthelloColor), typeof(DiscView), OthelloColor.Black);

        public bool IsVertical
        {
            get { return (bool)GetValue(IsVerticalProperty); }
            set { SetValue(IsVerticalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsVertical.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty IsVerticalProperty =
            BindableProperty.Create("IsVertical", typeof(bool), typeof(DiscView), false);


    }
}