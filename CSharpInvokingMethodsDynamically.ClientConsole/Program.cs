using System.Reflection;
using System.Text.RegularExpressions;
using CSharpInvokingMethodsDynamically.Core;

namespace CSharpInvokingMethodsDynamically.ClientConsole;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter C# method body:");
        string? methodBody = Console.ReadLine();

        List<string> parameters = DynamicExecution.GetMethodParameters(methodBody);

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
                parameterTypes.Add(DynamicExecution.GetTypeCodeFromString(paramSplitted.First()));
            }
        }

        //Console.WriteLine("Enter parameter names (comma-separated):");
        //string paramNames = Console.ReadLine();
        //string[]? parameterNames = null;
        //if (!string.IsNullOrEmpty(paramNames?.Trim()))
        //{
        //    parameterNames = paramNames?.Split(',');
        //}

        //Console.WriteLine("Enter parameter values (comma-separated):");
        //string? paramValues = Console.ReadLine();
        //if (!string.IsNullOrEmpty(paramValues?.Trim()))
        //{
        //    parameterValues = paramValues?.Split(',');
        //}

        //Console.WriteLine("Enter parameter type (comma-separated):");
        //string? paramTypes = Console.ReadLine();
        //if (!string.IsNullOrEmpty(paramTypes?.Trim()))
        //{
        //    parameterTypes = new List<TypeCode>();
        //    foreach (var type in paramTypes.Split(','))
        //    {
        //        parameterTypes.Add(GetTypeCodeFromString(type));
        //    }
        //}

        try
        {
            // Compile and execute the method with parameters
            object? result = DynamicExecution.ExecuteMethod(methodBody, parameterNames?.ToArray(), parameterValues?.ToArray(), parameterTypes?.ToArray());

            Console.WriteLine("Result: " + result);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        //Console.WriteLine("Enter parameter names (comma-separated):");
        //string paramNames = Console.ReadLine();
        //string[]? parameterNames = null;
        //if (!string.IsNullOrEmpty(paramNames?.Trim()))
        //{
        //    parameterNames = paramNames?.Split(',');
        //}

        //Console.WriteLine("Enter parameter values (comma-separated):");
        //string? paramValues = Console.ReadLine();
        //string[]? parameterValues = null;
        //if (!string.IsNullOrEmpty(paramValues?.Trim()))
        //{
        //    parameterValues = paramValues?.Split(',');
        //}

        //Console.WriteLine("Enter parameter type (comma-separated):");
        //string? paramTypes = Console.ReadLine();
        //List<TypeCode>? parameterTypes = null;
        //if (!string.IsNullOrEmpty(paramTypes?.Trim()))
        //{
        //    parameterTypes = new List<TypeCode>();
        //    foreach (var type in paramTypes.Split(','))
        //    {
        //        parameterTypes.Add(GetTypeCodeFromString(type));
        //    }
        //}

        //try
        //{
        //    // Compile and execute the method with parameters
        //    object? result = DynamicExecution.ExecuteMethod(methodBody, parameterNames, parameterValues, parameterTypes?.ToArray());

        //    Console.WriteLine("Result: " + result);
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine("Error: " + ex.Message);
        //}

        Console.ReadLine(); // Pause before exiting
    }

    

    //static List<string> GetMethodParameters(string methodBody)
    //{
    //    List<string> parameters = new List<string>();

    //    // Extract parameters using regular expression
    //    Match match = Regex.Match(methodBody, @"\((.*?)\)");
    //    if (match.Success)
    //    {
    //        string paramString = match.Groups[1].Value;
    //        string[] paramArray = paramString.Split(',');

    //        foreach (var param in paramArray)
    //        {
    //            string trimmedParam = param.Trim();
    //            parameters.Add(trimmedParam);
    //        }
    //    }

    //    return parameters;
    //}

    
}
