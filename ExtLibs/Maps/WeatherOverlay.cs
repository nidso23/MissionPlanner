using System.Collections.Generic;

namespace MissionPlanner.Maps
{
    using System;
    using GMap.NET.Projections;
    using GMap.NET.MapProviders;
    using GMap.NET;
    using System.Reflection;
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Globalization;

    /// <summary>
    /// WeatherOverlay Custom
    /// </summary>
    public class WeatherOverlay : GMapProvider
    {
        public static readonly WeatherOverlay Instance;
        private readonly HttpClient _client = new HttpClient() { BaseAddress = new Uri(URL) };
        public static string URL = "http://192.168.110.32:8111/cog/";

        WeatherOverlay()
        {
            MaxZoom = 24;
        }

        static WeatherOverlay()
        {
            Instance = new WeatherOverlay();

            Type mytype = typeof(GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>)field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);

            Instance.IsSet = false;
            Instance.LoadFromMpaUrl();
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("4574218D-B552-4CAF-89AE-F20951CFDB2" + new Random().Next(0,9).ToString());

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "WeatherOverlay Custom";

        public override string Name
        {
            get { return name; }
        }

        GMapProvider[] overlays;

        public override GMapProvider[] Overlays
        {
            get
            {
                if (overlays == null)
                {
                    overlays = new GMapProvider[] { this };
                }
                return overlays;
            }
        }

        public override PureProjection Projection
        {
            get { return MercatorProjection.Instance; }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            string url = MakeTileImageUrl(pos, zoom, LanguageStr);
            return GetTileImageUsingHttp(url);
        }

        #endregion

        string MakeTileImageUrl(GPoint pos, int zoom, string language)
        {
            if (IsSet)
            {
                var selectedRegion = RegionModelData[Region];
                var selectedModel = selectedRegion[Model];
                string selectedFile = selectedModel.AvailableFiles[Time][0];
                selectedFile = selectedFile.Replace("{level}", Level).Replace("{param}", Parameter);

                return URL + $"tiles/{zoom}/{pos.X}/{pos.Y}.png?url={selectedFile}&bidx=1&colormap_name={Parameter}";
            }

            return string.Format(CultureInfo.InvariantCulture, "https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{0}/{2}/{1}.png", zoom, pos.X, pos.Y);
        }

        private async void LoadFromMpaUrl()
        {
            RegionModelData = new Dictionary<string, Dictionary<string, RegionModelData>>();
            try
            {
                var response = await _client.GetAsync("region");
                response.EnsureSuccessStatusCode();
            }
            catch { }
            Regions regions = await GetRegionsAsync();
            foreach (string region in regions.AvailableRegions)
            {
                ModelsResponse models = await GetModelsAsync(region);
                Dictionary<string, RegionModelData> modelData = new Dictionary<string, RegionModelData>();
                foreach (string model in models.AvailableModels)
                {
                    RegionModelData regionModelData = await GetRegionModelDataAsync(region, model);
                    modelData.Add(model, regionModelData);
                }
                RegionModelData.Add(region, modelData);
            }
        }

        private async Task<Regions> GetRegionsAsync()
        {
            HttpResponseMessage response = await _client.GetAsync("region");
            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Regions>(jsonResponse);
        }

        private async Task<ModelsResponse> GetModelsAsync(string region)
        {
            HttpResponseMessage response = await _client.GetAsync($"model/{region}");
            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ModelsResponse>(jsonResponse);
        }

        private async Task<RegionModelData> GetRegionModelDataAsync(string region, string model)
        {
            HttpResponseMessage response = await _client.GetAsync($"available/{region}/{model}?latest=True");
            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RegionModelData>(jsonResponse);
        }

        public Dictionary<string, Dictionary<string, RegionModelData>> RegionModelData { get; set; }
        public string Time { get; set; }
        public string Level { get; set; }
        public string Parameter { get; set; }
        public string Model { get; set; }
        public string Region { get; set; } 
        public bool IsSet { get; set; }
    }

    public class Regions
    {
        [JsonProperty(PropertyName = "available_regions")]
        public List<string> AvailableRegions = null;
    }

    public class ModelsResponse
    {
        [JsonProperty(PropertyName = "available_models")]
        public List<string> AvailableModels = null;
    }

    public class RegionModelData
    {
        [JsonProperty(PropertyName = "available_times")]
        public List<string> AvailableTimes = null;

        [JsonProperty(PropertyName = "available_parameters")]
        public Dictionary<string, List<int?>> AvailableParams = null;

        [JsonProperty(PropertyName = "available_files")]
        public Dictionary<string, List<string>> AvailableFiles = null;
    }
}