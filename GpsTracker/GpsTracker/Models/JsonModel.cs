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

namespace GpsTracker.Models
{
    public class JsonModel
    {
        public List<LocationJsonModel> Locations { get; set; }

        public List<NetworkLogJsonModel> NetworkLogs { get; set; }

        public JsonModel()
        {

        }
    }
}