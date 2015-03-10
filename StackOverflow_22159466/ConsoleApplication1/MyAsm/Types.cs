namespace MyAsm
{
    public interface IA { }
    public interface IB { }
    public interface IC { }

    public class A : IA { }
    public class B : IB
    {
        private readonly IA _a;

        public B(IA a)
        {
            _a = a;
        }
    }

    public class C : IC
    {
        private readonly IB _b;

        public C(IB b)
        {
            _b = b;
        }
    }
}
