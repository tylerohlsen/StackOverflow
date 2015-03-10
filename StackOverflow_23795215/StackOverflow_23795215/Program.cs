using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace StackOverflow_23795215
{
    class Program
    {
        static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer().LoadConfiguration();

            //container.RegisterType<IReader<Wrapper<Fund>>, WrappedFundReader>();
            string typeA = typeof (IReader<>).AssemblyQualifiedName;
            string typeB = typeof (Wrapper<Fund>).AssemblyQualifiedName;

            var fundReader = container.Resolve<IReader<Fund>>();
            var wrappedReader = container.Resolve<IReader<Wrapper<Fund>>>();
        }
    }

    public interface IReader<T> { }

    public class Fund { }

    public class FundReader : IReader<Fund> { }

    public class Wrapper<T> { }

    public class WrappedFundReader : IReader<Wrapper<Fund>> { }
}
