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

namespace weather
{
    public class Weather : MissionPlanner.Plugin.Plugin
    {
        static ObservableCollection<PointLatLngAlt> WeatherPts = new ObservableCollection<PointLatLngAlt>();
        private GMapOverlay weatheroverlay;

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
            weatheroverlay = new GMapOverlay("Weather");
            weatheroverlay.IsVisibile = true;
            Host.FDGMapControl.Overlays.Add(weatheroverlay);

            return true;
        }

        public override bool Loaded()
        {
            var rootbut = new ToolStripMenuItem("Weather");
            ToolStripItemCollection col = Host.FDMenuMap.Items;
            col.Add(rootbut);

            var but = new ToolStripMenuItem("Wind");
            but.Click += async (s, e) =>
            {
                PointLatLngAlt point = GetLocation();
                
                try
                {
                    HttpResponseMessage response = await GetWeatherData(DateTime.Now, point.Lat, point.Lng, point.Alt, "wind");
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    WeatherAPI weatherResponse = JsonConvert.DeserializeObject<WeatherAPI>(jsonResponse);
                    MessageBox.Show(GetWindData(weatherResponse, point));
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show(Strings.ERROR, ex.Message);
                }
                UpdateOverlay(weatheroverlay);
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Ocean Current");
            but.Click += async (s, e) => {
                PointLatLngAlt point = GetLocation();
                
                try
                {
                    HttpResponseMessage response = await GetWeatherData(DateTime.Now, point.Lat, point.Lng, point.Alt, "current");
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    WeatherAPI weatherResponse = JsonConvert.DeserializeObject<WeatherAPI>(jsonResponse);
                    MessageBox.Show(GetCurrentData(weatherResponse, point));
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show(Strings.ERROR, ex.Message);
                }
                UpdateOverlay(weatheroverlay);
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Ocean Waves");
            but.Click += async (s, e) => {
                PointLatLngAlt point = GetLocation();

                try
                {
                    HttpResponseMessage response = await GetWeatherData(DateTime.Now, point.Lat, point.Lng, point.Alt, "wave");
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    WeatherAPI weatherResponse = JsonConvert.DeserializeObject<WeatherAPI>(jsonResponse);
                    MessageBox.Show(GetWaveData(weatherResponse, point));
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show(Strings.ERROR, ex.Message);
                }
                UpdateOverlay(weatheroverlay);
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Delete");
            but.Click += (s, e) => {
                WeatherPtDelete((GMapMarkerWeather)FlightData.instance.CurrentGMapMarker);
                UpdateOverlay(weatheroverlay);
            };
            rootbut.DropDownItems.Add(but);

            return true;
        }

        private readonly HttpClient _client = new HttpClient() { BaseAddress = new Uri("http://192.168.110.32:8189/") };

        public async Task<HttpResponseMessage> GetWeatherData(DateTimeOffset dateTime, double latitude, double longitude, double altitude, string model)
            => await _client.GetAsync($"/weatherapi/1.0/weather?latitude={latitude}&longitude={longitude}&altitude={altitude}&time={CreateDate(dateTime)}&model={model}");

        private string GetWindData(WeatherAPI data, PointLatLngAlt point)
        {
            if (data == null)
            {
                return string.Empty;
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
                        return string.Empty;
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
                            CustomMessageBox.Show(Strings.ERROR, "Something Went Wrong");
                        }
                    }
                }
            }
            speed /= 4;
            direction /= 4;
            latitude /= 4;
            longitude /= 4;

            string info = $"Wind\nlatitude: {latitude} longitude: {longitude} altitdue: {altitude}\nspeed: {speed} direction: {direction}";
            point.Tag = info;
            WeatherPts.Add(point);
            return info;
        }

        private string GetCurrentData(WeatherAPI data, PointLatLngAlt point)
        {
            if (data == null)
            {
                return string.Empty;
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
                        return string.Empty;
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
                            CustomMessageBox.Show(Strings.ERROR, "Something Went Wrong");
                        }
                    }
                }
            }
            speed /= 4;
            direction /= 4;
            latitude /= 4;
            longitude /= 4;

            string info = $"Ocean Current\nlatitude: {latitude} longitude: {longitude} altitdue: {altitude * -1}\nspeed: {speed} direction: {direction}";
            point.Tag = info;
            WeatherPts.Add(point);
            return info;
        }

        private string GetWaveData(WeatherAPI data, PointLatLngAlt point)
        {
            if (data == null)
            {
                return string.Empty;
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
                        return string.Empty;
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
                            CustomMessageBox.Show(Strings.ERROR, "Something Went Wrong");
                        }
                    }
                }
            }
            height /= 4;
            direction /= 4;
            latitude /= 4;
            longitude /= 4;

            string info = $"Waves\nlatitude: {latitude} longitude: {longitude}\nheight: {height} direction: {direction}";
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

        private PointLatLngAlt GetLocation()
        {
            double lat = Host.FDMenuMapPosition.Lat;
            double lng = Host.FDMenuMapPosition.Lng;

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

        public static void UpdateOverlay(GMapOverlay weatheroverlay)
        {
            if (weatheroverlay == null)
                return;
            weatheroverlay.Clear();

            foreach (var pnt in WeatherPts)
            {
                weatheroverlay.Markers.Add(new GMapMarkerWeather(pnt)
                {
                    ToolTipMode = MarkerTooltipMode.OnMouseOver,
                    ToolTipText = pnt.Tag
                });
            }
        }

        public static void WeatherPtDelete(GMapMarkerWeather point)
        {
            if (point == null || !(FlightData.instance.CurrentGMapMarker is GMapMarkerWeather))
                return;

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
    }
    public class WeatherAPI
    {
        [JsonProperty(PropertyName = "Region")]
        public Dictionary<string, List<Dictionary<string, object>>> WeatherRegions = null;
    }
}