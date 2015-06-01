using Microsoft.Practices.Unity;

namespace StackOverflow_29929703
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityContainer();

            container
                .RegisterType<IConnectionInfo, ConnectionInfo>()
                .RegisterType<IUserInfo, UserInfo>();

            //container.RegisterType<IDatabaseManagement, DatabaseManagement>(new InjectionConstructor(
            //        container.Resolve<IConnectionInfo>().Connection,
            //        container.Resolve<IUserInfo>().CurrentUser.UserEmployeeNumber));

            container.RegisterType<IDatabaseManagement, DatabaseManagement>(
                new InjectionFactory(c =>
                {
                    var connectionInfo = c.Resolve<IConnectionInfo>();
                    var userInfo = c.Resolve<IUserInfo>();

                    return new DatabaseManagement(
                        connectionInfo.Connection,
                        userInfo.CurrentUser.UserEmployeeNumber);
                }));

            var dbManagement1 = container.Resolve<IDatabaseManagement>();
            var dbManagement2 = container.Resolve<IDatabaseManagement>();
        }
    }

    internal class DatabaseManagement : IDatabaseManagement
    {
        private readonly string _connection;
        private readonly string _employeeNumber;

        public DatabaseManagement(string connection, string employeeNumber)
        {
            _connection = connection;
            _employeeNumber = employeeNumber;
        }
    }

    internal interface IDatabaseManagement
    {
    }

    internal class Employee
    {
        public Employee()
        {
            UserEmployeeNumber = Counter.GetNext().ToString();
        }

        public string UserEmployeeNumber { get; private set; }
    }

    internal class UserInfo : IUserInfo
    {
        public UserInfo()
        {
            CurrentUser = new Employee();
        }

        public Employee CurrentUser { get; private set; }
    }

    internal interface IUserInfo
    {
        Employee CurrentUser { get; }
    }

    internal class ConnectionInfo : IConnectionInfo
    {
        public ConnectionInfo()
        {
            Connection = "Foo";
        }

        public string Connection { get; private set; }
    }

    internal interface IConnectionInfo
    {
        string Connection { get; }
    }

    internal static class Counter
    {
        private static int Current = 0;

        public static int GetNext()
        {
            return ++Current;
        }
    }
}
