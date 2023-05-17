using System;
using WorkflowsWebview.Controls;
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

            Content = webView;
        }
    }
}
