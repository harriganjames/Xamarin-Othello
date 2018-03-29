using Othello.Main.Enum;

using Xamarin.Forms;

namespace Othello.Main.View
{
    public class DiscView : ContentView
        {

        public bool InUse
        {
            get { return (bool)GetValue(InUseProperty); }
            set { SetValue(InUseProperty, value); }
        }

        public static readonly BindableProperty InUseProperty =
            BindableProperty.Create("InUse", typeof(bool), typeof(DiscView), false);


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

        public static readonly BindableProperty IsVerticalProperty =
            BindableProperty.Create("IsVertical", typeof(bool), typeof(DiscView), false);

        public bool IsFlat
        {
            get { return (bool)GetValue(IsFlatProperty); }
            set { SetValue(IsFlatProperty, value); }
        }

        public static readonly BindableProperty IsFlatProperty =
            BindableProperty.Create("IsFlat", typeof(bool), typeof(DiscView), true);

    }
}