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
using XamarinAndroidPoiApp.Models;
using XamarinAndroidPoiApp.Services;

namespace XamarinAndroidPoiApp
{
    [Activity(Label = "XamarinAndroidPoiApp", MainLauncher = true, Icon = "@drawable/icon",
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class POIListActivity : Activity
    {
        private ListView poiListView;
        private ProgressBar progressBar;
        private List<PointOfInterest> poiListData;
        private POIListViewAdapter poiListAdapter;
        int scrollPosition;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.POIList);

            poiListView = FindViewById<ListView>(Resource.Id.poiListView);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

            //DownloadPoisListAsync();

            poiListView.ItemClick += POIClicked;
        }

        protected override void OnResume()
        {
            base.OnResume();
            DownloadPoisListAsync();
        }

        //public async void DownloadPoisListAsync()
        //{
        //    progressBar.Visibility = ViewStates.Visible;
        //    poiListData = GetPoisListTestData();
        //    progressBar.Visibility = ViewStates.Gone;
        //    poiListAdapter = new POIListViewAdapter(this, poiListData);
        //    poiListView.Adapter = poiListAdapter;
        //}

        //private List<PointOfInterest> GetPoisListTestData()
        //{
        //    List<PointOfInterest> listData = new List<PointOfInterest>();
        //    for (int i = 0; i < 20; i++)
        //    {
        //        PointOfInterest poi = new PointOfInterest();
        //        poi.Id = i;
        //        poi.Name = "Name " + i;
        //        poi.Address = "Address " + i;
        //        listData.Add(poi);
        //    }
        //    return listData;
        //}

        public async void DownloadPoisListAsync()
        {
            POIService service = new POIService();
            if (!service.isConnected(this))
            {
                Toast toast = Toast.MakeText(this,
                    "Not conntected to internet. Please check your device network settings.", ToastLength.Short);
                toast.Show();
            }
            else
            {
                progressBar.Visibility = ViewStates.Visible;
                poiListData = await service.GetPOIListAsync();
                progressBar.Visibility = ViewStates.Gone;
                poiListAdapter = new POIListViewAdapter(this, poiListData);
                poiListView.Adapter = poiListAdapter;
            }

            poiListView.Post(() =>
            {
                poiListView.SetSelection(scrollPosition);
            });
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.POIListViewMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.actionNew: // place holder for creating new poi 
                    StartActivity(typeof(POIDetailActivity));
                    return true;
                case Resource.Id.actionRefresh:
                    DownloadPoisListAsync();
                    return true;
                default: return base.OnOptionsItemSelected(item);
            }
        }

        protected void POIClicked(object sender, ListView.ItemClickEventArgs e)
        {
            PointOfInterest poi = poiListData[(int) e.Id];
            Intent poiDetailIntent = new Intent(this, typeof(POIDetailActivity));
            string poiJson = JsonConvert.SerializeObject(poi);
            poiDetailIntent.PutExtra("poi", poiJson);
            StartActivity(poiDetailIntent);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState); // Place your logic to save activity state 

            int currentPosition = poiListView.FirstVisiblePosition;
            outState.PutInt("scroll_position", currentPosition);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState); // Place your logic to restore activity state 
            scrollPosition = savedInstanceState.GetInt("scroll_position");

        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig); //update UI to reflect the orientation change 
        }
    }
}

