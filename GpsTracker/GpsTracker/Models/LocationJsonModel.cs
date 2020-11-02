using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GpsTracker.Database;

namespace GpsTracker.Models
{
    public class LocationJsonModel
    {
        public LocationJsonModel(LocationEntity entity)
        {
            Id = entity.Id;
            Provider = entity.Provider;
            Longitude = entity.Longitude;
            Latitude = entity.Latitude;
            Accuracy = entity.Accuracy;
            Altitude = entity.Altitude;
            Speed = entity.Speed;
            Time = entity.Time;
            DateTime = entity.DateTime;
        }

        public int Id { get; set; }

        public string Provider { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public float Accuracy { get; set; }

        public double Altitude { get; set; }

        public float Speed { get; set; }

        public long Time { get; set; }

        public DateTime DateTime { get; set; }
    }
}