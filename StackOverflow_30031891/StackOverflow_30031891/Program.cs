using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace StackOverflow_30031891
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityContainer();

            container.RegisterType<IImplementMe, FirstImplementation>("First");
            container.RegisterType<IImplementMe, SecondImplementation>("Second");
            container.RegisterType<IImplementMe, SecondImplementation>();

            var service = container.Resolve<Service>();
            service.Foo();
        }
    }

    public interface IImplementMe
    {
        void DoSomething();
    }
    public class FirstImplementation : IImplementMe
    {
        public void DoSomething()
        {
            Console.WriteLine("First");
        }
    }
    public class SecondImplementation : IImplementMe
    {
        public void DoSomething()
        {
            Console.WriteLine("Second");
        }
    }

    public class Service
    {
        private bool someCondition = true;

        private readonly Lazy<IImplementMe> _first;
        

        public Service(Lazy<IImplementMe> first)
        {
            _first = first;
        }

        //public Service(Func<Dictionary<string, IImplementMe>> myClassInstances)
        //{
        //    this.myClassInstances = myClassInstances;
        //}


        public void Foo()
        {
            if (someCondition)
            {
                _first.Value.DoSomething();
            }
            else
            {
                //myClassInstances.Invoke()["Second"].DoSomething();
            }
        }
    }
}
