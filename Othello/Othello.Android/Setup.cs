using Autofac;
using Othello.Main.Bootstrap;

namespace Othello.Droid
{
    public class Setup : AppSetup
    {
        protected override void RegisterDependencies(ContainerBuilder cb)
        {
            base.RegisterDependencies(cb);

        }
    }
}