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

    /// <summary>
    /// WeatherOverlay Custom
    /// </summary>
    public class WeatherOverlay : GMapProvider
    {
        public static readonly WeatherOverlay Instance;
        private readonly HttpClient _client = new HttpClient() { BaseAddress = new Uri(URL) };
        private Dictionary<string, Dictionary<string, RegionModelData>> _data;

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

            Instance.LoadFromMpaUrl();
        }

        #region GMapProvider Members

        readonly Guid id = new Guid("4574218D-B552-4CAF-89AE-F20951CFDB2A");

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
            string file0 = string.Empty;
            var region = _data["global_surface"];
            var model = region["gfs_atmo"];
            foreach (var file in model.AvailableFiles.Values)
            {
                file0 = file[0];
            }
            file0 = file0.Replace("{level}", "0").Replace("{param}", "wind_speed");

            return URL + $"tiles/{zoom}/{pos.X}/{pos.Y}.png?url={file0}&bidx=1&colormap_name=wind_speed";
        }

        public static string URL = "http://192.168.110.32:8111/cog/";

        public async void LoadFromMpaUrl()
        {
            _data = new Dictionary<string, Dictionary<string, RegionModelData>>();
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
                _data.Add(region, modelData);
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