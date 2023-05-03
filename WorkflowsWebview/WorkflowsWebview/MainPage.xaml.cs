using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowsWebview.Controls;
using Xamarin.Forms;

namespace WorkflowsWebview
{
    public partial class MainPage : ContentPage
    {
        
        public MainPage()
        {
            var customWebview = new CustomWebView
            {
                Source = new UrlWebViewSource
                {
                    // Aqui va el URL de nuestro Workflow
                    Url = "URL_WORKFLOW"
                },
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            
            Content = customWebview;
        }
    }
}
