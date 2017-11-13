using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Othello
{
    class Bootstrap
    {
        public Bootstrap()
        {

        }

        public Page Initialize()
        {
            //var container = new WindsorContainer();

            //// allow collection injection
            //container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));

            //// disable automatic property injection
            //container.Kernel.ComponentModelBuilder.RemoveContributor(
            //    container.Kernel.ComponentModelBuilder.Contributors.OfType<PropertiesDependenciesModelInspector>().Single());

            //container.Kernel.AddFacility<TypedFactoryFacility>();

            //container.Register(Component.For<Window>().Instance(w));
            //container.Register(Component.For<Dispatcher>().UsingFactoryMethod(() => w.Dispatcher));


            //container.Install(FromAssembly.InDirectory(
            //new AssemblyFilter(Path.GetDirectoryName(
            //    Assembly. .GetExecutingAssembly().Location))));



            var page = new MainPage();
            page.BindingContext = null;
            return page;
        }

    }
}
