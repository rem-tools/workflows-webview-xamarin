using Android.Views;
using Android.Content;
using Android.Webkit;
using Java.Interop;
using System.Threading.Tasks;
using WorkflowsWebview.Controls;
using WorkflowsWebview.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android;
using Android.Content.PM;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Newtonsoft.Json;
using Org.Json;
using Android.OS;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]
namespace WorkflowsWebview.Droid.Renderers
{
    public class CustomWebViewRenderer : WebViewRenderer
    {
        public CustomWebViewRenderer(Context context) : base(context)
        {
        }

        private async Task<bool> CheckPermissionsAsync()
        {
            string[] requiredPermissions =
                {
                    Manifest.Permission.Camera,
                    Manifest.Permission.RecordAudio,
                    Manifest.Permission.AccessFineLocation,
                    Manifest.Permission.AccessCoarseLocation
                };

            bool allPermissionsGranted = true;

            foreach (string permission in requiredPermissions)
            {
                if (ContextCompat.CheckSelfPermission(Context, permission) != Permission.Granted)
                {
                    allPermissionsGranted = false;
                    break;
                }
            }

            if (!allPermissionsGranted)
            {
                ActivityCompat.RequestPermissions(MainActivity.Instance, requiredPermissions, 0);
                await Task.Delay(200);
            }

            return allPermissionsGranted;
        }

        protected override async void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                await CheckPermissionsAsync();

                // Personaliza la configuración del WebView aquí
                Control.Settings.JavaScriptEnabled = true;

                Control.SetInitialScale(0);
                Control.VerticalScrollBarEnabled = false;

                Control.Settings.JavaScriptEnabled = true;
                Control.Settings.JavaScriptCanOpenWindowsAutomatically = true;
                Control.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.Normal);
                Control.Settings.MediaPlaybackRequiresUserGesture = false;
                Control.Settings.DomStorageEnabled = true;
                Control.Settings.AllowFileAccess = true;
                Control.Settings.AllowContentAccess = true;
                Control.Settings.AllowUniversalAccessFromFileURLs = true;
                Control.Settings.LoadsImagesAutomatically = true;

                // Update: Agregamos la aceleracion de hardware para el webview
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    Control.Settings.MixedContentMode = MixedContentHandling.AlwaysAllow;
                    Control.SetLayerType(LayerType.Hardware, null);
                }
                else if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
                {
                    Control.SetLayerType(LayerType.Hardware, null);
                } else
                {
                    Control.SetLayerType(LayerType.Software, null);
                }

                // Opcional: ajustar la prioridad de renderizado para que su contenido se muestre primero
                Control.SetRendererPriorityPolicy(RendererPriority.Important, false);
                
                Control.Settings.SetSupportZoom(false);
                Control.Settings.SetGeolocationEnabled(true);

                if (Control != null && e.NewElement is CustomWebView customWebView)
                {
                    Control.SetWebChromeClient(new CustomWebChromeClient(customWebView));
                }

                IntentFilter intentFilter = new IntentFilter();
                intentFilter.AddAction(Intent.ActionConfigurationChanged);

                Control.WebViewRenderProcessClient = new CustomWebViewRenderProcessClient();

                Control.SetWebViewClient(new CustomWebViewClient());

                Control.AddJavascriptInterface(new WorkflowsWebviewJsInterface(this), CustomWebView.JavascriptInterfaceName);
            }
        }
    }

    public class WorkflowsWebviewJsInterface : Java.Lang.Object
    {
        CustomWebViewRenderer _renderer;

        public WorkflowsWebviewJsInterface(CustomWebViewRenderer renderer)
        {
            _renderer = renderer;
        }

        [JavascriptInterface]
        [Export("onWorkflowCompletion")]
        public void OnWorkflowCompletion(string content)
        {
            if (_renderer.Element is CustomWebView customWebView)
            {
                customWebView.InvokeScriptMessageReceived("workflow", content);
            }

            System.Diagnostics.Debug.WriteLine("onWorkflowCompletion: TRIGGERED");
        }

        [JavascriptInterface]
        [Export("onStepCompletion")]
        public void OnStepCompletion(string content)
        {
            if (_renderer.Element is CustomWebView customWebView)
            {
                customWebView.InvokeScriptMessageReceived("step", content);
            }

            System.Diagnostics.Debug.WriteLine("onStepCompletion: TRIGGERED");
        }
    }

    public class CustomWebChromeClient : WebChromeClient
    {
        private readonly CustomWebView _customWebView;
        public CustomWebChromeClient(CustomWebView customWebView)
        {
            _customWebView = customWebView;
        }
        public override void OnPermissionRequest(PermissionRequest request)
        {
            request.Grant(request.GetResources());
        }

        public override void OnGeolocationPermissionsShowPrompt(string origin, GeolocationPermissions.ICallback callback)
        {
            base.OnGeolocationPermissionsShowPrompt(origin, callback);
            callback.Invoke(origin, true, false);
        }
    }

    public class CustomWebViewRenderProcessClient : WebViewRenderProcessClient
    {
        public override void OnRenderProcessResponsive(Android.Webkit.WebView view, WebViewRenderProcess renderer)
        {
            renderer.Terminate();
            view.Reload();
        }

        public override void OnRenderProcessUnresponsive(Android.Webkit.WebView view, WebViewRenderProcess renderer)
        {
            renderer.Terminate();
            view.Reload();
        }
    }

    public class CustomWebViewClient : WebViewClient
    {
        public override bool OnRenderProcessGone(Android.Webkit.WebView view, RenderProcessGoneDetail detail)
        {
            if (detail != null)
            {
                if (!detail.DidCrash())
                {
                    view.Destroy();
                    return true;
                }
                return false;
            }
            return base.OnRenderProcessGone(view, detail);
        }

        // OnPageFinished se llama cuando la página termina de cargarse
        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            base.OnPageFinished(view, url);

            // Workaround to Chromium Error injecting an element with movement into our Workflow Webview
            // https://bugs.chromium.org/p/chromium/issues/detail?id=1401352#c12
            string jsCode = "var spinner = document.createElement('div');" +
                "spinner.setAttribute('class', 'spinner');" +
                "document.body.insertBefore(spinner, document.body.firstChild);" +
                "var style = document.createElement('style');" +
                "style.innerHTML = '.spinner { position: absolute; top: 0; left: 0; z-index: 9999; border: 1px solid rgba(0,0,0,0.1); border-top-color: rgba(255,255,255,0.1); border-radius: 50%; width: 1px; height: 1px; animation: spin 1s linear infinite; } @keyframes spin { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }';" +
                "document.head.appendChild(style);";

            view.EvaluateJavascript(jsCode, null);
        }
    }
}
