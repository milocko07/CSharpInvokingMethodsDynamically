namespace CSharpInvokingMethodsDynamically.Core;

using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public static class DynamicExecution
{
    const string CLASS_NAME = "DynamicMethodClass";
    static readonly string template = "using System; \n" +
        "public class {0} {{\n" +
        "\t{1}\n" +
        "}}";
        //$"public static object DynamicMethod(object {parameters}) {{ {methodBody} }}
        //"{0}" +
        //"} }";

    public static object? ExecuteMethod(string? methodBody, string[]? parameterNames, string[]? parameterValues, TypeCode[]? parameterTypes)
    {
        if (string.IsNullOrEmpty(methodBody))
        {
            throw new ArgumentException("Missing method body.");
        }

        if (parameterNames != null && parameterValues != null && parameterNames.Length != parameterValues.Length)
        {
            throw new ArgumentException("Mismatch between parameter names and parameter values.");
        }

        if (parameterNames != null && parameterNames.Count(p => string.IsNullOrEmpty(p.Trim())) == parameterNames.Length)
        {
            throw new ArgumentException("Some parameter names are empty.");
        }

        if (parameterValues != null && parameterValues.Count(p => string.IsNullOrEmpty(p.ToString().Trim())) == parameterValues.Length)
        {
            throw new ArgumentException("Some parameter values are empty.");
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