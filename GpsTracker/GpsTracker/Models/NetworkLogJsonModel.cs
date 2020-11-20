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
using GpsTracker.Database.Entity;
using Newtonsoft.Json;

namespace GpsTracker.Models
{
    public class NetworkLogJsonModel
    {
        public int Id { get; set; }

        public bool IsConnected { get; set; }

        public DateTime DateTime { get; set; }

        public long Time
        {
            get
            {
                return ((DateTimeOffset)DateTime).ToUnixTimeMilliseconds();
            }
        }

        [JsonProperty("SSID")]
        public string Ssid { get; set; }

        public int HashCode { get; set; }

        public NetworkLogJsonModel(NetworkLogEntity entity)
        {
            Id = entity.Id;
            IsConnected = entity.IsConnected;
            DateTime = DateTime.SpecifyKind(entity.DateTime, DateTimeKind.Utc);
            Ssid = entity.Ssid;
            HashCode = entity.HashCode;
        }
    }
}