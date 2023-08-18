using System.Reflection;
using CSharpInvokingMethodsDynamically.Core;

namespace CSharpInvokingMethodsDynamically.ClientConsole;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter C# method body:");
        string methodBody = Console.ReadLine();

        Console.WriteLine("Enter parameter names (comma-separated):");
        string[] parameterNames = Console.ReadLine().Split(',');

        Console.WriteLine("Enter parameter values (comma-separated):");
        string[] parameterValues = Console.ReadLine().Split(',');

        try
        {
            // Compile and execute the method with parameters
            object result = DynamicExecution.ExecuteMethod(methodBody, parameterNames, parameterValues);

            Console.WriteLine("Result: " + result);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        Console.ReadLine(); // Pause before exiting
    }
}
