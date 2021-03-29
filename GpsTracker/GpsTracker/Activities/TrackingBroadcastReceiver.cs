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
        private Action trackingStarted;
        private Action trackingStopped;

        public TrackingBroadcastReceiver(Action<LocationChangedModel> locationChanged, Action trackingStarted, Action trackingStopped)
        {
            this.locationChanged = locationChanged;
            this.trackingStarted = trackingStarted;
            this.trackingStopped = trackingStopped;
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
                case "TrackingStarted":
                    {
                        trackingStarted();
                        break;
                    }
                case "TrackingStopped":
                    {
                        trackingStopped();
                        break;

                    }

                default:
                    break;
            }
        }
    }
}