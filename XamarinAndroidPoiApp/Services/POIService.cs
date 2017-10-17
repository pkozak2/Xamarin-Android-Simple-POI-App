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
using Newtonsoft.Json.Serialization;

namespace XamarinAndroidPoiApp.Services
{
    class POIService
    {
        private const string GET_POIS = "http://private-e451d-poilist.apiary-mock.com/com.packt.poiapp/api/poi/pois";
        private const string CREATE_POI = "http://private-e451d-poilist.apiary-mock.com/com.packt.poiapp/api/poi/create";
        private const string DELETE_POI = "http://private-e451d-poilist.apiary-mock.com/com.packt.poiapp/api/poi/delete/{0}";
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

        public async Task<string> CreateOrUpdatePOIAsync(PointOfInterest poi, Activity activity)
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new POIContractResolver();
            var poiJson = JsonConvert.SerializeObject(poi, Formatting.Indented, settings);

            HttpClient httpClient = new HttpClient();
            StringContent jsonContent = new StringContent(poiJson, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(CREATE_POI, jsonContent);
            if (response != null || response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                Console.Out.WriteLine("{0} saved.", poi.Name);
                return content;
            }
            return null;
        }

        public async Task<String> DeletePOIAsync(int poiId)
        {
            HttpClient httpClient = new HttpClient();
            String url = String.Format(DELETE_POI, poiId);
            HttpResponseMessage response = await httpClient.DeleteAsync(url);
            if (response != null || response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                Console.Out.WriteLine("One record deleted.");
                return content;
            }
            return null;
        }

        public bool isConnected(Context activity)
        {
            var connectivityManager = (ConnectivityManager) activity.GetSystemService(Context.ConnectivityService);
            var activeConnection = connectivityManager.ActiveNetworkInfo;
            return (null != activeConnection && activeConnection.IsConnected);
        }

        public class POIContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string key)
            {
                return key.ToLower();
            }
        }
    }
}