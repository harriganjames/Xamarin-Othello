using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Othello.Main.Bootstrap;
using Autofac;
using Othello.Main.ViewModel;
using Othello.Main.Factories;

namespace Othello.Main
{
    public partial class App : Application
    {
        IContainer _container;

        public App(AppSetup setup)
        {
            InitializeComponent();

            _container = setup.CreateContainer();

            _container.BeginLifetimeScope();

            var page = new MainPage();
            var vm = _container.Resolve<MainViewModelFactory>().Create();
            page.BindingContext = vm;

            MainPage = page;
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
