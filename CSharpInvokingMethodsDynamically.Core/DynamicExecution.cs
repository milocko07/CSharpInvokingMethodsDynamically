namespace CSharpInvokingMethodsDynamically.Core;

using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public static class DynamicExecution
{
    private const string CLASS_NAME = "DynamicMethodClass";
    private static readonly string template = "using System; \n" +
        "public class {0} {{\n" +
        "\t{1}\n" +
        "}}";
    //$"public static object DynamicMethod(object {parameters}) {{ {methodBody} }}
    //"{0}" +
    //"} }";

    public static List<string> GetMethodParameters(string? methodBody)
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

    public static TypeCode GetTypeCodeFromString(string typeString)
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

    public static object? ExecuteMethod(string? methodBody, string[]? parameterNames, string[]? parameterValues, TypeCode[]? parameterTypes)
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

        string code = string.Format(template, CLASS_NAME, methodBody);
        
        // Compile the code and create an assembly
        var compilation = CSharpCompilation
            .Create("DynamicMethodAssembly")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            //.AddReferences(AppDomain.CurrentDomain.GetAssemblies())
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(CSharpSyntaxTree.ParseText(code));

        Assembly? assembly = null;
        using (var memoryStream = new MemoryStream())
        {
            var compilationResult = compilation.Emit(memoryStream);
            if (!compilationResult.Success)
            {
                throw new InvalidOperationException("Compilation failed.");
            }

            assembly = Assembly.Load(memoryStream.ToArray());
        }
       
        if (assembly == null)
        {
            throw new Exception("Dynamic assembly was not loaded properly.");
        }

        // Get the DynamicMehodClass type and the correspond dynamicMethod method.
        Type? dynamicMethodClassType = assembly.GetType(CLASS_NAME);
        MethodInfo? dynamicMethod = dynamicMethodClassType?.GetMethods().FirstOrDefault();

        // Invoke the method.
        if (parameterNames == null && parameterValues == null)
        {
            return dynamicMethod?.Invoke(null, null);
        }
        else
        {
            // Convert parameter values to appropriate types
            var parameters = string.Join(",", parameterNames);
            object[] parsedParameters = new object[parameterValues.Length];
            for (int i = 0; i < parameterValues.Length; i++)
            {
                //parsedParameters[i] = Convert.ChangeType(parameterValues[i].Trim(), typeof(object));
                //parsedParameters[i] = int.Parse(parameterValues[i].Trim().ToString());
                parsedParameters[i] = Convert.ChangeType(parameterValues[i], parameterTypes[i]);
            }

            return dynamicMethod?.Invoke(null, parsedParameters);
        }
    }

    public static int DynamicMethod(int param1) { return param1 + 2; }
}