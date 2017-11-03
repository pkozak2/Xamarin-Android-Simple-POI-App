using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Widget;
using Android.OS;
using Android.Views;
using Newtonsoft.Json;
using XamarinAndroidPoiApp.Adapters;
using XamarinAndroidPoiApp.Managers;
using XamarinAndroidPoiApp.Models;
using XamarinAndroidPoiApp.Services;

namespace XamarinAndroidPoiApp
{
    [Activity(Label = "XamarinAndroidPoiApp", MainLauncher = true, Icon = "@drawable/icon",
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class POIListActivity : Android.Support.V4.App.FragmentActivity
    {
        public static bool isDualMode = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.POIList);

            DbManager.Instance.CreateTable();

            var detailsLayout = FindViewById(Resource.Id.poiDualDetailLayout);
            if (detailsLayout != null && detailsLayout.Visibility == ViewStates.Visible)
            {
                isDualMode = true;
            }
            else
            {
                isDualMode = false;
            }

        }
    }
}

