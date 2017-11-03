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

namespace XamarinAndroidPoiApp.Fragments
{
    public class ProgressDialogFragment : Android.Support.V4.App.DialogFragment
    {
        public override Dialog OnCreateDialog(Android.OS.Bundle savedInstanceState)
        {
            Cancelable = false;
            ProgressDialog _progressDialog = new ProgressDialog(Activity);
            _progressDialog.SetMessage("Getting location...");
            _progressDialog.Indeterminate = true;
            _progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            return _progressDialog;
        }
    }
}