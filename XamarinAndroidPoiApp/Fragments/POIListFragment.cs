using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using XamarinAndroidPoiApp.Adapters;
using XamarinAndroidPoiApp.Managers;
using XamarinAndroidPoiApp.Models;
using XamarinAndroidPoiApp.Services;

namespace XamarinAndroidPoiApp.Fragments
{
    public class POIListFragment : Android.Support.V4.App.ListFragment
    {
        private ProgressBar progressBar;
        private List<PointOfInterest> poiListData;
        private POIListViewAdapter poiListAdapter;
        private Activity activity;
        private int scrollPosition;

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
            this.activity = activity;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            if (null != savedInstanceState)
            {
                scrollPosition = savedInstanceState.GetInt("scroll_position");
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            int currentPosition = ListView.FirstVisiblePosition;
            outState.PutInt("scroll_position", currentPosition);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            //return base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.POIListFragment, container, false);

            progressBar = view.FindViewById<ProgressBar>(Resource.Id.progressBar);

            HasOptionsMenu = true;

            return view;
        }

        public override void OnResume()
        {
            DownloadPoisListAsync();
            base.OnResume();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.POIListViewMenu, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.actionNew:
                    if (POIListActivity.isDualMode)
                    {
                        var detailFragment = new POIDetailsFragment();
                        Android.Support.V4.App.FragmentTransaction ft = FragmentManager.BeginTransaction();
                        ft.Replace(Resource.Id.poiDualDetailLayout, detailFragment);
                        ft.Commit();
                    }
                    else
                    {
                        Intent intent = new Intent(activity, typeof(POIDetailActivity));
                        StartActivity(intent);
                    }
                    return true;
                case Resource.Id.actionRefresh:
                    DownloadPoisListAsync();
                    return true;
                default: return base.OnOptionsItemSelected(item);
            }
        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            PointOfInterest poi = poiListData[position];
            if (POIListActivity.isDualMode)
            {
                var detailFragment = new POIDetailsFragment();
                detailFragment.Arguments = new Bundle();
                detailFragment.Arguments.PutString("poi", JsonConvert.SerializeObject(poi));
                Android.Support.V4.App.FragmentTransaction ft = FragmentManager.BeginTransaction();
                ft.Replace(Resource.Id.poiDualDetailLayout, detailFragment);
                ft.Commit();
            }
            else
            {
                Intent poiDetailIntent = new Intent(activity, typeof(POIDetailActivity));
                poiDetailIntent.PutExtra("poi", JsonConvert.SerializeObject(poi));
                StartActivity(poiDetailIntent);
            }
        }

        public async void DownloadPoisListAsync()
        {
            POIService service = new POIService();
            if (!service.isConnected(activity))
            {
                Toast toast = Toast.MakeText(activity,
                    "Not conntected to internet. Please check your device network settings.", ToastLength.Short);
                toast.Show();
                poiListData = DbManager.Instance.GetPOIListFromCache();
            }
            else
            {
                progressBar.Visibility = ViewStates.Visible;
                poiListData = await service.GetPOIListAsync();
                //Clear cached data        
                DbManager.Instance.ClearPOICache();
                //Save updated POI data
                DbManager.Instance.InsertAll(poiListData);
            }

            progressBar.Visibility = ViewStates.Gone;
            poiListAdapter = new POIListViewAdapter(activity, poiListData);
            this.ListAdapter = poiListAdapter;
            ListView.Post(() => { ListView.SetSelection(scrollPosition); });

        }

    }
}