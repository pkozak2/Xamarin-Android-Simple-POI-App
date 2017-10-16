using System.Collections.Generic;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using XamarinAndroidPoiApp.Adapters;
using XamarinAndroidPoiApp.Models;

namespace XamarinAndroidPoiApp
{
    [Activity(Label = "XamarinAndroidPoiApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class POIListActivity : Activity
    {
        private ListView poiListView;
        private ProgressBar progressBar;
        private List<PointOfInterest> poiListData;
        private POIListViewAdapter poiListAdapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.POIList);

            poiListView = FindViewById<ListView>(Resource.Id.poiListView);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

            DownloadPoisListAsync();
        }

        public async void DownloadPoisListAsync()
        {
            progressBar.Visibility = ViewStates.Visible;
            poiListData = GetPoisListTestData();
            progressBar.Visibility = ViewStates.Gone;
            poiListAdapter = new POIListViewAdapter(this, poiListData);
            poiListView.Adapter = poiListAdapter;
        }

        private List<PointOfInterest> GetPoisListTestData()
        {
            List<PointOfInterest> listData = new List<PointOfInterest>();
            for (int i = 0; i < 20; i++)
            {
                PointOfInterest poi = new PointOfInterest();
                poi.Id = i;
                poi.Name = "Name " + i;
                poi.Address = "Address " + i;
                listData.Add(poi);
            }
            return listData;
        }
    }
}

