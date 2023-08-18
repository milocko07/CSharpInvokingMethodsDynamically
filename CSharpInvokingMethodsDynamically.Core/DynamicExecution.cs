namespace CSharpInvokingMethodsDynamically.Core;

using System.Reflection;
using Microsoft.CodeAnalysis;
public static class DynamicExecution
{
    static readonly string template = "using System; " +
        "public class DynamicMethodClass {{ " +
        //$"public static object DynamicMethod(object {parameters}) {{ {methodBody} }}
        "{0}" +
        "}}";

    public static object ExecuteMethod(string methodBody, string[] parameterNames, string[] parameterValues)
    {
        string parameters = string.Join(", ", parameterNames);
        string code = $"using System; public class DynamicMethodClass {{ public static object DynamicMethod(object {parameters}) {{ {methodBody} }} }}";

        // Compile the code and create an assembly
        var compilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create("DynamicMethodAssembly")
            .WithOptions(new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            //.AddReferences(AppDomain.CurrentDomain.GetAssemblies())
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddSyntaxTrees(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(code));

        var memoryStream = new System.IO.MemoryStream();
        var compilationResult = compilation.Emit(memoryStream);

        if (!compilationResult.Success)
        {
            throw new InvalidOperationException("Compilation failed.");
        }

        Assembly assembly = Assembly.Load(memoryStream.ToArray());

        // Get the DynamicMethodClass type and DynamicMethod method
        Type dynamicMethodClassType = assembly.GetType("DynamicMethodClass");
        MethodInfo dynamicMethod = dynamicMethodClassType.GetMethod("DynamicMethod");

        // Convert parameter values to appropriate types
        object[] parsedParameters = new object[parameterValues.Length];
        for (int i = 0; i < parameterValues.Length; i++)
        {
            //parsedParameters[i] = Convert.ChangeType(parameterValues[i].Trim(), typeof(object));
            parsedParameters[i] = int.Parse(parameterValues[i].Trim().ToString());
        }

        //for (int i = 0; i < parameterValues.Length; i++)
        //{
        //    parsedParameters[i] = 88;
        //}

        // Invoke the method
        object instance = null;
        object result = dynamicMethod.Invoke(instance, parsedParameters);
        return result;
    }


}