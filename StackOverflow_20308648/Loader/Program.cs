using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Loader
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assemblyPath = Path.Combine(baseDirectory, "AssemblyWithAttributes.dll");

            ReadAttributes(assemblyPath);

            if (Debugger.IsAttached)
            {
                Console.WriteLine();
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
            }
        }

        private static void ReadAttributes(string assemblyPath)
        {
            Assembly assembly = Assembly.LoadFile(assemblyPath);
            //Assembly assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            Type type = assembly.GetType("AssemblyWithAttributes.Test");
            if (type != null)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (MethodInfo m in methods)
                {
                    Console.WriteLine("{0}.{1}()", m.DeclaringType, m.Name);
                    foreach (Attribute a in Attribute.GetCustomAttributes(m, false))
                    {
                        Console.WriteLine("\t{0}", a);
                        foreach (FieldInfo f in a.GetType().GetFields())
                        {
                            Console.WriteLine("\t\t{0}: {1}", f.Name, f.GetValue(a));
                        }
                    }
                }
            }
        }
    }
}
