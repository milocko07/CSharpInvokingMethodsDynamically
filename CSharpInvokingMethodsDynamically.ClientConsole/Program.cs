using System.Reflection;
using CSharpInvokingMethodsDynamically.Core;

namespace CSharpInvokingMethodsDynamically.ClientConsole;

class Program
{
    static void Main(string[] args)
    {
        //object convertedValue = Convert.ChangeType("4", TypeCode.Int32);
        //Sum(convertedValue);
        Console.WriteLine("Enter C# method body:");
        string? methodBody = Console.ReadLine();

        Console.WriteLine("Enter parameter names (comma-separated):");
        string paramNames = Console.ReadLine();
        string[]? parameterNames = null;
        if (!string.IsNullOrEmpty(paramNames?.Trim()))
        {
            parameterNames = paramNames?.Split(',');
        }

        Console.WriteLine("Enter parameter values (comma-separated):");
        string? paramValues = Console.ReadLine();
        string[]? parameterValues = null;
        if (!string.IsNullOrEmpty(paramValues?.Trim()))
        {
            parameterValues = paramValues?.Split(',');
        }

        Console.WriteLine("Enter parameter type (comma-separated):");
        string? paramTypes = Console.ReadLine();
        List<TypeCode>? parameterTypes = null;
        if (!string.IsNullOrEmpty(paramTypes?.Trim()))
        {
            parameterTypes = new List<TypeCode>();
            foreach (var type in paramTypes.Split(','))
            {
                parameterTypes.Add(GetTypeCodeFromString(type));
            }
        }

        try
        {
            // Compile and execute the method with parameters
            object? result = DynamicExecution.ExecuteMethod(methodBody, parameterNames, parameterValues, parameterTypes?.ToArray());

            Console.WriteLine("Result: " + result);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }

        Console.ReadLine(); // Pause before exiting
    }

    static TypeCode GetTypeCodeFromString(string typeString)
    {
        switch (typeString)
        {
            case "Boolean":
                return TypeCode.Boolean;
            case "Byte":
                return TypeCode.Byte;
            case "Int16":
                return TypeCode.Int16;
            case "Int32":
                return TypeCode.Int32;
            case "Int64":
                return TypeCode.Int64;
            case "Single":
                return TypeCode.Single;
            case "Double":
                return TypeCode.Double;
            case "Char":
                return TypeCode.Char;
            case "String":
                return TypeCode.String;
            // Add more cases for other type strings...

            default:
                return TypeCode.Object; // Default to Object if the type string is unknown.
        }
    }
}
