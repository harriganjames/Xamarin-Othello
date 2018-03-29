using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Aub.Xamarin.Toolkit.Control
{
    public class SelectedProxyView : ContentView
    {
        public SelectedProxyView()
        {
            this.BindingContextChanged += SelectedProxyView_BindingContextChanged;
        }

        private void SelectedProxyView_BindingContextChanged(object sender, EventArgs e)
        {
            var proxy = sender as SelectedProxyView;
            proxy.IsSelected = proxy.BindingContext == proxy.SelectedItem;
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create("IsSelected", typeof(bool), typeof(SelectedProxyView), false, defaultBindingMode:BindingMode.TwoWay);


        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create("SelectedItem", typeof(object), typeof(SelectedProxyView), null, defaultBindingMode: BindingMode.TwoWay, propertyChanged:OnSelectedItemChanged);

        static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var proxy = bindable as SelectedProxyView;
            proxy.IsSelected = bindable.BindingContext == newValue;
        }

    }
}
