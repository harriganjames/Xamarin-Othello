﻿namespace Aub.Xamarin.Toolkit.ViewModel
{
    public class BooleanResultEventArgs
    {
        public BooleanResultEventArgs(bool result)
        {
            Result = result;
        }
        public bool Result { get; set; }
    }
}
