using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace StackOverflow_29480123
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityContainer();
            container.AddExtension(new Interception());

            container.RegisterTypes(
                AllClasses.FromLoadedAssemblies().WithMatchingInterface(),
                WithMappings.FromMatchingInterface,
                WithName.Default,
                WithLifetime.ContainerControlled,
                getInjectionMembers: t => new InjectionMember[]
                {
                    new Interceptor<InterfaceInterceptor>(),
                    new InterceptionBehavior<PolicyInjectionBehavior>(),
                });

            var controller = container.Resolve<MyController>();
        }
    }

    public static class TypeFilters
    {
        public static IEnumerable<Type> WithMatchingInterface(this IEnumerable<Type> types)
        {
            return types.Where(type => type.GetTypeInfo().GetInterface("I" + type.Name) != null);
        }
    }

    public interface IFoo
    {
        int GetData();
    }

    public class Foo : IFoo
    {
        public int GetData()
        {
            return 1;
        }
    }

    public class MyController
    {
        [Dependency]
        public IFoo Foo { get; set; }
    }
}
