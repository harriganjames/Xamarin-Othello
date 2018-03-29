using Aub.Xamarin.Toolkit.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Aub.Xamarin.Toolkit.Service
{
    public class UserInterfaceService : IUserInterfaceService
    {
        readonly Page _mainPage;

        Dictionary<Type, Type> _mappings = new Dictionary<Type, Type>();

        public UserInterfaceService(Page page)
        {
            _mainPage = page;
        }

        public void RegisterViewModelToView<TViewModel,TView>()
            where TView : ContentView
        {
            _mappings.Add(typeof(TViewModel), typeof(TView));
        }

        public async Task<DialogResultEnum> ShowView(IViewModelDialog viewModel)
        {
            var dialogResult = DialogResultEnum.NotSet;
            var taskCompletionSource = new TaskCompletionSource<object>();
            var page = new ContentPage();
            page.BindingContext = viewModel;
            page.Title = viewModel.PageTitle;
            ContentView contentView;
            Type viewType;
            if(_mappings.TryGetValue(viewModel.GetType(), out viewType))
            {
                contentView = Activator.CreateInstance(viewType) as ContentView;
                if(contentView!=null)
                {
                    page.Content = contentView;
                }
            }

            viewModel.RequestClose = async (result) => {
                dialogResult = result;
                await _mainPage.Navigation.PopAsync();
            };

            page.Disappearing += (s,e) => {
                if (dialogResult == DialogResultEnum.NotSet)
                    dialogResult = DialogResultEnum.Back;
                taskCompletionSource.SetResult(null);
            };

            await _mainPage.Navigation.PushAsync(page);

            await taskCompletionSource.Task;

            return dialogResult;
        }

    }
}
