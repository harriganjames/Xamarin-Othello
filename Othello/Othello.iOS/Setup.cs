using Othello.Main.Bootstrap;
using Autofac;

namespace Othello.iOS
{
    public class Setup : AppSetup
    {
        protected override void RegisterDependencies(ContainerBuilder cb)
        {
            base.RegisterDependencies(cb);

        }
    }
}