using System;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;
using MyAsm;

namespace ConsoleApplication1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();

            Method1(container);
            Method2(container);
        }

        private static void Method1(IUnityContainer container)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().
               SingleOrDefault(asm => asm.GetName().Name == "MyAsm");

            container.RegisterTypes(
                AllClasses.FromAssemblies(assembly),
                WithMappings.FromAllInterfacesInSameAssembly,
                WithName.Default,
                WithLifetime.Transient);
            
            var c = container.Resolve(typeof(IC));
        }

        private static void Method2(IUnityContainer container)
        {
            var assembly = Assembly.LoadFile(@"E:\TFS\Home\StackOverflow\StackOverflow_22159466\ConsoleApplication1\MyAsm\bin\Debug\MyAsm.dll");

            container.RegisterTypes(
                AllClasses.FromAssemblies(assembly),
                WithMappings.FromAllInterfacesInSameAssembly,
                WithName.Default,
                WithLifetime.Transient);

            var c = container.Resolve(assembly.GetType("MyAsm.IC"));
        }
    }
}