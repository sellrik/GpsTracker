using System;
using SQLite;

namespace GpsTracker.Database.Entity
{
    public class NetworkLogEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public bool IsConnected { get; set; }

        public DateTime DateTime { get; set; }

        public string Ssid { get; set; }

        public bool IsUploaded { get; set; } = false;

        public DateTime UploadDateTime { get; set; }
    }
}