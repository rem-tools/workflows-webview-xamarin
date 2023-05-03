using WorkflowsWebview.Controls;
using WorkflowsWebview.iOS.Renderers;
using WebKit;
using Xamarin.Forms;
using System.Diagnostics;
using Xamarin.Forms.Platform.iOS;
using Newtonsoft.Json;
using System.Collections.Generic;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]
namespace WorkflowsWebview.iOS.Renderers
{
    public class CustomWebViewRenderer : WkWebViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (NativeView != null)
            {
                var webView = NativeView as WKWebView;

                if (webView != null)
                {
                    // Personaliza la configuración del WebView aquí
                    webView.Configuration.Preferences.JavaScriptEnabled = true;
                    webView.AllowsBackForwardNavigationGestures = false;
                    webView.Configuration.AllowsInlineMediaPlayback = true;
                    webView.Configuration.MediaTypesRequiringUserActionForPlayback = WKAudiovisualMediaTypes.None;
                    webView.Configuration.MediaPlaybackRequiresUserAction = false;

                    webView.Configuration.UserContentController.AddScriptMessageHandler(new WorkflowsWebViewScriptMessageHandler(), CustomWebView.JavascriptInterfaceName);

                }
            }
        }
    }

    public class WorkflowsWebViewScriptMessageHandler : WKScriptMessageHandler
    {
        public override void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            if (message.Name == "workflowsWebview")
            {
                var messageBody = JsonConvert.DeserializeObject<Dictionary<string, string>>(message.Body.ToString());

                if (messageBody["entity"] == "step")
                {
                    var step = messageBody["value"];
                    Debug.WriteLine($"Step: {messageBody["entity"]} - {step}");
                } else
                {
                    var workflow = messageBody["value"];
                    Debug.WriteLine($"Workflow: {messageBody["entity"]} - {workflow}");
                }
            }
        }
    }

}
