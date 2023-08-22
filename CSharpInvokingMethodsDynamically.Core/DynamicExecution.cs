namespace CSharpInvokingMethodsDynamically.Core;

using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public static class DynamicExecution
{
    private const string CLASS_NAME = "DynamicMethodClass";
    private const string ASSEMBLY_NAME = "DynamicMethodAssembly";

    private static readonly string codeTemplate =
        "using System; \n" +
        "public class {0} {{\n" +
        "\t{1}\n" +
        "}}";

    public static List<string> ExtractParametersFromMethod(string? methodBody)
    {
        List<string> parameters = new List<string>();

        if (methodBody == null)
        {
            return parameters;
        }

        // Extract parameters using regular expression
        Match match = Regex.Match(methodBody, @"\((.*?)\)");
        if (match.Success)
        {
            string paramString = match.Groups[1].Value;

            if (string.IsNullOrEmpty(paramString))
            {
                return parameters;
            }

            string[] paramArray = paramString.Split(',');

            foreach (var param in paramArray)
            {
                string trimmedParam = param.Trim();
                string[] paramParts = trimmedParam.Split(' ');
                if (paramParts.Length >= 2)
                {
                    string paramType = paramParts[0];
                    string paramName = paramParts[1];
                    string paramWithType = $"{paramType} {paramName}";
                    parameters.Add(paramWithType);
                }
                else if (paramParts.Length == 1)
                {
                    // If the parameter has no type specified (likely variable name only)
                    parameters.Add(paramParts[0]);
                }
            }
        }

        return parameters;
    }

    public static TypeCode ConvertParameterTypeFromString(string typeString)
    {
        switch (typeString.ToLower())
        {
            case "boolean":
                return TypeCode.Boolean;
            case "byte":
                return TypeCode.Byte;
            case "int16":
                return TypeCode.Int16;
            case "int":
                return TypeCode.Int32;
            case "int32":
                return TypeCode.Int32;
            case "int64":
                return TypeCode.Int64;
            case "single":
                return TypeCode.Single;
            case "double":
                return TypeCode.Double;
            case "char":
                return TypeCode.Char;
            case "string":
                return TypeCode.String;
            // Add more cases for other type strings...

            default:
                return TypeCode.Object; // Default to Object if the type string is unknown.
        }
    }

    public static object? ExecuteMethod(string? methodBody, 
        string[]? parameterNames, string[]? parameterValues, TypeCode[]? parameterTypes)
    {
        ValidateMethod(methodBody, parameterNames, parameterValues, parameterTypes);

        string code = string.Format(codeTemplate, CLASS_NAME, methodBody);

        Assembly? assembly = CreateDynamicAssembly(code);

        // Get the dynamic Class type and the corresponding dynamic method.
        Type? dynamicMethodClassType = assembly.GetType(CLASS_NAME);
        MethodInfo? dynamicMethod = dynamicMethodClassType?.GetMethods().FirstOrDefault();

        return InvokeMethod(parameterNames, parameterValues, parameterTypes, dynamicMethod);
    }

    private static void ValidateMethod(string? methodBody, string[]? parameterNames, string[]? parameterValues, TypeCode[]? parameterTypes)
    {
        if (string.IsNullOrEmpty(methodBody))
        {
            throw new ArgumentException("Missing method body.");
        }

        if (parameterNames != null && parameterValues != null && parameterTypes != null
            && parameterNames.Length != parameterValues.Length && parameterValues.Length != parameterTypes.Length)
        {
            throw new ArgumentException("Mismatch between parameter names-values-types.");
        }

        if (parameterNames != null && parameterNames.Count(p => string.IsNullOrEmpty(p.Trim())) == parameterNames.Length)
        {
            throw new ArgumentException("Some parameter names are empty.");
        }

        if (parameterValues != null && parameterValues.Count(p => string.IsNullOrEmpty(p.ToString().Trim())) == parameterValues.Length)
        {
            throw new ArgumentException("Some parameter values are empty.");
        }

        if (parameterTypes != null && parameterTypes.Count(p => string.IsNullOrEmpty(p.ToString().Trim())) == parameterTypes.Length)
        {
            throw new ArgumentException("Some parameter types are empty.");
        }
    }

    /// <summary>
    /// Compiles the code and creates a dynamic assembly.
    /// </summary>
    /// <param name="code">code to compile and load.</param>
    /// <returns>Assembly loaded.</returns>
    private static Assembly CreateDynamicAssembly(string code)
    {
        Assembly? assembly = null;

        using (var memoryStream = new MemoryStream())
        {
            CompileMethod(code, memoryStream);
            assembly = Assembly.Load(memoryStream.ToArray());
        }

        if (assembly == null)
        {
            throw new Exception("Dynamic assembly was not loaded properly.");
        }

        return assembly;
    }

    private static void CompileMethod(string code, MemoryStream memoryStream)
    {
        var compilation = CSharpCompilation
                    .Create(ASSEMBLY_NAME)
                    .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                    .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                    .AddSyntaxTrees(CSharpSyntaxTree.ParseText(code));

        var compilationResult = compilation.Emit(memoryStream);
        if (!compilationResult.Success)
        {
            string errorMessages = string.Empty;
            foreach (var error in compilationResult.Diagnostics)
            {
                errorMessages += error.GetMessage().ToString() + "\n";
            }
            throw new InvalidOperationException($"Compilation failed:\n{errorMessages}");
        }
    }

    /// <summary>
    /// Allows to execute the method with or without parameters.
    /// </summary>
    /// <param name="parameterNames"></param>
    /// <param name="parameterValues"></param>
    /// <param name="parameterTypes"></param>
    /// <param name="dynamicMethod"></param>
    /// <returns></returns>
    private static object? InvokeMethod(string[]? parameterNames, string[]? parameterValues, TypeCode[]? parameterTypes, MethodInfo? dynamicMethod)
    {
        if (parameterNames == null && parameterValues == null && parameterTypes == null)
        {
            return dynamicMethod?.Invoke(null, null);
        }
        else
        {
            // Convert parameter values to appropriate types
            object[] parsedParameters = new object[parameterValues.Length];
            for (int i = 0; i < parameterValues.Length; i++)
            {
                parsedParameters[i] = Convert.ChangeType(parameterValues[i], parameterTypes[i]);
            }

            return dynamicMethod?.Invoke(null, parsedParameters);
        }
    }
}