using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms.Platform.Android;
using XamarinFormsProvilder.Sample.FSharp;

namespace XamarinFormsProvilder.Sample.Droid
{
    [Activity(Label = "XamarinFormsProvilder.Sample", MainLauncher = true)]
    public class MainActivity : AndroidActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);

            SetPage(App.GetMainPage());

        }
    }
}

