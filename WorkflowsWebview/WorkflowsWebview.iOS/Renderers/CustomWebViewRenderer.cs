using WorkflowsWebview.Controls;
using WorkflowsWebview.iOS.Renderers;
using WebKit;
using Xamarin.Forms;
using System.Diagnostics;
using Xamarin.Forms.Platform.iOS;
using Foundation;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]
namespace WorkflowsWebview.iOS.Renderers
{
    public class CustomWebViewRenderer : ViewRenderer<CustomWebView, WKWebView>
    {
        WKWebView _wkWebView;

        protected override void OnElementChanged(ElementChangedEventArgs<CustomWebView> e)
        {
            base.OnElementChanged(e);

            if (_wkWebView == null)
            {
                var userController = new WKUserContentController();
                userController.AddScriptMessageHandler(new WorkflowsWebViewScriptMessageHandler(this), CustomWebView.JavascriptInterfaceName);

                var config = new WKWebViewConfiguration
                {
                    AllowsInlineMediaPlayback = true,
                    MediaTypesRequiringUserActionForPlayback = WKAudiovisualMediaTypes.None,
                    MediaPlaybackRequiresUserAction = false,
                    RequiresUserActionForMediaPlayback = false,
                    UserContentController = userController
                };

                _wkWebView = new WKWebView(Frame, config);
                SetNativeControl(_wkWebView);
            }

            if (e.OldElement != null)
            {
                Control.Configuration.UserContentController.RemoveScriptMessageHandler(CustomWebView.JavascriptInterfaceName);
            }

            if (e.NewElement != null)
            {
                var urlSource = (UrlWebViewSource)e.NewElement.Source;
                _wkWebView.LoadRequest(new NSUrlRequest(new NSUrl(urlSource.Url)));
            }
        }
    }

    public class WorkflowsWebViewScriptMessageHandler : WKScriptMessageHandler
    {
        private readonly CustomWebViewRenderer _renderer;

        public WorkflowsWebViewScriptMessageHandler(CustomWebViewRenderer renderer)
        {
            _renderer = renderer;
        }

        public override void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            if (message.Name == "workflowsWebview")
            {
                // your existing code...

                if (_renderer.Element is CustomWebView customWebView)
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
                        customWebView.InvokeScriptMessageReceived(entity, value);
                        Debug.WriteLine($"Step: {entity} - {value}");

                        // Aqui se puede usar el contenido del Step
                    }
                    else if (entity == "workflow")
                    {
                        customWebView.InvokeScriptMessageReceived(entity, value);
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

}
