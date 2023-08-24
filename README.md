# CsharpInvokingMethodsDinamically
This library demostrates how to invoke and run c# **primitive methods** dinimically from a string that contains the structure of those methods.

**Maui app demo:**
![image](https://github.com/milocko07/CSharpInvokingMethodsDynamically/assets/37205551/88080c78-cd5d-4f06-b8a3-4d8c54e77914)


![image](https://github.com/milocko07/CSharpInvokingMethodsDynamically/assets/37205551/700741a5-dcb7-45f7-878a-72be99dbd30d)


![image](https://github.com/milocko07/CSharpInvokingMethodsDynamically/assets/37205551/4cc8103f-3de9-4049-a108-52a38a032e43)

**Console app demo:**

![image](https://github.com/milocko07/CSharpInvokingMethodsDynamically/assets/37205551/945c4cbe-ce4f-4663-b3f9-9f510ea2b010)

**Instalation:**

Download code, open with Visual Studio 2022, compile and choose your client app (console or maui) to open with.

**Examples:**

 ```csharp
public static double SumDecimals(double param1,   double param2){ return param1 + param2; }
```
---------------------------------------------------------------
```csharp
public static string CalculateFibonacciSeriesAsString(int n)
{
    StringBuilder result = new StringBuilder();
    if (n >= 1)
    {
        result.Append("0");
    }
    if (n >= 2)
    {
        result.Append(", 1");
    }
    int a = 0, b = 1;
    for (int i = 2; i < n; i++)
    {
        int nextTerm = a + b;
        result.Append($", {nextTerm}");
        a = b;
        b = nextTerm;
    }
    return result.ToString();
}
```
---------------------------------------------------------------
```csharp
public static string CalcualteOdds(int number1, int number2)
{
    var desde = number1;
    var hasta = number2;

    var primos = string.Empty;

    for (int i = desde; i < hasta; i++)
    {
        if ((i % 2) != 0)
        {
            primos += $"{i}, ";
        }
    }
    return primos;
}
```
