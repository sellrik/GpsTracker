using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GpsTracker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GpsTracker.Activities
{
    public class TrackingBroadcastReceiver : BroadcastReceiver
    {
        private Action<LocationChangedModel> locationChanged;

        public TrackingBroadcastReceiver(Action<LocationChangedModel> locationChanged)
        {
            this.locationChanged = locationChanged;
        }


        public override void OnReceive(Context context, Intent intent)
        {
            switch (intent.Action)
            {
                case "LocationChanged":
                    {
                        var data = intent.GetStringExtra("Location");
                        var location = JsonConvert.DeserializeObject<LocationChangedModel>(data);
                        locationChanged(location);

                        break;
                    }
                default:
                    break;
            }
        }
    }
}