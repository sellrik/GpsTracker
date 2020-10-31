
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
    public class LocationUploadModel
    {
        public List<LocationEntity> Locations { get; set; }
    }
}