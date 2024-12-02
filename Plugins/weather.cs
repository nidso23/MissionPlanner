using GMap.NET.WindowsForms;
using MissionPlanner;
using MissionPlanner.Controls;
using MissionPlanner.Maps;
using MissionPlanner.Utilities;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Forms;
using MissionPlanner.GCSViews;
using GMap.NET.WindowsForms.Markers;
using System.Drawing;

namespace weather
{
    public class Weather : MissionPlanner.Plugin.Plugin
    {
        static ObservableCollection<PointLatLngAlt> WeatherPts = new ObservableCollection<PointLatLngAlt>();
        private GMapOverlay _weatheroverlay;
        private GMapOverlay _weatheroverlayFP;

        public override string Name
        {
            get { return "Weather"; }
        }

        public override string Version
        {
            get { return "0"; }
        }

        public override string Author
        {
            get { return "Nicholas Idso"; }
        }

        public override bool Init()
        {
            _weatheroverlay = new GMapOverlay("Weather");
            _weatheroverlay.IsVisibile = true;
            Host.FDGMapControl.Overlays.Add(_weatheroverlay);

            _weatheroverlayFP = new GMapOverlay("Weather");
            _weatheroverlayFP.IsVisibile = true;
            Host.FPGMapControl.Overlays.Add(_weatheroverlayFP);

            return true;
        }

        public override bool Loaded()
        {
            var rootbut = new ToolStripMenuItem("Weather");
            Controls(rootbut, FlightMenu.Data);
            ToolStripItemCollection col = Host.FDMenuMap.Items;
            col.Add(rootbut);

            var rootbutFP = new ToolStripMenuItem("Weather");
            Controls(rootbutFP, FlightMenu.Planner);
            ToolStripItemCollection colFP = Host.FPMenuMap.Items;
            colFP.Add(rootbutFP);

            return true;
        }

        private void Controls(ToolStripMenuItem rootbut, FlightMenu map)
        {
            var but = new ToolStripMenuItem("Wind");
            but.Click += async (s, e) =>
            {
                PointLatLngAlt point = GetLocation(map);

                try
                {
                    WeatherAPI weatherResponse = await GetWeatherData(DateTime.Now, point.Lat, point.Lng, point.Alt, "wind");
                    CustomMessageBox.Show(GetWindData(weatherResponse, point));
                }
                catch
                {
                    CustomMessageBox.Show("Cannot Get Wind Data");
                }
                UpdateOverlay(_weatheroverlay);
                UpdateOverlay(_weatheroverlayFP);
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Ocean Current");
            but.Click += async (s, e) => {
                PointLatLngAlt point = GetLocation(map);

                try
                {
                    WeatherAPI weatherResponse = await GetWeatherData(DateTime.Now, point.Lat, point.Lng, point.Alt, "current");
                    CustomMessageBox.Show(GetCurrentData(weatherResponse, point));
                }
                catch
                {
                    CustomMessageBox.Show("Cannot Get Ocean Current Data");
                }
                UpdateOverlay(_weatheroverlay);
                UpdateOverlay(_weatheroverlayFP);
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Ocean Waves");
            but.Click += async (s, e) => {
                PointLatLngAlt point = GetLocation(map);

                try
                {
                    WeatherAPI weatherResponse = await GetWeatherData(DateTime.Now, point.Lat, point.Lng, point.Alt, "wave");
                    CustomMessageBox.Show(GetWaveData(weatherResponse, point));
                }
                catch
                {
                    CustomMessageBox.Show("Cannot Get Wave Data");
                }
                UpdateOverlay(_weatheroverlay);
                UpdateOverlay(_weatheroverlayFP);
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Delete");
            but.Click += (s, e) => {
                if (FlightData.instance.CurrentGMapMarker != null && FlightData.instance.CurrentGMapMarker is GMapMarkerWeather && map == FlightMenu.Data)
                {
                    WeatherPtDelete((GMapMarkerWeather)FlightData.instance.CurrentGMapMarker);
                    UpdateOverlay(_weatheroverlay);
                    UpdateOverlay(_weatheroverlayFP);
                }
                else if (FlightPlanner.instance.currentMarker != null && FlightPlanner.instance.CurrentMarkerWeather is GMarkerGoogle && map == FlightMenu.Planner)
                {
                    WeatherPtDelete((GMapMarkerWeather)FlightPlanner.instance.CurrentMarkerWeather);
                    UpdateOverlay(_weatheroverlay);
                    UpdateOverlay(_weatheroverlayFP);
                }
            };
            rootbut.DropDownItems.Add(but);
        }

        public static string Url = "http://192.168.110.32:8189/";

        private async Task<WeatherAPI> GetWeatherData(DateTimeOffset dateTime, double latitude, double longitude, double altitude, string model)
        {
            HttpClient client = new HttpClient() { BaseAddress = new Uri(Url) };
            HttpResponseMessage response = await client.GetAsync($"/weatherapi/1.0/weather?latitude={latitude}&longitude={longitude}&altitude={altitude}&time={CreateDate(dateTime)}&model={model}");
            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WeatherAPI>(jsonResponse);
        }

        private string GetWindData(WeatherAPI data, PointLatLngAlt point)
        {
            if (data == null)
            {
                return "No Wind Data";
            }
            double speed = 0;
            double direction = 0;
            double latitude = 0;
            double longitude = 0;
            double altitude = -1;
            foreach (var dictList in data.WeatherRegions.Values)
            {
                foreach (var windData in dictList)
                {
                    if (windData.ContainsKey("NULL"))
                    {
                        return "No Wind Data";
                    }
                    else
                    {
                        try
                        {
                            speed += windData["wind_speed"].ToString() == "NULL" ? -1 : double.Parse(windData["wind_speed"].ToString());
                            direction += windData["wind_from_direction"].ToString() == "NULL" ? -1 : double.Parse(windData["wind_from_direction"].ToString());
                            latitude += windData["latitude"].ToString() == "NULL" ? -1 : double.Parse(windData["latitude"].ToString());
                            longitude += windData["longitude"].ToString() == "NULL" ? -1 : double.Parse(windData["longitude"].ToString());
                            altitude = windData["Altitude"].ToString() == "NULL" ? -1 : double.Parse(windData["Altitude"].ToString());
                        }
                        catch
                        {
                            CustomMessageBox.Show("Cannot Process Wind Data");
                        }
                    }
                }
            }
            speed /= 4;
            direction /= 4;
            latitude /= 4;
            longitude /= 4;

            string info = $"Wind\nlatitude: {latitude} longitude: {longitude} altitdue: {altitude}m\nspeed: {speed}m/s direction: {direction}";
            point.Tag = info;
            WeatherPts.Add(point);
            return info;
        }

        private string GetCurrentData(WeatherAPI data, PointLatLngAlt point)
        {
            if (data == null)
            {
                return "No Ocean Current Data";
            }
            double speed = 0;
            double direction = 0;
            double latitude = 0;
            double longitude = 0;
            double altitude = 999;
            foreach (var dictList in data.WeatherRegions.Values)
            {
                foreach (var windData in dictList)
                {
                    if (windData.ContainsKey("NULL"))
                    {
                        return "No Ocean Current Data";
                    }
                    else
                    {
                        try
                        {
                            speed += windData["water_speed"].ToString() == "NULL" ? -1 : double.Parse(windData["water_speed"].ToString());
                            direction += windData["water_to_direction"].ToString() == "NULL" ? -1 : double.Parse(windData["water_to_direction"].ToString());
                            latitude += windData["latitude"].ToString() == "NULL" ? -1 : double.Parse(windData["latitude"].ToString());
                            longitude += windData["longitude"].ToString() == "NULL" ? -1 : double.Parse(windData["longitude"].ToString());
                            altitude = windData["Altitude"].ToString() == "NULL" ? 999 : double.Parse(windData["Altitude"].ToString());
                        }
                        catch
                        {
                            CustomMessageBox.Show("Cannot Process Ocean Current Data");
                        }
                    }
                }
            }
            speed /= 4;
            direction /= 4;
            latitude /= 4;
            longitude /= 4;

            string info = $"Ocean Current\nlatitude: {latitude} longitude: {longitude} altitdue: {altitude * -1}m\nspeed: {speed}m/s direction: {direction}";
            point.Tag = info;
            WeatherPts.Add(point);
            return info;
        }

        private string GetWaveData(WeatherAPI data, PointLatLngAlt point)
        {
            if (data == null)
            {
                return "No Wave Data";
            }
            double height = 0;
            double direction = 0;
            double latitude = 0;
            double longitude = 0;
            foreach (var dictList in data.WeatherRegions.Values)
            {
                foreach (var windData in dictList)
                {
                    if (windData.ContainsKey("NULL"))
                    {
                        return "No Wave Data";
                    }
                    else
                    {
                        try
                        {
                            height += windData["sea_surface_wind_wave_significant_height"].ToString() == "NULL" ? -1 : double.Parse(windData["sea_surface_wind_wave_significant_height"].ToString());
                            direction += windData["sea_surface_swell_wave_to_direction"].ToString() == "NULL" ? -1 : double.Parse(windData["sea_surface_swell_wave_to_direction"].ToString());
                            latitude += windData["latitude"].ToString() == "NULL" ? -1 : double.Parse(windData["latitude"].ToString());
                            longitude += windData["longitude"].ToString() == "NULL" ? -1 : double.Parse(windData["longitude"].ToString());
                        }
                        catch
                        {
                            CustomMessageBox.Show("Cannot Process Wave Data");
                        }
                    }
                }
            }
            height /= 4;
            direction /= 4;
            latitude /= 4;
            longitude /= 4;

            string info = $"Waves\nlatitude: {latitude} longitude: {longitude}\nheight: {height}m direction: {direction}";
            point.Tag = info;
            WeatherPts.Add(point);
            return info;
        }

        private string CreateDate(DateTimeOffset dateTime)
        {
            string month = LessThan10(dateTime.Month);
            string day = LessThan10(dateTime.Day);
            string hour = LessThan10(dateTime.Hour);
            string minute = LessThan10(dateTime.Minute);
            string second = LessThan10(dateTime.Second);

            return month + "/" + day + "/" + dateTime.Year.ToString() + " " + hour + ":" + minute + ":" + second + " -00:00" ;
        }

        private string LessThan10(int time)
        {
            string timeString = string.Empty;
            if (time < 10)
            {
                timeString = $"0{time}";
            }
            else
            {
                timeString = time.ToString();
            }
            return timeString;
        }

        private PointLatLngAlt GetLocation(FlightMenu map)
        {
            double lat;
            double lng;
            if (map == FlightMenu.Data)
            {
                lat = Host.FDMenuMapPosition.Lat;
                lng = Host.FDMenuMapPosition.Lng;
            }
            else
            {
                lat = Host.FPMenuMapPosition.Lat;
                lng = Host.FPMenuMapPosition.Lng;
            }

            string location = $"{lat};{lng};0";
            double alt = 0;
            InputBox.Show("Enter Coords", "Please enter the coords 'lat;long;alt'", ref location);

            var split = location.Split(';');

            if (split.Length == 3)
            {
                lat = double.Parse(split[0], CultureInfo.InvariantCulture);
                lng = double.Parse(split[1], CultureInfo.InvariantCulture);
                alt = double.Parse(split[2], CultureInfo.InvariantCulture);
            }
            else
            {
                CustomMessageBox.Show(Strings.InvalidField, Strings.ERROR);
            }

            location = $"{lat};{lng};{alt}";

            if (alt < 0)
            {
                alt *= -1;
            }

            PointLatLngAlt point = new PointLatLngAlt
            {
                Alt = alt,
                Lat = lat,
                Lng = lng
            };

            return point;
        }

        private static void UpdateOverlay(GMapOverlay weatheroverlay)
        {
            if (weatheroverlay == null)
                return;
            weatheroverlay.Clear();

            foreach (var pnt in WeatherPts)
            {
                var marker = new GMapMarkerWeather(pnt)
                {
                    ToolTipMode = MarkerTooltipMode.OnMouseOver,
                    ToolTipText = pnt.Tag
                };
                marker.ToolTip.Font = new Font("Arial", 9, FontStyle.Bold);
                weatheroverlay.Markers.Add(marker);
            }
        }

        private static void WeatherPtDelete(GMapMarkerWeather point)
        {
            for (int a = 0; a < WeatherPts.Count; a++)
            {
                if (WeatherPts[a].Point() == point.Position)
                {
                    WeatherPts.RemoveAt(a);
                    return;
                }
            }
        }

        public override bool Loop()
        {
            return true;
        }

        public override bool Exit()
        {
            return true;
        }

        private enum FlightMenu
        {
            Data,
            Planner
        }
    }
    public class WeatherAPI
    {
        [JsonProperty(PropertyName = "Region")]
        public Dictionary<string, List<Dictionary<string, object>>> WeatherRegions = null;
    }
}