using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GpsTracker.Models
{
    public class LocationChangedModel
    {

        public LocationChangedModel()
        {

        }

        public LocationChangedModel(Location location)
        {
            Longitude = location.Longitude;
            Latitude = location.Latitude;
            Time = location.Time;
        }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public long Time { get; set; }
    }
}