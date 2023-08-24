// Ignore Spelling: Splitted

using CSharpInvokingMethodsDynamically.Core;

namespace CSharpInvokingMethodsDynamically.ClientConsole;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter C# method body:");
        string? methodBody = Console.ReadLine();

        List<string> parameters = DynamicExecution.ExtractParametersFromMethod(methodBody);

        List<string>? parameterNames = null;
        List<string>? parameterValues = null;
        List<TypeCode>? parameterTypes = null;

        if (parameters.Any())
        {
            parameterNames = new List<string>();
            parameterValues = new List<string>();
            parameterTypes = new List<TypeCode>();

            foreach (string parameter in parameters)
            {
                Console.WriteLine($"Enter parameter value for parameter: {parameter}");
                parameterValues.Add(Console.ReadLine());
                var paramSplitted = parameter?.Split(' ');
                parameterNames.Add(paramSplitted.Last());
                parameterTypes.Add(DynamicExecution.ConvertParameterTypeFromString(paramSplitted.First()));
            }
        }

        try
        {
            // Compile and execute the method with parameters
            object? result = DynamicExecution.ExecuteMethod(methodBody,
                parameterNames?.ToArray(), parameterValues?.ToArray(), parameterTypes?.ToArray());

            Console.WriteLine("Result: " + result);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        Console.ReadLine();
    }
}
