using System.Collections.ObjectModel;
using CSharpInvokingMethodsDynamically.Core;

namespace ClientMauiApp
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Entry> ParameterEntries { get; } = new ObservableCollection<Entry>();

        public MainPage()
        {
            InitializeComponent();
        }

        private void MethodEditor_Completed(object sender, EventArgs e)
        {
            ExecuteCode.IsEnabled = true;
            ParameterEntries.Clear();
            if (!string.IsNullOrEmpty(MethodEditor.Text?.Trim()))
            {
                List<string> parameters = DynamicMethodExecution.ExtractParametersFromMethod(MethodEditor.Text);
                if (parameters.Any())
                {
                    ParameterLabel.IsVisible = true;
                    foreach (var parameter in parameters)
                    {
                        ParameterEntries.Add(new Entry
                        {
                            Placeholder = $"Enter parameter value for parameter: {parameter}",
                            IsEnabled = true,
                            Text = string.Empty,
                        });
                    }
                }
                else
                {
                    ParameterLabel.IsVisible = false;
                }

                BindingContext = this;
            }
        }

        private async void ExecuteCode_Clicked(object sender, EventArgs e)
        {
            MethodEditor.IsEnabled = false;
            ExecuteCode.IsEnabled = false;
            Result.Text = string.Empty;

            await Task.Delay(500);

            List<string> parameters = DynamicMethodExecution.ExtractParametersFromMethod(MethodEditor.Text);

            List<string>? parameterNames = null;
            List<string>? parameterValues = null;
            List<TypeCode>? parameterTypes = null;

            if (parameters.Any())
            {
                parameterNames = new List<string>();
                parameterValues = new List<string>();
                parameterTypes = new List<TypeCode>();
                int index = 0;
                foreach (string parameter in parameters)
                {
                    var paramSplitted = parameter?.Split(' ');
                    parameterNames.Add(paramSplitted.Last());
                    parameterValues.Add(ParameterEntries[index].Text);
                    parameterTypes.Add(DynamicMethodExecution.ConvertParameterTypeFromString(paramSplitted.First()));
                    index++;
                }
            }

            try
            {
                // Compile and execute the method with parameters
                object? result = DynamicMethodExecution.ExecuteMethod(MethodEditor.Text,
                    parameterNames?.ToArray(), parameterValues?.ToArray(), parameterTypes?.ToArray());
                Result.Text = result.ToString();
            }
            catch (Exception ex)
            {
                Result.Text = "Error: " + ex.Message;
            }

            MethodEditor.IsEnabled = true;
            ExecuteCode.IsEnabled = true;
        }
    }
}