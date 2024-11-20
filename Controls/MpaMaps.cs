using GMap.NET.MapProviders;
using log4net.Core;
using MissionPlanner.GCSViews;
using MissionPlanner.Maps;
using MissionPlanner.Utilities;
using Newtonsoft.Json;
using RFDLib.IO.ATCommand;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public class MpaMaps : Form
    {
        private Label labelRegion;
        private ComboBox RegionBox;
        private Label labelModel;
        private ComboBox ModelBox;
        private Label labelParameter;
        private ComboBox ParameterBox;
        private Label labelLevel;
        private ComboBox LevelBox;
        private Label labelTime;
        private ComboBox TimeBox;
        private Button buttonOkay;
        //public static string URL = "http://192.168.110.32:8111/cog/";
        //private readonly HttpClient _client = new HttpClient() { BaseAddress = new Uri(URL) };
        //private Dictionary<string, Dictionary<string, RegionModelData>> _data;
        //private List<string> _availableRegions;

        public MpaMaps()
        {
            //LoadFromMpaUrl();
            InitializeComponent();

            Utilities.ThemeManager.ApplyThemeTo(this);

            //RegionBox.Text = Settings.Instance.GetInt32("RegionBox", 1).ToString();
            //ModelBox.Text = Settings.Instance.GetInt32("ModelBox", 1).ToString();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MpaMaps));
            this.labelRegion = new System.Windows.Forms.Label();
            this.RegionBox = new System.Windows.Forms.ComboBox();
            this.labelModel = new System.Windows.Forms.Label();
            this.ModelBox = new System.Windows.Forms.ComboBox();
            this.labelParameter = new System.Windows.Forms.Label();
            this.ParameterBox = new System.Windows.Forms.ComboBox();
            this.labelLevel = new System.Windows.Forms.Label();
            this.LevelBox = new System.Windows.Forms.ComboBox();
            this.labelTime = new System.Windows.Forms.Label();
            this.TimeBox = new System.Windows.Forms.ComboBox();
            this.buttonOkay = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // labelRegion
            //
            resources.ApplyResources(this.labelRegion, "labelRegion");
            this.labelRegion.Name = "labelRegion";
            //
            // RegionBox
            //
            this.RegionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RegionBox.FormattingEnabled = true;
            SetRegionItems();
            resources.ApplyResources(this.RegionBox, "RegionBox");
            this.RegionBox.Name = "RegionBox";
            this.RegionBox.SelectedIndexChanged += new System.EventHandler(this.RegionBox_SelectedIndexChanged);
            //
            // labelModel
            //
            resources.ApplyResources(this.labelModel, "labelModel");
            this.labelModel.Name = "labelModel";
            //
            // ModelBox
            //
            this.ModelBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ModelBox.FormattingEnabled = true;
            SetModelItems();
            resources.ApplyResources(this.ModelBox, "ModelBox");
            this.ModelBox.Name = "ModelBox";
            this.ModelBox.SelectedIndexChanged += new System.EventHandler(this.ModelBox_SelectedIndexChanged);
            //
            // labelParameter
            //
            resources.ApplyResources(this.labelParameter, "labelParameter");
            this.labelParameter.Name = "labelParameter";
            //
            // ParameterBox
            //
            this.ParameterBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ParameterBox.FormattingEnabled = true;
            SetParameterItems();
            resources.ApplyResources(this.ParameterBox, "ParameterBox");
            this.ParameterBox.Name = "ParameterBox";
            this.ParameterBox.SelectedIndexChanged += new System.EventHandler(this.ParameterBox_SelectedIndexChanged);
            //
            // labelLevel
            //
            resources.ApplyResources(this.labelLevel, "labelLevel");
            this.labelLevel.Name = "labelLevel";
            //
            // LevelBox
            //
            this.LevelBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LevelBox.FormattingEnabled = true;
            SetLevelItems();
            resources.ApplyResources(this.LevelBox, "LevelBox");
            this.LevelBox.Name = "LevelBox";
            this.LevelBox.SelectedIndexChanged += new System.EventHandler(this.LevelBox_SelectedIndexChanged);
            //
            // labelTime
            //
            resources.ApplyResources(this.labelTime, "labelTime");
            this.labelTime.Name = "labelTime";
            //
            // TimeBox
            //
            this.TimeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TimeBox.FormattingEnabled = true;
            SetTimeItems();
            resources.ApplyResources(this.TimeBox, "TimeBox");
            this.TimeBox.Name = "TimeBox";
            this.TimeBox.SelectedIndexChanged += new System.EventHandler(this.TimeBox_SelectedIndexChanged);
            //
            // buttonOkay
            //
            this.buttonOkay.Name = "buttonOkay";
            this.buttonOkay.Click += new System.EventHandler(this.OnClickOkayButton);
            resources.ApplyResources(this.buttonOkay, "buttonOkay");
            //
            // MpaMaps
            //
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.labelRegion);
            this.Controls.Add(this.RegionBox);
            this.Controls.Add(this.labelModel);
            this.Controls.Add(this.ModelBox);
            this.Controls.Add(this.labelParameter);
            this.Controls.Add(this.ParameterBox);
            this.Controls.Add(this.labelLevel);
            this.Controls.Add(this.LevelBox);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.TimeBox);
            this.Controls.Add(this.buttonOkay);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MpaMaps";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void RegionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Settings.Instance["RegionBox"] = RegionBox.Text;

            SetModelItems();
            SetParameterItems();
            SetLevelItems();
            SetTimeItems();
        }

        private void ModelBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Settings.Instance["ModelBox"] = ModelBox.Text;

            SetParameterItems();
            SetLevelItems();
            SetTimeItems();
        }

        private void ParameterBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetLevelItems();
        }

        private void LevelBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void TimeBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void OnClickOkayButton(object sender, EventArgs e)
        {
            WeatherOverlay.Instance.Region = RegionBox.Text;
            WeatherOverlay.Instance.Model = ModelBox.Text;
            WeatherOverlay.Instance.Parameter = ParameterBox.Text;
            WeatherOverlay.Instance.Level = LevelBox.Text;
            WeatherOverlay.Instance.Time = TimeBox.Text;
            WeatherOverlay.Instance.IsSet = true;
            FlightPlanner.instance.MainMap.MapProvider = WeatherOverlay.Instance;
            FlightData.mymap.MapProvider = WeatherOverlay.Instance;
            this.Close();
        }

        private void SetRegionItems()
        {
            List<string> regions = new List<string>();
            foreach (var key in WeatherOverlay.Instance.RegionModelData.Keys)
            {
                regions.Add(key);
            }
            object[] regionObject = new object[WeatherOverlay.Instance.RegionModelData.Count];
            for (int i = 0; i < regionObject.Length; i++)
            {
                regionObject[i] = regions[i];
            }
            this.RegionBox.Items.AddRange(regionObject);
            this.RegionBox.Text = regions[0];
        }

        private void SetModelItems()
        {
            List<string> models = new List<string>();
            foreach (var key in WeatherOverlay.Instance.RegionModelData[RegionBox.Text].Keys)
            {
                models.Add(key);
            }
            object[] modelObject = new object[WeatherOverlay.Instance.RegionModelData[RegionBox.Text].Count];
            for (int i = 0; i < modelObject.Length; i++)
            {
                modelObject[i] = models[i];
            }
            ModelBox.Items.Clear();
            ModelBox.Items.AddRange(modelObject);
            ModelBox.Text = models[0];
        }

        private void SetParameterItems()
        {
            List<string> parameters = new List<string>();
            foreach (var key in WeatherOverlay.Instance.RegionModelData[RegionBox.Text][ModelBox.Text].AvailableParams.Keys)
            {
                parameters.Add(key);
            }
            object[] paramObject = new object[WeatherOverlay.Instance.RegionModelData[RegionBox.Text][ModelBox.Text].AvailableParams.Count];
            for (int i = 0; i < paramObject.Length; i++)
            {
                paramObject[i] = parameters[i];
            }
            ParameterBox.Items.Clear();
            ParameterBox.Items.AddRange(paramObject);
            if (parameters.Count == 0)
            {
                ParameterBox.Text = "Not Available";
            }
            else
            {
                ParameterBox.Text = parameters[0];
            }
        }

        private void SetLevelItems()
        {
            LevelBox.Items.Clear();
            if (ParameterBox.Items.Count > 0)
            {
                var levels = WeatherOverlay.Instance.RegionModelData[RegionBox.Text][ModelBox.Text].AvailableParams[ParameterBox.Text];

                object[] levelObject = new object[levels.Count];
                for (int i = 0; i < levelObject.Length; i++)
                {
                    levelObject[i] = levels[i].ToString();
                }
                LevelBox.Items.AddRange(levelObject);
            }
            else
            {
                LevelBox.Text = "Not Available";
            }
        }

        private void SetTimeItems()
        {
            TimeBox.Items.Clear();
            var times = WeatherOverlay.Instance.RegionModelData[RegionBox.Text][ModelBox.Text].AvailableTimes;

            object[] timeObject = new object[times.Count];
            for (int i = 0; i < timeObject.Length; i++)
            {
                timeObject[i] = times[i].ToString();
            }
            TimeBox.Items.AddRange(timeObject);
        }

        //public async void LoadFromMpaUrl()
        //{
        //    _data = new Dictionary<string, Dictionary<string, RegionModelData>>();
        //    try
        //    {
        //        var response = await _client.GetAsync("region");
        //        //response.EnsureSuccessStatusCode();
        //    }
        //    catch { }
        //    Regions regions = await GetRegionsAsync();
        //    _availableRegions = regions.AvailableRegions;
        //    foreach (string region in regions.AvailableRegions)
        //    {
        //        ModelsResponse models = await GetModelsAsync(region);
        //        Dictionary<string, RegionModelData> modelData = new Dictionary<string, RegionModelData>();
        //        foreach (string model in models.AvailableModels)
        //        {
        //            RegionModelData regionModelData = await GetRegionModelDataAsync(region, model);
        //            modelData.Add(model, regionModelData);
        //        }
        //        _data.Add(region, modelData);
        //    }
        //}

        //private async Task<Regions> GetRegionsAsync()
        //{
        //    HttpResponseMessage response = await _client.GetAsync("region");
        //    string jsonResponse = await response.Content.ReadAsStringAsync();
        //    return JsonConvert.DeserializeObject<Regions>(jsonResponse);
        //}

        //private async Task<ModelsResponse> GetModelsAsync(string region)
        //{
        //    HttpResponseMessage response = await _client.GetAsync($"model/{region}");
        //    string jsonResponse = await response.Content.ReadAsStringAsync();
        //    return JsonConvert.DeserializeObject<ModelsResponse>(jsonResponse);
        //}

        //private async Task<RegionModelData> GetRegionModelDataAsync(string region, string model)
        //{
        //    HttpResponseMessage response = await _client.GetAsync($"available/{region}/{model}?latest=True");
        //    string jsonResponse = await response.Content.ReadAsStringAsync();
        //    return JsonConvert.DeserializeObject<RegionModelData>(jsonResponse);
        //}
    }

    //public class Regions
    //{
    //    [JsonProperty(PropertyName = "available_regions")]
    //    public List<string> AvailableRegions = null;
    //}

    //public class ModelsResponse
    //{
    //    [JsonProperty(PropertyName = "available_models")]
    //    public List<string> AvailableModels = null;
    //}

    //public class RegionModelData
    //{
    //    [JsonProperty(PropertyName = "available_times")]
    //    public List<string> AvailableTimes = null;

    //    [JsonProperty(PropertyName = "available_parameters")]
    //    public Dictionary<string, List<int?>> AvailableParams = null;

    //    [JsonProperty(PropertyName = "available_files")]
    //    public Dictionary<string, List<string>> AvailableFiles = null;
    //}
}