using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace StackOverflow_25693593
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = new UnityContainer();
            container.AddNewExtension<Interception>();

            container.RegisterType<InjectionPolicy, ReportAttributePolicy>(typeof (ReportAttributePolicy).AssemblyQualifiedName, new ContainerControlledLifetimeManager());

            var controllers = AllClasses.FromLoadedAssemblies().DerivedFrom<RootController>();

            container.RegisterTypes(
                controllers, 
                WithMappings.None, 
                WithName.Default, 
                WithLifetime.Transient);

            container.EnableInterception(controllers);

            var yup = container.Resolve<MyController>();
            yup.Action();
            int val = yup.Property;
            yup.Property = 1;
        }
    }

    public static class InterceptionExtensions
    {
        public static IUnityContainer EnableInterception<T>(this IUnityContainer container)
        {
            container.EnableInterception(typeof (T));
            return container;
        }

        public static IUnityContainer EnableInterception(this IUnityContainer container, Type type)
        {
            if (type.IsInterface)
                container.Configure<Interception>().SetInterceptorFor(type, new InterfaceInterceptor());
            else
                container.Configure<Interception>().SetInterceptorFor(type, new VirtualMethodInterceptor());
            return container;
        }

        public static IUnityContainer EnableInterception(this IUnityContainer container, IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                container.EnableInterception(type);
            }
            return container;
        }
    }

    public static class EnumerableTypesExtensions
    {
        public static IEnumerable<Type> DerivedFrom<T>(this IEnumerable<Type> types)
        {
            return types.Where(t => typeof (T).IsAssignableFrom(t) && typeof (T) != t);
        }
    }

    public abstract class RootController
    {
        public abstract int Property { [Report] get; [Report] set; }

        [Report]
        public abstract void Action();
    }

    public class MyController : RootController
    {
        private int _property;

        public override int Property
        {
            get
            {
                Console.WriteLine("MyController.get_Property");
                return _property;
            }
            set
            {
                _property = value;
                Console.WriteLine("MyController.set_Property");
            }
        }

        public override void Action()
        {
            Console.WriteLine("MyController.Action()");
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = true)]
    public class ReportAttribute : Attribute
    {
    }

    public class ReportAttributePolicy : InjectionPolicy
    {
        protected override bool DoesMatch(MethodImplementationInfo member)
        {
            bool match = false;
            if (member.InterfaceMethodInfo != null)
                match = ReflectionHelper.GetAllAttributes<ReportAttribute>(member.InterfaceMethodInfo, true).Length > 0;
            if (!match)
                match = ReflectionHelper.GetAllAttributes<ReportAttribute>(member.ImplementationMethodInfo, true).Length > 0;
            return match;
        }

        protected override IEnumerable<ICallHandler> DoGetHandlersFor(MethodImplementationInfo member, IUnityContainer container)
        {
            if (!DoesMatch(member))
                return new ICallHandler[0];

            return new[] {new ReportCallHandler()};
        }
    }

    public class ReportCallHandler : ICallHandler
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            Console.WriteLine("Before: {0} on type: {1}", input.MethodBase.Name, input.MethodBase.ReflectedType.Name);
            IMethodReturn returnValue = getNext()(input, getNext);
            Console.WriteLine("After:  {0} on type: {1}", input.MethodBase.Name, input.MethodBase.ReflectedType.Name);
            return returnValue;
        }

        public int Order { get; set; }
    }
}