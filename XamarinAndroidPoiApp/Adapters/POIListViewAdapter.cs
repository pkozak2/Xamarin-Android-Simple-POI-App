using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using XamarinAndroidPoiApp.Models;

namespace XamarinAndroidPoiApp.Adapters
{
    public class POIListViewAdapter : BaseAdapter<PointOfInterest>
    {
        private readonly Activity context;
        private List<PointOfInterest> poiListData;
        public Location CurrentLocation { get; set; }

        public POIListViewAdapter(Activity _context, List<PointOfInterest> _poiListData) : base()
        {
            this.context = _context;
            this.poiListData = _poiListData;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            if (view == null)
            {
                view = context.LayoutInflater.Inflate(Resource.Layout.POIListItem, null);
            }

            PointOfInterest poi = this[position];
            view.FindViewById<TextView>(Resource.Id.nameTextView).Text = poi.Name;
            if (String.IsNullOrEmpty(poi.Address))
            {
                view.FindViewById<TextView>(Resource.Id.addrTextView).Visibility = ViewStates.Gone;
            }
            else
            {
                view.FindViewById<TextView>(Resource.Id.addrTextView).Text = poi.Address;
            }
            var imageView = view.FindViewById<ImageView>(Resource.Id.poiImageView);
            if (!String.IsNullOrEmpty(poi.Image))
            {
                Koush.UrlImageViewHelper.SetUrlDrawable(imageView, poi.Image, Resource.Drawable.icon);
            }

            var distanceTextView = view.FindViewById<TextView>(Resource.Id.distanceTextView);
            if ((CurrentLocation != null) && (poi.Latitude.HasValue) && (poi.Longitude.HasValue))
            {
                Location poiLocation = new Location("");
                poiLocation.Latitude = poi.Latitude.Value;
                poiLocation.Longitude = poi.Longitude.Value;
                float distance = CurrentLocation.DistanceTo(poiLocation) * 0.000621371F;
                distanceTextView.Text = String.Format("{0:0,0.00}  miles", distance);
            }
            else
            {
                distanceTextView.Text = "??";
            }
            return view;
        }

        public override int Count
        {
            get { return poiListData.Count; }
        }

        public override PointOfInterest this[int position]
        {
            get { return poiListData[position]; }
        }
    }
}