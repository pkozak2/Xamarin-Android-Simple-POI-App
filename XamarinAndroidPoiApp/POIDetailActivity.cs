using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using XamarinAndroidPoiApp.Fragments;
using XamarinAndroidPoiApp.Models;
using XamarinAndroidPoiApp.Services;

namespace XamarinAndroidPoiApp
{
    [Activity(Label = "POIDetailActivity")]
    public class POIDetailActivity : Activity
    {
        PointOfInterest _poi;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.POIDetail);

            var detailFragment = new POIDetailsFragment();
            detailFragment.Arguments = new Bundle();
            if (Intent.HasExtra("poi"))
            {
                string poiJson = Intent.GetStringExtra("poi");
                detailFragment.Arguments.PutString("poi", poiJson);
            }

            FragmentTransaction ft = FragmentManager.BeginTransaction();
            ft.Add(Resource.Id.poiDetailLayout, detailFragment);
            ft.Commit();
        }


    }
}