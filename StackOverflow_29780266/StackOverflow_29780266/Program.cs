using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace StackOverflow_29780266
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var unityContainer = new UnityContainer();

            unityContainer
                .AddExtension(new Interception())
                .Configure<Interception>()
                .AddPolicy("Foo")
                .AddMatchingRule<FooMatchingRule>()
                .AddCallHandler<FooCallHandler>();

            unityContainer.RegisterType<IRoleAuthorizationService, RoleAuthorizationService>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<PolicyInjectionBehavior>());

            unityContainer.RegisterType<IUserAuthorizationBC, UserAuthorizationBC>(
                new Interceptor<VirtualMethodInterceptor>(),
                new InterceptionBehavior<PolicyInjectionBehavior>());

            unityContainer.RegisterType<IDataContractFactory, DefaultDataContractFactory>(
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<PolicyInjectionBehavior>());

            var roleAuthSvc = unityContainer.Resolve<IRoleAuthorizationService>();
            roleAuthSvc.DoStuff();
        }
    }

    public interface IUserAuthorizationBC
    {
        void DoStuff();
    }

    public class UserAuthorizationBC : IUserAuthorizationBC
    {
        public virtual void DoStuff() { }
    }

    public class RoleAuthorizationService : RoleAuthorizationServiceBase
    {
        private readonly IDataContractFactory _factory;
        private readonly UserAuthorizationBC _businessProcessor;

        public RoleAuthorizationService(IDataContractFactory factory, UserAuthorizationBC businessProcessor)
            : base(factory, businessProcessor)
        {
            _factory = factory;
            _businessProcessor = businessProcessor;
        }

        public override void DoStuff()
        {
            _factory.DoStuff();
            _businessProcessor.DoStuff();
        }
    }

    public abstract class RoleAuthorizationServiceBase : IRoleAuthorizationService
    {
        protected RoleAuthorizationServiceBase(IDataContractFactory factory, UserAuthorizationBC businessProcessor)
        {}

        public abstract void DoStuff();
    }

    public interface IRoleAuthorizationService
    {
        void DoStuff();
    }

    public interface IDataContractFactory
    {
        void DoStuff();
    }

    public class DefaultDataContractFactory : IDataContractFactory
    {
        public virtual void DoStuff() { }
    }

    public class FooCallHandler : ICallHandler 
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            return getNext()(input, getNext);
        }

        public int Order { get; set; }
    }

    public class FooMatchingRule : IMatchingRule
    {
        public bool Matches(MethodBase member)
        {
            return true;
        }
    }
}
