namespace CSharpInvokingMethodsDynamically.Core;

using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

/// <summary>
/// Library that compiles and executes dynamically c# primitive methods from a string input.
/// </summary>
public static class DynamicMethodExecution
{
    #region constants
    /// <summary>
    /// name for the class to create dynamically.
    /// </summary>
    private const string CLASS_NAME = "DynamicMethodClass";

    /// <summary>
    /// Name for the dynamic assembly to create.
    /// </summary>
    private const string ASSEMBLY_NAME = "DynamicMethodAssembly";
    #endregion

    #region private properties
    /// <summary>
    /// Structure of the format code to set.
    /// </summary>
    private static readonly string codeTemplate =
        "using System; \n" +
        "using System.Text; \n" +
        "public class {0} {{\n" + // We can add more namespaces.
        "\t{1}\n" +
        "}}";
    #endregion

    #region public methods
    /// <summary>
    /// Gets the parameter lists that matches from the input string method.
    /// </summary>
    /// <param name="methodBody">Input c# string method.</param>
    /// <returns>parameters with format {type name} if any found.</returns>
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

    /// <summary>
    /// Maps a type string parameter to a c# type.
    /// </summary>
    /// <param name="typeString">Parameter type as string. For instance int or string.</param>
    /// <returns>Parameter type mapped.</returns>
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

    /// <summary>
    /// Allows to invoke a c# definition method.
    /// </summary>
    /// <param name="methodBody">Input c# string method.</param>
    /// <param name="parameterNames">List of the method parameters names.</param>
    /// <param name="parameterValues">List of the method parameter values.</param>
    /// <param name="parameterTypes">List of the method parameter types.</param>
    /// <returns>Output of the method if was executed successfully.</returns>
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
    #endregion

    #region private methods
    /// <summary>
    /// Validates if the inputs to invoke the c# method are correct.
    /// </summary>
    /// <param name="methodBody">Input c# string method.</param>
    /// <param name="parameterNames">List of the method parameters names.</param>
    /// <param name="parameterValues">List of the method parameter values.</param>
    /// <param name="parameterTypes">List of the method parameter types.</param>
    /// <exception cref="ArgumentException"></exception>
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
    /// Compiles the complete c# code and creates a dynamic assembly.
    /// </summary>
    /// <param name="code">Complete c# code to compile and load.</param>
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

    /// <summary>
    /// Compiles the complete c# code.
    /// </summary>
    /// <param name="code">Complete c# code to compile.</param>
    /// <param name="memoryStream">stream in which to compile the code.</param>
    /// <exception cref="InvalidOperationException"></exception>
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
    /// Allows to execute the c# method with or without parameters.
    /// </summary>
    /// <param name="parameterNames">List of the method parameters names.</param>
    /// <param name="parameterValues">List of the method parameter values.</param>
    /// <param name="parameterTypes">List of the method parameter types.</param>
    /// <param name="dynamicMethod">memory object that contains the method to invoke.</param>
    /// <returns>Output of the method if was invoked successfully.</returns>
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
    #endregion
}