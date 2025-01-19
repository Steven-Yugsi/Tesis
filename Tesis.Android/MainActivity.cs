using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Tesis;
using Xamarin.Essentials;

[Activity(Label = "Tesis.Android", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        Platform.Init(this, savedInstanceState);

        //Window.AddFlags(WindowManagerFlags.LayoutNoLimits);
        Window.ClearFlags(WindowManagerFlags.LayoutNoLimits);


        Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#ADD8E6"));

        if (Build.VERSION.SdkInt >= BuildVersionCodes.R) // Android 11 o superior
        {
            var controller = Window.InsetsController;
            if (controller != null)
            {
                controller.SetSystemBarsAppearance(
                    (int)WindowInsetsControllerAppearance.LightStatusBars,
                    (int)WindowInsetsControllerAppearance.LightStatusBars
                );
            }
        }        
        global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
        Com.Airbnb.Lottie.LottieAnimationView animationView = new Com.Airbnb.Lottie.LottieAnimationView(this);


        LoadApplication(new App());
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }
}
