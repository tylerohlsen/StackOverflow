using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace StackOverflow_27371346
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = new UnityContainer();

            // Option 1
            container.RegisterTypes(
                AllClasses.FromLoadedAssemblies(),
                WithMappings.FromAllInterfaces,
                WithName.Default,
                WithLifetime.Transient,
                type =>
                {
                    // If settings type, load the setting
                    if (!type.IsAbstract && typeof (ISettings).IsAssignableFrom(type))
                    {
                        return new[]
                        {
                            new InjectionFactory((c, t, n) =>
                            {
                                var svc = (ISettings) c.Resolve(t);
                                return svc.LoadSetting(t);
                            })
                        };
                    }

                    // Otherwise, no special consideration is needed
                    return new InjectionMember[0];
                });

            // Option 2
            container.RegisterTypes(
                AllClasses.FromLoadedAssemblies().IsSetting(),
                WithMappings.FromAllInterfaces,
                WithName.Default,
                WithLifetime.Transient,
                SettingsRegistration.InjectionMembers);

            // Option 3
            container.RegisterTypes(new SettingsRegistrationConvention(AllClasses.FromLoadedAssemblies()));
        }
    }

    public static class SettingsRegistration
    {
        public static IEnumerable<Type> IsSetting(this IEnumerable<Type> types)
        {
            return types.Where(type => !type.IsAbstract && typeof (ISettings).IsAssignableFrom(type));
        }

        public static IEnumerable<InjectionMember> InjectionMembers(Type type)
        {
            return new[] {new InjectionFactory(LoadSetting)};
        }

        public static ISettings LoadSetting(IUnityContainer container, Type type, string name)
        {
            var svc = (ISettings) container.Resolve(type, name);
            return svc.LoadSetting(type);
        }
    }

    public class SettingsRegistrationConvention : RegistrationConvention
    {
        private readonly IEnumerable<Type> _scanTypes;

        public SettingsRegistrationConvention(IEnumerable<Type> scanTypes)
        {
            if (scanTypes == null)
                throw new ArgumentNullException("scanTypes");

            _scanTypes = scanTypes;
        }

        public override IEnumerable<Type> GetTypes()
        {
            return _scanTypes.Where(type => !type.IsAbstract && typeof (ISettings).IsAssignableFrom(type));
        }

        public override Func<Type, IEnumerable<Type>> GetFromTypes()
        {
            return WithMappings.FromAllInterfaces;
        }

        public override Func<Type, string> GetName()
        {
            return WithName.Default;
        }

        public override Func<Type, LifetimeManager> GetLifetimeManager()
        {
            return WithLifetime.Transient;
        }

        public override Func<Type, IEnumerable<InjectionMember>> GetInjectionMembers()
        {
            return type => new[]
            {
                new InjectionFactory((c, t, n) =>
                {
                    var svc = (ISettings) c.Resolve(t);
                    return svc.LoadSetting(t);
                })
            };
        }
    }

    /*
        public class SettingsRegistration : IRegistrationConvention
        {
            public void Process(Type type, Registry registry)
            {
                if (!type.IsAbstract && typeof(ISettings).IsAssignableFrom(type))
                {
                    registry.For(type).Use(x =>
                    {
                        var svc = x.GetInstance<ISettingService>();
                        return svc.LoadSetting(type);
                    });
                }
            }
        }
        */
    }

    public interface ISettings
    {
        ISettings LoadSetting(Type type);
    }
}
