using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Othello.Main.Bootstrap;
using Autofac;
using Othello.Main.ViewModel;
using Othello.Main.Factories;
using Othello.Main.View;

namespace Othello.Main
{
    public partial class App : Application
    {
        IContainer _container;

        public App(AppSetup setup)
        {
            InitializeComponent();

            var page = new MainPageView();
            var navPage = new NavigationPage(page);

            _container = setup.CreateContainer(cb => {
                cb.RegisterInstance(navPage as Page);
                cb.RegisterInstance(this as Application);
            });

            _container.BeginLifetimeScope();

            setup.RegisterViewModelMappings();

            var vm = _container.Resolve<MainViewModelFactory>().Create();
            page.BindingContext = vm;

            MainPage = navPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
