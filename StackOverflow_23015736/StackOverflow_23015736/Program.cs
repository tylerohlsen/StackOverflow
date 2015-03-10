using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace StackOverflow_23015736
{
    class Program
    {
        static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();
            
            //There's other options for each parameter (and you can supply your own custom options)
            container.RegisterTypes(
                AllClasses.FromLoadedAssemblies().Where(type => typeof(IPrePopulationModule).IsAssignableFrom(type)),
                WithMappings.FromAllInterfacesInSameAssembly,
                WithName.TypeName,
                type => new PerHttpRequestLifetimeManager());
        }
    }

    public interface IPrePopulationModule
    {
        
    }

    public class PopulateGenders : IPrePopulationModule
    {
        
    }
}
