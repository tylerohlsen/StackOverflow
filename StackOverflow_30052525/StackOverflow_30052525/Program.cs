using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace StackOverflow_30052525
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityContainer();

            container
                .RegisterType<ISomeClass1, SomeClass1>()
                .RegisterType<IProcessingService, ProcessingService>()
                //.RegisterType<IProcessingService, ProcessingService>(new InjectionFactory((c, type, name) =>
                //{
                //    return new ProcessingService(c.Resolve<ISomeClass1>(), new SomeClass2(type));
                //}))
                .RegisterType(typeof(IGenericSomeClass2<>), typeof(GenericSomeClass2<>));

            container.Resolve<IProcessingService>();
        }
    }

    internal class ProcessingService : IProcessingService
    {
        private readonly ISomeClass1 someClass1;
        private readonly ISomeClass2 someClass2;

        //public ProcessingService(ISomeClass1 someClass1, ISomeClass2 someClass2)
        //{
        //    this.someClass1 = someClass1;
        //    this.someClass2 = someClass2;
        //}

        public ProcessingService(ISomeClass1 someClass1, IGenericSomeClass2<ProcessingService> someClass2)
        {
            this.someClass1 = someClass1;
            this.someClass2 = someClass2;
        }
    }

    internal interface ISomeClass2
    {
    }

    class SomeClass2 : ISomeClass2
    {
        private readonly Type _declaringType;

        public SomeClass2(Type declaringType)
        {
            _declaringType = declaringType;
        }
    }

    internal interface ISomeClass1
    {
    }

    class SomeClass1 : ISomeClass1
    {
    }

    internal interface IProcessingService
    {
    }
    internal interface IGenericSomeClass2<T>: ISomeClass2 {}

    public class GenericSomeClass2<T>: IGenericSomeClass2<T>
    {
        private readonly ISomeClass2 someClass2;
        public GenericSomeClass2()
        {
            this.someClass2 = new SomeClass2(typeof(T));
        }
        // Pass-through implementation
    }
}
