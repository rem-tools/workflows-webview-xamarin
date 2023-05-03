using WorkflowsWebview.Controls;
using WorkflowsWebview.iOS.Renderers;
using WebKit;
using Xamarin.Forms;
using System.Diagnostics;
using Xamarin.Forms.Platform.iOS;
using Newtonsoft.Json;
using System.Collections.Generic;
using Foundation;

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
                    // Personaliza la configuraci�n del WebView aqu�
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

                if (!(message.Body is NSDictionary messageBody))
                {
                    Debug.WriteLine("Wrong message");
                    return;
                }

                NSObject entityObject, valueObject;
                if (!messageBody.TryGetValue(new NSString("entity"), out entityObject) ||
                    !messageBody.TryGetValue(new NSString("value"), out valueObject))
                {
                    Debug.WriteLine("Wrong message");
                    return;
                }

                string entity = entityObject.ToString();
                string value = valueObject.ToString();

                if (entity == "step")
                {
                    Debug.WriteLine($"Step: {entity} - {value}");

                    // Aqui se puede usar el contenido del Step
                }
                else if (entity == "workflow")
                {
                    Debug.WriteLine($"Workflow: {entity} - {value}");

                    // Aqui se puede usar el contenido del Workflow
                }
                else
                {
                    Debug.WriteLine("Wrong message");
                }
            }
        }
    }

}
