using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace StackOverflow_21905504
{
    public class NamedServiceSelectorExtension : UnityContainerExtension
    {
        private static ICollection<NamedServiceRegistration> NamedServiceRegistrations { get; set; }

        /// <summary>
        /// Initializes the <see cref="NamedServiceSelectorExtension"/> class.
        /// </summary>
        static NamedServiceSelectorExtension()
        {
            NamedServiceRegistrations = new List<NamedServiceRegistration>();
        }

        /// <summary>
        /// Registers the specified service name.
        /// </summary>
        /// <typeparam name="TCaller">The type of the caller.</typeparam>
        /// <typeparam name="TDependency">The type of the dependency.</typeparam>
        /// <param name="serviceName">Name of the service.</param>
        public static void Register<TCaller, TDependency>(string serviceName)
        {
            Register(typeof (TCaller), typeof (TDependency), serviceName);
        }

        /// <summary>
        /// Registers the specified caller type.
        /// </summary>
        /// <param name="callerType">Type of the caller.</param>
        /// <param name="dependencyType">Type of the dependency.</param>
        /// <param name="serviceName">Name of the service.</param>
        public static void Register(Type callerType, Type dependencyType, string serviceName)
        {
            NamedServiceRegistrations.Add(new NamedServiceRegistration(callerType, dependencyType, serviceName));
        }

        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, this method will modify the given
        /// <see cref="T:Microsoft.Practices.Unity.ExtensionContext" /> by adding strategies, policies, etc. to
        /// install it's functions into the container.
        /// </remarks>
        protected override void Initialize()
        {
            Context.BuildPlanStrategies.AddNew<NamedServiceSelectorStrategy>(UnityBuildStage.PreCreation);
        }

        private class NamedServiceRegistration
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NamedServiceRegistration"/> class.
            /// </summary>
            /// <param name="callerType">Type of the caller.</param>
            /// <param name="dependencyType">Type of the dependency.</param>
            /// <param name="serviceName">Name of the service.</param>
            public NamedServiceRegistration(Type callerType, Type dependencyType, string serviceName)
            {
                CallerType = callerType;
                DependencyType = dependencyType;
                ServiceName = serviceName;
            }

            public Type CallerType { get; private set; }
            public Type DependencyType { get; private set; }
            public string ServiceName { get; private set; }
        }

        private class NamedServiceSelectorStrategy : BuilderStrategy
        {
            /// <summary>
            /// Called during the chain of responsibility for a build operation. The
            /// PreBuildUp method is called when the chain is being executed in the
            /// forward direction.
            /// </summary>
            /// <param name="context">Context of the build operation.</param>
            public override void PreBuildUp(IBuilderContext context)
            {
                IPolicyList constructorSelectorPolicyList;
                var selector = context.Policies.Get<IConstructorSelectorPolicy>(context.BuildKey, out constructorSelectorPolicyList);
                if (selector == null)
                    return;

                var selectedConstructor = selector.SelectConstructor(context, constructorSelectorPolicyList);
                if (selectedConstructor == null)
                    return;

                var callerType = selectedConstructor.Constructor.ReflectedType;
                var parameters = selectedConstructor.Constructor.GetParameters();
                var parameterKeys = selectedConstructor.GetParameterKeys();

                for (int i = 0; i < parameters.Length; i++)
                {
                    Type dependencyType = parameters[i].ParameterType;
                    string parameterKey = parameterKeys[i];

                    string serviceName = NamedServiceRegistrations.
                        Where(reg => reg.CallerType == callerType && reg.DependencyType == dependencyType).
                        Select(reg => reg.ServiceName).
                        FirstOrDefault();

                    if (serviceName == null)
                        continue;

                    IDependencyResolverPolicy resolverPolicy = new NamedTypeDependencyResolverPolicy(dependencyType, serviceName);
                    constructorSelectorPolicyList.Set(resolverPolicy, parameterKey);
                }

                context.Policies.Set<IConstructorSelectorPolicy>(new FixedConstructorSelectorPolicy(selectedConstructor), context.BuildKey);
            }
        }
        
        private class FixedConstructorSelectorPolicy : IConstructorSelectorPolicy
        {
            private readonly SelectedConstructor _selectedConstructor;

            /// <summary>
            /// Initializes a new instance of the <see cref="FixedConstructorSelectorPolicy"/> class.
            /// </summary>
            /// <param name="selectedConstructor">The selected constructor.</param>
            /// <exception cref="System.ArgumentNullException">selectedConstructor</exception>
            public FixedConstructorSelectorPolicy(SelectedConstructor selectedConstructor)
            {
                if (selectedConstructor == null)
                    throw new ArgumentNullException("selectedConstructor");

                _selectedConstructor = selectedConstructor;
            }

            /// <summary>
            /// Choose the constructor to call for the given type.
            /// </summary>
            /// <param name="context">Current build context</param>
            /// <param name="resolverPolicyDestination">The <see cref="T:Microsoft.Practices.ObjectBuilder2.IPolicyList" /> to add any
            /// generated resolver objects into.</param>
            /// <returns>
            /// The chosen constructor.
            /// </returns>
            public SelectedConstructor SelectConstructor(IBuilderContext context, IPolicyList resolverPolicyDestination)
            {
                return _selectedConstructor;
            }
        }
    }
}