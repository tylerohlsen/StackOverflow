using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.Unity.Utility;

namespace StackOverflow_21905504
{
    /// <summary>
    /// A class that holds the collection of minimum required
    /// parameters for a constructor, so that the container can
    /// be configured to call this constructor.
    /// </summary>
    public class RequiredInjectionConstructor : InjectionMember
    {
        private readonly List<InjectionParameterValue> _requiredParameterValues;

        /// <summary>
        /// Create a new instance of <see cref="RequiredInjectionConstructor"/> that looks
        /// for a constructor with a minimum of the given required set of parameters.
        /// </summary>
        /// <param name="requiredParameterValues">The values for the parameters, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.</param>
        public RequiredInjectionConstructor(params object[] requiredParameterValues)
        {
            _requiredParameterValues = InjectionParameterValue.ToParameters(requiredParameterValues).ToList();
        }

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the required parameter values.
        /// </summary>
        /// <param name="serviceType">Interface registered, ignored in this implementation.</param>
        /// <param name="implementationType">Type to register.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            ConstructorInfo ctor = FindConstructor(implementationType, _requiredParameterValues);
            IEnumerable<InjectionParameterValue> selectedConstructorParameterValues = GetSelectedConstructorParameterValues(ctor, _requiredParameterValues);

            policies.Set<IConstructorSelectorPolicy>(
                new SpecifiedConstructorSelectorPolicy(ctor, selectedConstructorParameterValues.ToArray()),
                new NamedTypeBuildKey(implementationType, name));
        }

        private static ConstructorInfo FindConstructor(Type typeToCreate, IEnumerable<InjectionParameterValue> requiredInjectionParameters)
        {
            var typeToCreateReflector = new ReflectionHelper(typeToCreate);

            var matchedConstructors = typeToCreateReflector.InstanceConstructors.
                Where(ctor =>
                {
                    var constructorParameterTypes = ctor.GetParameters().Select(info => info.ParameterType);
                    return requiredInjectionParameters.All(required => constructorParameterTypes.Any(required.MatchesType));
                });

            if (matchedConstructors.Any())
            {
                // Prefer the constructor that has the least number of arguments.
                // Other preference models could be implemented here. 
                return matchedConstructors.OrderBy(ctor =>
                    ctor.GetParameters().Count()).
                    FirstOrDefault();
            }

            string signature = string.Join(", ", requiredInjectionParameters.Select(required => required.ParameterTypeName).ToArray());

            throw new InvalidOperationException(
                string.Format("Unable to find a constructor with the minimum required parameters.  Type: {0}, RequiredParameters: {1}",
                    typeToCreate.FullName,
                    signature));
        }

        private static IEnumerable<InjectionParameterValue> GetSelectedConstructorParameterValues(ConstructorInfo ctor, IEnumerable<InjectionParameterValue> requiredInjectionParameters)
        {
            var injectionParameterValues = new List<InjectionParameterValue>();

            foreach (var parameter in ctor.GetParameters())
            {
                var existingInjectionParameter = requiredInjectionParameters.FirstOrDefault(required => required.MatchesType(parameter.ParameterType));
                injectionParameterValues.Add(existingInjectionParameter ?? new ResolvedParameter(parameter.ParameterType));
            }

            return injectionParameterValues;
        }
    }
}