using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace StackOverflow_24205327
{
    class Program
    {
        static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();



            container.RegisterType<IVpModuleComposition, VpModuleComposition1>("VpModuleComposition1");
            container.RegisterType<IVpModuleComposition, VpModuleComposition2>("VpModuleComposition2");
            container.RegisterType<IVpModuleComposition, VpModuleComposition3>("VpModuleComposition3");
            container.RegisterType<IVpModuleComposition, VpModuleComposition4>("VpModuleComposition4");

            var registrations = container.Registrations.Where(reg => reg.RegisteredType == typeof(IVpModuleComposition));

            var moduleCompositions = new List<IVpModuleComposition>();

            registrations.ForEach(reg =>
                moduleCompositions.Add(container.Resolve<IVpModuleComposition>(reg.Name)));

            moduleCompositions.AddRange(container.ResolveAll<IVpModuleComposition>());

            var test = container.ResolveAll<IVpModuleComposition>();



            //container.RegisterType<BaseToolRepository, ToolRepository>();
            //container.RegisterType<BaseGenericRepository<ToolServiceClient, Tool>, ToolRepository>();
            //container.RegisterType<IGenericRepository<Tool>, ToolRepository>();

            container.RegisterTypes(
                AllClasses.FromLoadedAssemblies(),
                WithCustomMappings.AggregateMappings(WithMappings.FromAllInterfaces, WithCustomMappings.FromAllAbstractBaseClasses),
                WithName.Default,
                WithLifetime.PerResolve);

            container.Resolve<ToolRepository>();
            container.Resolve<BaseToolRepository>();
            container.Resolve<BaseGenericRepository<ToolServiceClient, Tool>>();
            container.Resolve<IGenericRepository<Tool>>();
        }
    }


    public interface IVpModuleComposition { }
    public class VpModuleComposition1 : IVpModuleComposition { }
    public class VpModuleComposition2 : IVpModuleComposition { }
    public class VpModuleComposition3 : IVpModuleComposition { }
    public class VpModuleComposition4 : IVpModuleComposition { }





    public interface IGenericRepository<T> { }
    public abstract class BaseGenericRepository<TA, TB> : IGenericRepository<TB> { }
    public abstract class BaseToolRepository : BaseGenericRepository<ToolServiceClient, Tool> { }
    public class ToolRepository : BaseToolRepository { }
    
    public class Tool { }
    public class ToolServiceClient { }


    public static class WithCustomMappings
    {
        public static Func<Type, IEnumerable<Type>> AggregateMappings(params Func<Type, IEnumerable<Type>>[] mappings)
        {
            return type =>
            {
                var mappedTypes = new List<Type>();
                foreach (var mapping in mappings)
                    mappedTypes.AddRange(mapping(type));
                return mappedTypes.Distinct();
            };
        }

        public static Func<Type, IEnumerable<Type>> FromAllAbstractBaseClasses 
        {
            get
            {
                return type =>
                {
                    var abstractBaseTypes = new List<Type>();
                    var baseType = type.BaseType;
                    while (baseType != null)
                    {
                        if (baseType.IsAbstract)
                            abstractBaseTypes.Add(baseType);
                        baseType = baseType.BaseType;
                    }
                    return abstractBaseTypes;
                };
            }
        }
    }
}
