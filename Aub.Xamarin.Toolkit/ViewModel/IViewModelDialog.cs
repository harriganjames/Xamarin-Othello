using Aub.Xamarin.Toolkit.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aub.Xamarin.Toolkit.ViewModel
{
    public interface IViewModelDialog
    {
        Action<DialogResultEnum> RequestClose { set; }
        string PageTitle { get; }
    }
}
