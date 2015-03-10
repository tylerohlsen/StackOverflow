using System;
using System.Data.Entity;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.WebApi;

namespace StackOverflow_23743750
{
    class Program
    {
        static void Main(string[] args)
        {
            IUnityContainer rootContainer = new UnityContainer();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityHierarchicalDependencyResolver(rootContainer);
            GlobalConfiguration.Configuration.Filters.Add(new RegisterHttpRequestActionAttribute());

            rootContainer.RegisterType<MyController>();
            rootContainer.RegisterType<MyDbContext>(new HierarchicalLifetimeManager(), new InjectionFactory(container => 
                new MyDbContext(container.Resolve<HttpRequestMessage>().Properties["TenantDB"] as string)));

            HttpRequestMessage requestMessage = new HttpRequestMessage();
            requestMessage.Properties.Add("TenantDB", "FooDB");
            MyController controller = rootContainer.Resolve<MyController>();
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RegisterHttpRequestActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var container = GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IUnityContainer)) as IUnityContainer;
            container.RegisterInstance(actionContext.Request, new HierarchicalLifetimeManager());
        }
    }

    public class MyDbContext : DbContext
    {
        public string DatabaseName { get; set; }

        public MyDbContext(string databaseName) : base()
        {
            DatabaseName = databaseName;
        }
    }

    public class MyController : ApiController
    {
        public MyController(MyDbContext dbContext)
        {
            // dbContext.DatabaseName should be set!
        }
    }
}
