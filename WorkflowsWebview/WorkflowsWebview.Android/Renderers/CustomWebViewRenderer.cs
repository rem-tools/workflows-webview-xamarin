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
                    Manifest.Permission.AccessFineLocation
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
            var workflow = JsonConvert.DeserializeObject(content);

            System.Diagnostics.Debug.WriteLine($"onWorkflowCompletion: {workflow}");
        }

        [JavascriptInterface]
        [Export("onStepCompletion")]
        public void OnStepCompletion(string content)
        {
            var step = JsonConvert.DeserializeObject(content);

            System.Diagnostics.Debug.WriteLine($"onStepCompletion: {step}");
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
    }

    public class CustomWebViewRenderProcessClient : WebViewRenderProcessClient
    {
        public override void OnRenderProcessResponsive(global::Android.Webkit.WebView view, WebViewRenderProcess renderer)
        {
            renderer.Terminate();
            view.Reload();
        }

        public override void OnRenderProcessUnresponsive(global::Android.Webkit.WebView view, WebViewRenderProcess renderer)
        {
            renderer.Terminate();
            view.Reload();
        }
    }

    public class CustomWebViewClient : WebViewClient
    {
        public override bool OnRenderProcessGone(global::Android.Webkit.WebView view, RenderProcessGoneDetail detail)
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
    }
}
