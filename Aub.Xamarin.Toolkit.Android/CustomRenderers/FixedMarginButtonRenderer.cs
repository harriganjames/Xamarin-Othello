using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Button), typeof(Aub.Xamarin.Toolkit.Android.CustomRenderers.FixedMarginButtonRenderer))]

namespace Aub.Xamarin.Toolkit.Android.CustomRenderers
{
    public class FixedMarginButtonRenderer : ButtonRenderer
    {
        public FixedMarginButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // remove default background image
                //Control.SetPadding(0, 0, 0, 0);
                //Control.Background = null; // removes the default border and drop-shadow?
                //Control.SetBackgroundColor(Element.BackgroundColor.ToAndroid());
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "BackgroundColor")
            {
                // You have to set background color here again, because the background color can be changed later.
                //Control.SetBackgroundColor(Element.BackgroundColor.ToAndroid());
            }
        }
    }
}