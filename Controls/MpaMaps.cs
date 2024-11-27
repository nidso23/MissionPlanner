using GMap.NET;
using GMap.NET.MapProviders;
using MissionPlanner.GCSViews;
using MissionPlanner.Maps;
using System;
using System.Collections.Generic;
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
        private Button buttonPropagation;
        private Label labelUrl;
        private TextBox TextBoxUrl;
        private Button buttonUrl;

        private bool _buttonLevelEnable = false;
        private bool _buttonTimeEnable = false;

        public MpaMaps()
        {
            InitializeComponent();
            Utilities.ThemeManager.ApplyThemeTo(this);
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
            this.buttonPropagation = new System.Windows.Forms.Button();
            this.labelUrl = new System.Windows.Forms.Label();
            this.TextBoxUrl = new System.Windows.Forms.TextBox();
            this.buttonUrl = new System.Windows.Forms.Button();
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
            this.buttonOkay.Enabled = false;
            this.buttonOkay.Click += new System.EventHandler(this.OnClickOkayButton);
            resources.ApplyResources(this.buttonOkay, "buttonOkay");
            //
            // buttonPropagation
            //
            this.buttonPropagation.Name = "buttonPropagation";
            this.buttonPropagation.Click += new System.EventHandler(this.OnClickPropagationButton);
            resources.ApplyResources(this.buttonPropagation, "buttonPropagation");
            //
            // labelUrl
            //
            resources.ApplyResources(this.labelUrl, "labelUrl");
            this.labelUrl.Name = "labelUrl";
            //
            // TextBoxUrl
            //
            this.TextBoxUrl.Name = "TextBoxUrl";
            this.TextBoxUrl.Text = WeatherOverlay.URL;
            resources.ApplyResources(this.TextBoxUrl, "TextBoxUrl");
            this.TextBoxUrl.TextChanged += new System.EventHandler(this.TextBoxUrl_TextChanged);
            //
            // buttonUrl
            //
            this.buttonUrl.Name = "buttonUrl";
            this.buttonUrl.Click += new System.EventHandler(this.OnClickUrlButton);
            resources.ApplyResources(this.buttonUrl, "buttonUrl");
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
            this.Controls.Add(this.buttonPropagation);
            this.Controls.Add(this.labelUrl);
            this.Controls.Add(this.TextBoxUrl);
            this.Controls.Add(this.buttonUrl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MpaMaps";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void RegionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetModelItems();
            SetParameterItems();
            SetLevelItems();
            SetTimeItems();
        }

        private void ModelBox_SelectedIndexChanged(object sender, EventArgs e)
        {
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
            _buttonLevelEnable = true;
            buttonOkay.Enabled = _buttonLevelEnable && _buttonTimeEnable;
        }

        private void TimeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _buttonTimeEnable = true;
            buttonOkay.Enabled = _buttonLevelEnable && _buttonTimeEnable;
        }

        private void TextBoxUrl_TextChanged(object sender, EventArgs e)
        {
            WeatherOverlay.URL = TextBoxUrl.Text;
            WeatherOverlay.Instance.LoadFromMpaUrl();
        }

        private void OnClickOkayButton(object sender, EventArgs e)
        {
            if (RegionBox.Text == string.Empty ||
                ModelBox.Text == string.Empty ||
                ParameterBox.Text == string.Empty ||
                LevelBox.Text == string.Empty ||
                TimeBox.Text == string.Empty)
            {
                return;
            }
            foreach (var map in GMapProviders.List)
            {
                if (map.Name == "WeatherOverlay Custom")
                {
                    GMapProviders.List.Remove(map);
                    break;
                }
            }
            WeatherOverlay.Instance.Region = RegionBox.Text;
            WeatherOverlay.Instance.Model = ModelBox.Text;
            WeatherOverlay.Instance.Parameter = ParameterBox.Text;
            WeatherOverlay.Instance.Level = LevelBox.Text;
            WeatherOverlay.Instance.Time = TimeBox.Text;
            WeatherOverlay.Instance.IsSet = true;
            GMapProviders.List.Add(WeatherOverlay.Instance);
            FlightPlanner.instance.comboBoxMapType.DataSource = GMapProviders.List.ToArray();
            int index = FlightPlanner.instance.comboBoxMapType.FindString("WeatherOverlay");
            if (index != -1)
            {
                FlightPlanner.instance.comboBoxMapType.SelectedIndex = index;

                //Clear memory cache and force map reload
                GMaps.Instance.MemoryCache.Clear();
                FlightPlanner.instance.MainMap.Core.ReloadMap();
                FlightData.mymap.Core.ReloadMap();
                FlightPlanner.instance.MainMap.Refresh();
                FlightData.mymap.Refresh();
            }
            this.Close();
        }

        private void OnClickPropagationButton(object sender, EventArgs e)
        {
            new PropagationSettings().Show();
        }

        private void OnClickUrlButton(object sender, EventArgs e)
        {
            if (WeatherOverlay.Instance.RegionModelData.Count == 0)
            {
                CustomMessageBox.Show("Unable to connect");
            }
            SetRegionItems();
            SetModelItems();
            SetParameterItems();
            SetLevelItems();
            SetTimeItems();
        }

        private void SetRegionItems()
        {
            RegionBox.Items.Clear();
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
            RegionBox.Items.AddRange(regionObject);
            if (regions.Count > 0)
            {
                RegionBox.Text = regions[0];
            }
        }

        private void SetModelItems()
        {
            ModelBox.Items.Clear();
            if (WeatherOverlay.Instance.RegionModelData.Count == 0)
            {
                return;
            }
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
            ModelBox.Items.AddRange(modelObject);
            if (models.Count > 0)
            {
                ModelBox.Text = models[0];
            }
        }

        private void SetParameterItems()
        {
            ParameterBox.Items.Clear();
            if (WeatherOverlay.Instance.RegionModelData.Count == 0)
            {
                return;
            }
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
            _buttonLevelEnable = false;
            _buttonTimeEnable = false;
            buttonOkay.Enabled = false;

            LevelBox.Items.Clear();
            if (WeatherOverlay.Instance.RegionModelData.Count == 0)
            {
                return;
            }
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
            if (WeatherOverlay.Instance.RegionModelData.Count == 0)
            {
                return;
            }
            var times = WeatherOverlay.Instance.RegionModelData[RegionBox.Text][ModelBox.Text].AvailableTimes;

            object[] timeObject = new object[times.Count];
            for (int i = 0; i < timeObject.Length; i++)
            {
                timeObject[i] = times[i].ToString();
            }
            TimeBox.Items.AddRange(timeObject);
        }
    }
}