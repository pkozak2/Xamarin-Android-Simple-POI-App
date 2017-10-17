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
using XamarinAndroidPoiApp.Models;
using XamarinAndroidPoiApp.Services;

namespace XamarinAndroidPoiApp
{
    [Activity(Label = "POIDetailActivity")]
    public class POIDetailActivity : Activity
    {
        private PointOfInterest _poi;
        private EditText _nameEditText;
        private EditText _descrEditText;
        private EditText _addrEditText;
        private EditText _latEditText;
        private EditText _longEditText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.POIDetail);
            _nameEditText = FindViewById<EditText>(Resource.Id.nameEditText);
            _descrEditText = FindViewById<EditText>(Resource.Id.descrEditText);
            _addrEditText = FindViewById<EditText>(Resource.Id.addrEditText);
            _latEditText = FindViewById<EditText>(Resource.Id.latEditText);
            _longEditText = FindViewById<EditText>(Resource.Id.longEditText);

            if (Intent.HasExtra("poi"))
            {
                string poiJson = Intent.GetStringExtra("poi");
                _poi = JsonConvert.DeserializeObject<PointOfInterest>(poiJson);
            }
            else
            {
                _poi = new PointOfInterest();
            }

            UpdateUI();
        }

        protected void UpdateUI()
        {
            _nameEditText.Text = _poi.Name;
            _descrEditText.Text = _poi.Description;
            _addrEditText.Text = _poi.Address;
            _latEditText.Text = _poi.Latitude.ToString();
            _longEditText.Text = _poi.Longitude.ToString();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.POIDetailMenu, menu);
            base.OnCreateOptionsMenu(menu);

            // Disable delete for a new POI
            if (_poi.Id <= 0)
            {
                IMenuItem item = menu.FindItem(Resource.Id.actionDelete);
                item.SetEnabled(false);
                item.SetVisible(false);
            }
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.actionSave:
                    SavePOI();
                    return true;

                case Resource.Id.actionDelete:
                    DeletePOI();
                    return true;
                default: return base.OnOptionsItemSelected(item);
            }
        }

        protected void SavePOI()
        {
            bool errors = false;
            if (String.IsNullOrEmpty(_nameEditText.Text))
            {
                _nameEditText.Error = "Name cannot be empty";
                errors = true;
            }
            else _nameEditText.Error = null;

            double? tempLatitude = null;
            if (!String.IsNullOrEmpty(_latEditText.Text))
            {
                try
                {
                    tempLatitude = Double.Parse(_latEditText.Text);
                    if ((tempLatitude > 90) | (tempLatitude < -90))
                    {
                        _latEditText.Error = "Latitude must be a decimal value between -90 and 90";
                        errors = true;
                    }
                    else _latEditText.Error = null;
                }
                catch
                {
                    _latEditText.Error = "Latitude must be valid decimal number";
                    errors = true;
                }
            }
            double? tempLongitude = null;
            if (!String.IsNullOrEmpty(_longEditText.Text))
            {
                try
                {
                    tempLongitude = Double.Parse(_longEditText.Text);
                    if ((tempLongitude > 180) | (tempLongitude < -180))
                    {
                        _longEditText.Error = "Longitude must be a decimal value between -180 and 180";
                        errors = true;
                    }
                    else _longEditText.Error = null;
                }
                catch
                {
                    _longEditText.Error = "Longitude must be valid decimal number";
                    errors = true;
                }
            }

            if (errors)
            {
                return;
            }


            _poi.Name = _nameEditText.Text;
            _poi.Description = _descrEditText.Text;
            _poi.Address = _addrEditText.Text;
            _poi.Latitude = tempLatitude;
            _poi.Longitude = tempLongitude;
            CreateOrUpdatePOIAsync(_poi);
        }

        protected void ConfirmDelete(object sender, EventArgs e)
        {
            DeletePOIAsync();
        }

        protected void DeletePOI()
        {
            AlertDialog.Builder alertConfirm = new AlertDialog.Builder(this);
            alertConfirm.SetTitle("Confirm delete");
            alertConfirm.SetCancelable(false);
            alertConfirm.SetPositiveButton("OK", ConfirmDelete);
            alertConfirm.SetNegativeButton("Cancel", delegate { });
            alertConfirm.SetMessage(String.Format("Are you sure you want to delete {0}?", _poi.Name));
            alertConfirm.Show();
        }

        private async void CreateOrUpdatePOIAsync(PointOfInterest poi)
        {
            POIService service = new POIService();
            if (!service.isConnected(this))
            {
                Toast toast = Toast.MakeText(this,
                    "Not conntected to internet. Please check your device network settings.", ToastLength.Short);
                toast.Show();
                return;
            }
            string response = await service.CreateOrUpdatePOIAsync(_poi, this);
            if (!string.IsNullOrEmpty(response))
            {
                Toast toast = Toast.MakeText(this, String.Format("{0} saved.", _poi.Name), ToastLength.Short);
                toast.Show();
                Finish();
            }
            else
            {
                Toast toast = Toast.MakeText(this, "Something went Wrong!", ToastLength.Short);
                toast.Show();
            }
        }

        public async void DeletePOIAsync()
        {
            POIService service = new POIService();
            if (!service.isConnected(this))
            {
                Toast toast = Toast.MakeText(this,
                    "Not conntected to internet. Please check your device network settings.", ToastLength.Short);
                toast.Show();
                return;
            }
            string response = await service.DeletePOIAsync(_poi.Id);
            if (!string.IsNullOrEmpty(response))
            {
                Toast toast = Toast.MakeText(this, String.Format("{0} deleted.", _poi.Name), ToastLength.Short);
                toast.Show();
                Finish();
            }
            else
            {
                Toast toast = Toast.MakeText(this, "Something went Wrong!", ToastLength.Short);
                toast.Show();
            }
        }


    }
}