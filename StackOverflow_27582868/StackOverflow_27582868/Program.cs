using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace StackOverflow_27582868
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityContainer();
            container.AddExtension(new Interception());


            container.RegisterType<ICustomSerialDevice, CustomSerialDevice>(
               new ContainerControlledLifetimeManager(), new InjectionMethod("postConstructMethodName"));

            container.Configure<Interception>()
                     .SetInterceptorFor<ICustomSerialDevice>(new InterfaceInterceptor());



            var device = container.Resolve<ICustomSerialDevice>();

            device.Connect();
            device.ExecuteMyCommand();
            device.ExecuteProtocolCommand();
            device.WriteCommand();
        }
    }


    public interface IDevice
    {
        void Connect();
    }

    public interface ISerialDevice : IDevice
    {
        void WriteCommand();
    }

    public interface IProtocolSerialDevice : ISerialDevice
    {
        void ExecuteProtocolCommand();
    }

    public interface ICustomSerialDevice : IProtocolSerialDevice
    {
        void ExecuteMyCommand();
    }

    public abstract class AbstractSerialDevice : ISerialDevice
    {
        public virtual void Connect()
        {
            //omitted
        }

        public virtual void WriteCommand()
        {
            //omitted
        }
    }

    public abstract class AbstractProtocolSerialDevice : AbstractSerialDevice, IProtocolSerialDevice
    {
        public virtual void ExecuteProtocolCommand()
        {
            //omitted
        }
    }

    public class CustomSerialDevice : AbstractProtocolSerialDevice, ICustomSerialDevice
    {
        [MyHandler]
        public override void Connect()
        { base.Connect(); }

        [MyHandler]
        public override void WriteCommand()
        { base.WriteCommand(); }

        [MyHandler]
        public override void ExecuteProtocolCommand()
        { base.ExecuteProtocolCommand(); }

        [MyHandler]
        public void ExecuteMyCommand()
        { /*omitted*/ }

        public void postConstructMethodName()
        { /*omitted*/ }
    }

    //[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class MyHandlerAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return container.Resolve<MyCallHandler>();
        }
    }

    public class MyCallHandler : ICallHandler
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            Console.WriteLine("Intercepted " + input.MethodBase.Name);
            return getNext()(input, getNext);
        }

        public int Order { get; set; }
    }
}
