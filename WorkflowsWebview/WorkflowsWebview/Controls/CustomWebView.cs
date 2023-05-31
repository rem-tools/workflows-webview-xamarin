using System;
using Xamarin.Forms;


namespace WorkflowsWebview.Controls
{
    public class CustomWebView : WebView
    {
        public const string JavascriptInterfaceName = "workflowsWebview";

        public event Action<string, string> OnScriptMessageReceived;

        public void InvokeScriptMessageReceived(string entity, string value)
        {
            OnScriptMessageReceived?.Invoke(entity, value);
        }
    }
}
