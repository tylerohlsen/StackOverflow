using System.IO.Ports;
using Microsoft.Practices.Unity;

namespace StackOverflow_21905504
{
    class Program
    {
        static void Main(string[] args)
        {
            //NamedServiceSelectorExtension.Register<MyParentTypeA, IMyChildType>("MyChildTypeA");
            //NamedServiceSelectorExtension.Register<MyParentTypeB, IMyChildType>("MyChildTypeB");

            IUnityContainer container = new UnityContainer();
            //container.AddNewExtension<NamedServiceSelectorExtension>();

            container.RegisterType<IMyChildType, MyChildTypeA>("MyChildTypeA");
            container.RegisterType<IMyChildType, MyChildTypeB>("MyChildTypeB");
            container.RegisterType<MyParentTypeA>(new RequiredInjectionConstructor(new ResolvedParameter<MyChildTypeA>()));
            container.RegisterType<MyParentTypeB>(new RequiredInjectionConstructor(new ResolvedParameter<MyChildTypeB>()));

            container.RegisterType<IOtherType, MyOtherClass>();

            
            var parentA1 = container.Resolve<MyParentTypeA>();
            var parentB1 = container.Resolve<MyParentTypeB>();
            var parentA2 = container.Resolve<MyParentTypeA>();
            var parentB2 = container.Resolve<MyParentTypeB>();

            parentA1.DoStuff();
            parentB1.DoStuff();
            parentA2.DoStuff();
            parentB2.DoStuff();

            //container.RegisterType<IMyChildType, MyChildType>();
            //var parentC = container.Resolve<MyParentTypeC>();
        }
    }


    interface IMyChildType
    {
        void DoStuff();
    }

    class MyChildTypeA : IMyChildType
    {
        public void DoStuff() { }
    }

    class MyChildTypeB : IMyChildType
    {
        public void DoStuff() { }
    }

    class MyChildType : IMyChildType
    {
        public void DoStuff() { }
    }


    interface IOtherType
    {
         
    }

    class MyOtherClass : IOtherType
    {
         
    }


    class MyParentTypeA
    {
        private readonly IMyChildType _childType;
        private readonly IOtherType _otherType;

        public MyParentTypeA(IMyChildType childType, IOtherType otherType)
        {
            _childType = childType;
            _otherType = otherType;
        }

        public void DoStuff()
        {
            _childType.DoStuff();
        }
    }

    class MyParentTypeB
    {
        private readonly IMyChildType _childType;

        public MyParentTypeB(IMyChildType childType)
        {
            _childType = childType;
        }

        public void DoStuff()
        {
            _childType.DoStuff();
        }
    }

    class MyParentTypeC
    {
        private readonly IMyChildType _childType;

        public MyParentTypeC(IMyChildType childType)
        {
            _childType = childType;
        }

        public void DoStuff()
        {
            _childType.DoStuff();
        }
    }
}
