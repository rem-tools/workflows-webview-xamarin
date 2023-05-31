using Newtonsoft.Json;
using System;
using WorkflowsWebview.Controls;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WorkflowsWebview
{
    public partial class MainPage : ContentPage
    {
        public const string Url = "WORKFLOW_URL";
        
        public MainPage()
        {
            InitializeComponent();
        }

        private void OpenWebViewActivityButton_Clicked(object sender, EventArgs e)
        {

            var webView = new CustomWebView();

            // logs Url
            System.Diagnostics.Debug.WriteLine(Url);

            webView.Source = new UrlWebViewSource
            {
                Url = Url,
            };

            webView.OnScriptMessageReceived += (entity, value) =>
            {
                // parsedValue is the step or workflow object
                var parsedValue = JsonConvert.DeserializeObject(value);

                if (entity == "step")
                {
                    // In case of step event
                    System.Diagnostics.Debug.WriteLine($"step: {parsedValue}");
                }
                else if (entity == "workflow")
                {
                    // In case of workflow event
                    System.Diagnostics.Debug.WriteLine($"workflow: {parsedValue}");
                }
            };

            Content = webView;
        }
    }
}
