using Aub.Xamarin.Toolkit.Service;
using Autofac;
using Othello.Main.Engine;
using Othello.Main.Factories;
using Othello.Main.View;
using Othello.Main.ViewModel;
using System;

namespace Othello.Main.Bootstrap
{
    public class AppSetup
    {
        IContainer _container;
        public IContainer CreateContainer(Action<ContainerBuilder> registerAction)
        {
            var containerBuilder = new ContainerBuilder();
            RegisterDependencies(containerBuilder);
            registerAction?.Invoke(containerBuilder);
            _container = containerBuilder.Build();
            return _container;
        }

        protected virtual void RegisterDependencies(ContainerBuilder cb)
        {
            cb.RegisterType<MainViewModel>().SingleInstance();
            cb.RegisterType<MainViewModelFactory>().SingleInstance();
            cb.RegisterType<CellViewModel>();
            cb.RegisterType<CellViewModelFactory>().SingleInstance();
            cb.RegisterType<DiscViewModel>();
            cb.RegisterType<DiscViewModelFactory>().SingleInstance();
            cb.RegisterType<BoardViewModel>();
            cb.RegisterType<BoardViewModelFactory>().SingleInstance();
            cb.RegisterType<GameViewModel>();
            cb.RegisterType<GameViewModelFactory>().SingleInstance();
            cb.RegisterType<OthelloEngine>();
            cb.RegisterType<OthelloEngineFactory>().SingleInstance();

            cb.RegisterType<UserInterfaceService>().As<IUserInterfaceService>().SingleInstance();
            cb.RegisterType<PersistenceService>().As<IPersistenceService>().SingleInstance();
        }


        public void RegisterViewModelMappings()
        {
            var uis = _container.Resolve<IUserInterfaceService>();

            uis.RegisterViewModelToView<NewGameViewModel, NewGameView>();

        }

    }
}
