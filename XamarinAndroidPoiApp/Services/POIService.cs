using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using XamarinAndroidPoiApp.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace XamarinAndroidPoiApp.Services
{
    class POIService
    {
        private const string GET_POIS = "http://private-e451d-poilist.apiary-mock.com/com.packt.poiapp/api/poi/pois";
        private List<PointOfInterest> poiListData = null;

        public async Task<List<PointOfInterest>> GetPOIListAsync()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await httpClient.GetAsync(GET_POIS);
            
            if (response != null || response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                Console.Out.WriteLine("Response Body: \r\n {0}", content);
                poiListData = new List<PointOfInterest>();
                JObject jsonResponse = JObject.Parse(content);

                IList<JToken> results = jsonResponse["pois"].ToList();
                foreach (JToken token in results)
                {
                    PointOfInterest poi = token.ToObject<PointOfInterest>();
                    poiListData.Add(poi);
                }
                return poiListData;

            }
            else
            {
                Console.Out.WriteLine("Failed to fetch data. Try again later!");
                return null;
            }
        }

        public bool isConnected(Context activity)
        {
            var connectivityManager = (ConnectivityManager) activity.GetSystemService(Context.ConnectivityService);
            var activeConnection = connectivityManager.ActiveNetworkInfo;
            return (null != activeConnection && activeConnection.IsConnected);
        }
    }
}