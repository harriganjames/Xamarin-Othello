using Autofac;
using Othello.Main.Engine;
using Othello.Main.Factories;
using Othello.Main.ViewModel;

namespace Othello.Main.Bootstrap
{
    public class AppSetup
    {
        public IContainer CreateContainer()
        {
            var containerBuilder = new ContainerBuilder();
            RegisterDependencies(containerBuilder);
            return containerBuilder.Build();
        }

        protected virtual void RegisterDependencies(ContainerBuilder cb)
        {
            cb.RegisterType<MainViewModel>().SingleInstance();
            cb.RegisterType<MainViewModelFactory>().SingleInstance();
            cb.RegisterType<CellViewModel>();
            cb.RegisterType<CellViewModelFactory>().SingleInstance();
            cb.RegisterType<BoardViewModel>();
            cb.RegisterType<BoardViewModelFactory>().SingleInstance();
            cb.RegisterType<GameViewModel>();
            cb.RegisterType<GameViewModelFactory>().SingleInstance();
            cb.RegisterType<OthelloEngine>();
            cb.RegisterType<OthelloEngineFactory>().SingleInstance();
        }
    }
}
