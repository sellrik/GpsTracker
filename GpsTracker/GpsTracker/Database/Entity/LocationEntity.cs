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
using SQLite;

namespace GpsTracker.Database
{
    public class LocationEntity
    {
        private static DateTime UnixDateTimeStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Provider { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public float Accuracy { get; set; }

        public double Altitude { get; set; }

        public float Speed { get; set; }

        public long Time { get; set; }

        public DateTime DateTime { get; set; }

        public bool IsUploaded { get; set; } = false;

        public DateTime UploadDateTime { get; set; }

        public LocationEntity()
        {

        }

        public LocationEntity(string provider, double longitude, double latitude, float accuracy, double altitude, float speed, long time)
        {
            Provider = provider;
            Longitude = longitude;
            Latitude = latitude;
            Accuracy = accuracy;
            Altitude = altitude;
            Speed = speed;
            Time = time;
            DateTime = GetDateTimeFromUnixTime(time);
            IsUploaded = false;
        }

        private static DateTime GetDateTimeFromUnixTime(long ticks)
        {
            return UnixDateTimeStart.AddMilliseconds(ticks);
        }
    }
}