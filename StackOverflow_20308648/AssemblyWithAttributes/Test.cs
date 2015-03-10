using System;

namespace AssemblyWithAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Author : Attribute
    {
        public string name;
        public double version;

        public Author(string name)
        {
            this.name = name;
            version = 1.0;
        }
    }

    public class Test
    {
        private string name = "";

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        [Author("Andrew")]
        public void Message(string mess)
        {
            Console.WriteLine(mess);
        }

        [Author("Andrew")]
        public void End()
        {
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }

        public double Power(double num, int pow)
        {
            return Math.Pow(num, pow);
        }
    }
}
