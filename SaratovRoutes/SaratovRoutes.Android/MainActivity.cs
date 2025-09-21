using System;
using Android;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Android.Webkit;
using Xamarin.Essentials;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;

namespace SaratovRoutes.Droid
{
    [Activity(Label = "SaratovRoutes", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        const int RequestLocationId = 0;

        readonly string[] LocationPermissions =
        {
        Manifest.Permission.AccessFineLocation,
        Manifest.Permission.AccessCoarseLocation
    };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            

            
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
            Android.Webkit.WebView webView = new Android.Webkit.WebView(this);
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.SetGeolocationEnabled(true);
           // Xamarin.Forms.WebView.SetWebContentsDebuggingEnabled(true);
            //Android.Webkit.WebView webView2 = new Android.Webkit.WebView(this);
            //webView2.Settings.JavaScriptEnabled = true;
            //webView2.Settings.SetGeolocationEnabled(true);
            //webView.SetWebChromeClient(new GeolocationWebChromeClient());
            LoadApplication(new App());
            RequestPermissions();
        }
        void RequestPermissions()
        {
            if ((ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted) &&
                (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) == (int)Permission.Granted))
            {
                // Permissions already granted - display a message or do something
            }
            else
            {
                // Permissions not granted - request them
                ActivityCompat.RequestPermissions(this, LocationPermissions, RequestLocationId);
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            switch (requestCode)
            {
                case RequestLocationId:
                    {
                        if (grantResults.Length > 0 && grantResults[0] == (int)Permission.Granted)
                        {
                            // Permissions granted - display a message or do something
                        }
                        else
                        {
                            // Permissions denied - display a message or disable functionality
                        }
                    }
                    break;
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public class GeoWebChromeClient : WebChromeClient
        {
            public override void OnGeolocationPermissionsShowPrompt(string origin, GeolocationPermissions.ICallback callback)
            {
                callback.Invoke(origin, true, false);
            }
        }

    }
}