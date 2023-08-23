using CSharpInvokingMethodsDynamically.Core;

namespace ClientMauiApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void MethodEditor_Unfocused(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(MethodEditor.Text?.Trim()))
            {
                ExecuteCode.IsEnabled = true;
                List<string> parameters = DynamicExecution.ExtractParametersFromMethod(MethodEditor.Text);
            }
            else 
            { 
                ExecuteCode.IsEnabled = false; 
            }
        }

        private void ExecuteCode_Clicked(object sender, EventArgs e)
        {
            object? result = DynamicExecution.ExecuteMethod(MethodEditor.Text, null, null, null);
            resultLabel.Text = result.ToString();
        }

        //private void OnCounterClicked(object sender, EventArgs e)
        //{
        //    count++;

        //    if (count == 1)
        //        CounterBtn.Text = $"Clicked {count} time";
        //    else
        //        CounterBtn.Text = $"Clicked {count} times";

        //    SemanticScreenReader.Announce(CounterBtn.Text);
        //}
    }
}