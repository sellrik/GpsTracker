using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GpsTracker.Database.Entity;
using GpsTracker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GpsTracker.Activities
{
    public class TracksBroadcastReceiver : BroadcastReceiver
    {
        private Action<TrackEntity> trackingStopped;

        public TracksBroadcastReceiver(Action<TrackEntity> trackingStopped)
        {
            this.trackingStopped = trackingStopped;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            switch (intent.Action)
            {
                case "TrackingStopped":
                    {
                        var data = intent.GetStringExtra("TrackEntity");
                        var entity = JsonConvert.DeserializeObject<TrackEntity>(data);
                        trackingStopped(entity);

                        break;
                    }
                default:
                    break;
            }
        }
    }
}