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
	public partial class MainPageView : ContentPage
	{
		public MainPageView()
		{
			InitializeComponent();
		}

        async void NewGameClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new NewGamePageView());
        }


	}
}