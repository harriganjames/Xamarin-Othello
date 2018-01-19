using Othello.Main.Enum;

using Xamarin.Forms;

namespace Othello.Main.View
{
    public class CellView : ContentView
    {

        public bool IsPending
        {
            get { return (bool)GetValue(IsPendingProperty); }
            set { SetValue(IsPendingProperty, value); }
        }

        public static readonly BindableProperty IsPendingProperty =
            BindableProperty.Create("IsPending", typeof(bool), typeof(CellView), false);

        public bool IsPlaying
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }

        public static readonly BindableProperty IsPlayingProperty =
            BindableProperty.Create("IsPlaying", typeof(bool), typeof(CellView), false);


        public object Disc
        {
            get { return (object)GetValue(DiscProperty); }
            set { SetValue(DiscProperty, value); }
        }

        public static readonly BindableProperty DiscProperty =
            BindableProperty.Create("Disc", typeof(object), typeof(CellView), null);

    }
}