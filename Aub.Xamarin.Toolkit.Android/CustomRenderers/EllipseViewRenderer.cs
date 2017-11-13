using System.ComponentModel;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Aub.Xamarin.Toolkit.CustomViews;

[assembly: ExportRenderer(typeof(EllipseView),
                          typeof(Aub.Xamarin.Toolkit.Android.CustomRenderers.EllipseViewRenderer))]

namespace Aub.Xamarin.Toolkit.Android.CustomRenderers
{
    public class EllipseViewRenderer : ViewRenderer<EllipseView, EllipseDrawableView>
    {
        double width, height;

        protected override void OnElementChanged(ElementChangedEventArgs<EllipseView> args)
        {
            base.OnElementChanged(args);

            if (Control == null)
            {
                SetNativeControl(new EllipseDrawableView(Context));
            }

            if (args.NewElement != null)
            {
                SetColor();
                SetSize();
            }
        }

        protected override void OnElementPropertyChanged(object sender,
                                                         PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(sender, args);

            if (args.PropertyName == VisualElement.WidthProperty.PropertyName)
            {
                width = Element.Width;
                SetSize();
            }
            else if (args.PropertyName == VisualElement.HeightProperty.PropertyName)
            {
                height = Element.Height;
                SetSize();
            }
            else if (args.PropertyName == EllipseView.ColorProperty.PropertyName)
            {
                SetColor();
            }
        }

        void SetColor()
        {
            Control.SetColor(Element.Color);
        }

        void SetSize()
        {
            Control.SetSize(width, height);
        }
    }
}