using Aub.Xamarin.Toolkit.ViewModel;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Aub.Xamarin.Toolkit.Service
{
    public interface IUserInterfaceService
    {
        Task<DialogResultEnum> ShowView(IViewModelDialog viewModel);
        void RegisterViewModelToView<TViewModel, TView>()
            where TView : ContentView;
    }
}
