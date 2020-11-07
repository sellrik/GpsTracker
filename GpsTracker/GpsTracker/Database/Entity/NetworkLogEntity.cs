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

namespace GpsTracker.Database.Entity
{
    public class NetworkLogEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public bool IsConnected { get; set; }

        public DateTime DateTime { get; set; }

        public bool IsUploaded { get; set; } = false;

        public DateTime UploadDateTime { get; set; }
    }
}