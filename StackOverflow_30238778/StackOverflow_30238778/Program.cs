using Microsoft.Practices.Unity;

namespace StackOverflow_30238778
{
    class Program
    {
        static void Main(string[] args)
        {
            var _unityContainer = new UnityContainer();


            var token = new SecurityToken();
            token.Token = "Test";
            token.Value = "Test";

            var passport = new PassportContext();
            passport.Permissions = new SecurityPermissions { Add = true, Change = true, Inquiry = true, Delete = true };

            _unityContainer = new UnityContainer();
            _unityContainer.RegisterInstance<PassportContext>(passport, new ContainerControlledLifetimeManager());
            _unityContainer.RegisterInstance<SecurityToken>(token, new ContainerControlledLifetimeManager());
            _unityContainer.RegisterType<IWorksheetRepository, WorksheetRepository>();

            //_unityContainer.RegisterType<PassportContext>(new ContainerControlledLifetimeManager());
            //_unityContainer.RegisterType<SecurityToken>(new ContainerControlledLifetimeManager());

            var repository = _unityContainer.Resolve<IWorksheetRepository>();
        }
    }

    public class SecurityToken
    {
        public string Token { get; set; }
        public string Value { get; set; }
    }

    public class PassportContext
    {
        public SecurityPermissions Permissions { get; set; }
    }

    public class SecurityPermissions
    {
        public bool Add { get; set; }
        public bool Change { get; set; }
        public bool Inquiry { get; set; }
        public bool Delete { get; set; }
    }

    public sealed class WorksheetRepository : IWorksheetRepository
    {
        private PassportContext _passportContext;
        private SecurityToken _token;

        public WorksheetRepository(PassportContext passportContext, SecurityToken token)
        {
            _passportContext = passportContext;
            _token = token;
        }
    }

    public interface IWorksheetRepository
    {
    }
}
