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
	public partial class TestView : ContentView
	{
		public TestView ()
		{
			InitializeComponent ();
		}

        async void Go(object sender, EventArgs args)
        {

            var x = this;

            white.AnchorY = 0.4;
            black.AnchorY = 0.6;
            double final = 0;
            if (white.RotationX == 0)
            {
                final = 180;
            }
            await Task.WhenAll(white.RotateXTo(90), black.RotateXTo(90));
            await Task.Delay(1000);
            await Task.WhenAll(white.RotateXTo(final), black.RotateXTo(final));

        }


    }
}