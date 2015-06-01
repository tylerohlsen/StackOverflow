using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceStuff;
using Microsoft.Practices.Unity;

namespace StackOverflow_30166690
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityContainer();

            container.RegisterTypes(AllClasses.FromLoadedAssemblies(),
                WithMappings.FromMatchingInterface,
                WithName.Default,
                WithLifetime.Transient);

            var controller =  container.Resolve<EmployeeController>();
        }
    }

    public class EmployeeController
    {
        private IEmployeeDataService dataService;

        public EmployeeController(IEmployeeDataService dataService)
        {
            this.dataService = dataService;
        }
    }
}
