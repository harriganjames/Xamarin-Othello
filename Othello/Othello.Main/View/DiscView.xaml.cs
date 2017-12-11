﻿using Othello.Main.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

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
            BindableProperty.Create("State", typeof(CellStateEnum), typeof(DiscView), CellStateEnum.Empty, propertyChanged:OnStatePropertyChanged);

        public CellStateEnum TransitionedState
        {
            get { return (CellStateEnum)GetValue(TransitionedStateProperty); }
            set { SetValue(TransitionedStateProperty, value); }
        }

        public static readonly BindableProperty TransitionedStateProperty =
            BindableProperty.Create("TransitionedState", typeof(CellStateEnum), typeof(DiscView), CellStateEnum.Empty);


        static async void OnStatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var discView = (DiscView)bindable;
            var oldState = (CellStateEnum)oldValue;
            var newState = (CellStateEnum)newValue;

            if(oldState!=CellStateEnum.Empty)
            {
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