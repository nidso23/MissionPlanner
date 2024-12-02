using System;
using GMap.NET;
using GMap.NET.WindowsForms.Markers;

namespace MissionPlanner.Maps
{
    [Serializable]
    public class GMapMarkerWeather : GMarkerGoogle
    {
        public GMapMarkerWeather(PointLatLng p)
            : base(p, GMarkerGoogleType.yellow_small)
        {
        }
    }
}