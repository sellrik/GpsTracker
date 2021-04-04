using Android;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Fragment.App;
using GpsTracker.Database.Entity;
using GpsTracker.Models;
using GpsTracker.Services;
using Newtonsoft.Json;
using System;
using System.Globalization;

namespace GpsTracker.Activities
{
    public class TrackingFragment : Fragment
    {
        const string _startStopButtonStartedText = "STOP";
        const string _startStopButtonStoppedText = "START";
        const int AccessFineLocationPermissionCode = 123;
        bool _accessFineLocationPermissionGranted = false;

        private bool isStarted = false;

        private DateTime startTime;

        private double distance;

        private LocationChangedModel currentModel;

        private TrackingBroadcastReceiver _trackingBroadcastReceiver;
        private LocalBroadcastManager LocalBroadcastManager => LocalBroadcastManager.GetInstance(Context);

        private TextView textViewTrackingCoordinates;
        private TextView textViewTrackingDistance;
        private TextView textViewTrackingDuration;
        private TextView textViewTrackingStarted;
        private Button _startStopButton;

        private TrackService _trackService;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here

            _trackService = new TrackService();
        }

        public override void OnResume()
        {
            base.OnResume();
            RegisterBroadcastReceiver();
            SetStartStopButtonText(BackgroundLocationService.IsStarted);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            UnRegisterBroadcastReceiver();
            StopService();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_tracking, container, false);

            textViewTrackingCoordinates = view.FindViewById<TextView>(Resource.Id.textViewTrackingCoordinates);
            textViewTrackingCoordinates.Text = string.Empty;

            textViewTrackingDistance = view.FindViewById<TextView>(Resource.Id.textViewTrackingDistance);
            textViewTrackingDistance.Text = string.Empty;

            textViewTrackingDuration = view.FindViewById<TextView>(Resource.Id.textViewTrackingDuration);
            textViewTrackingDuration.Text = string.Empty;

            textViewTrackingStarted = view.FindViewById<TextView>(Resource.Id.textViewTrackingStarted);
            textViewTrackingStarted.Text = string.Empty;

            _startStopButton = view.FindViewById<Button>(Resource.Id.buttonStartStop);
            _startStopButton.Click += startStopButton_Click;

            return view;
        }

        private void startStopButton_Click(object sender, EventArgs e)
        {
            try
            {
                _startStopButton.Enabled = false;

                if (_startStopButton.Text == _startStopButtonStoppedText) // Stopped
                {
                    StartLocationService();
                    SetStartStopButtonText(true);
                }
                else // Started
                {
                    StopService();

                    SetStartStopButtonText(false);
                }
            }
            finally
            {
                _startStopButton.Enabled = true;
            }
        }

        private void StartLocationService()
        {
            CheckPermission();

            if (!HasPermission())
            {
                return;
            }

            StartService();

            TrackingStarted();
        }

        void StartService()
        {
            var intent = new Intent(Context, typeof(BackgroundLocationService));

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                Activity.StartForegroundService(intent);
            }
            else
            {
                Activity.StartService(intent);
            }
        }

        void StopService()
        {
            var stopTime = DateTime.UtcNow;

            var intent = new Intent(Context, typeof(BackgroundLocationService));
            Activity.StopService(intent);

            TrackingStopped(stopTime);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == AccessFineLocationPermissionCode)
            {
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {
                    _accessFineLocationPermissionGranted = true;
                }
                else
                {
                    _accessFineLocationPermissionGranted = false;
                }
            }
        }

        public bool CheckPermission()
        {
            if (ContextCompat.CheckSelfPermission(Context, Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                _accessFineLocationPermissionGranted = true;
            }
            else
            {
                Toast.MakeText(Context, "'AccessFineLocation' permission is missing!", ToastLength.Short).Show();

                ActivityCompat.RequestPermissions(Activity, new[] { Manifest.Permission.AccessFineLocation }, AccessFineLocationPermissionCode);

                return false;
            }

            return true;
        }

        public bool HasPermission()
        {
            if (_accessFineLocationPermissionGranted)
            {
                return true;
            }

            return false;
        }

        private void SetStartStopButtonText(bool isServiceStarted)
        {
            if (isServiceStarted)
            {
                _startStopButton.Text = _startStopButtonStartedText;
            }
            else
            {
                _startStopButton.Text = _startStopButtonStoppedText;
            }

        }

        private void RegisterBroadcastReceiver()
        {
            if (_trackingBroadcastReceiver == null)
            {
                var intentFilter = new IntentFilter();
                intentFilter.AddAction("LocationChanged");

                _trackingBroadcastReceiver = new TrackingBroadcastReceiver(LocationChanged);

                LocalBroadcastManager.RegisterReceiver(_trackingBroadcastReceiver, intentFilter);
            }
        }

        private void UnRegisterBroadcastReceiver()
        {
            if (_trackingBroadcastReceiver != null)
            {
                LocalBroadcastManager.UnregisterReceiver(_trackingBroadcastReceiver);
                _trackingBroadcastReceiver = null;
            }
        }

        private void LocationChanged(LocationChangedModel model)
        {
            var dateTime = GetDateTime(model.Time);

            if (!isStarted)
            {
                isStarted = true;
                startTime = dateTime;
            }

            var coordinatesText = $"N{FormatValue(model.Latitude)} E{FormatValue(model.Longitude)}";
            textViewTrackingCoordinates.Text = coordinatesText;

            var duration = dateTime - startTime;
            var durationText = $"Duration: {duration.Hours}h {duration.Minutes}m {duration.Seconds}s";
            textViewTrackingDuration.Text = durationText;

            var startedText = $"Started: {startTime.ToLocalTime().ToString("MM.dd.yyyy HH:mm")}";
            textViewTrackingStarted.Text = startedText;

            // TODO?
            if (currentModel != null && dateTime > GetDateTime(currentModel.Time))
            {
                var delta = CalculateDistance(currentModel.Latitude, currentModel.Longitude, model.Latitude, model.Longitude);
                distance += delta;
            }

            textViewTrackingDistance.Text = distance.ToString("Distance: 0.00km");
            currentModel = model;

            string FormatValue(double value)
            {
                return value.ToString("0.000000");
            }
        }

        private void TrackingStarted()
        {
            isStarted = false;
            startTime = default(DateTime);
            distance = 0;
            currentModel = null;

            textViewTrackingCoordinates.Text = string.Empty;
            textViewTrackingDuration.Text = string.Empty;
            textViewTrackingStarted.Text = string.Empty;
            textViewTrackingDistance.Text = string.Empty;
        }

        private void TrackingStopped(DateTime stopTime)
        {
            if (distance > 0)
            {
                var trackEntity = new TrackEntity
                {
                    StartDate = startTime,
                    EndDate = stopTime, // TODO: utc?
                    Distance = distance
                };

                _trackService.Add(trackEntity);

                var json = JsonConvert.SerializeObject(trackEntity);
                var intent = new Intent("TrackingStopped");
                intent.PutExtra("TrackEntity", json);

                LocalBroadcastManager.SendBroadcast(intent);
            }

            isStarted = false;
            startTime = default(DateTime);
            distance = 0;
            currentModel = null;

            textViewTrackingCoordinates.Text = string.Empty;
            textViewTrackingDuration.Text = string.Empty;
            textViewTrackingStarted.Text = string.Empty;
            textViewTrackingDistance.Text = string.Empty;
        }

        private DateTime GetDateTime(long ticks)
        {
            var startDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return startDate.AddMilliseconds(ticks);
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // https://stackoverflow.com/questions/365826/calculate-distance-between-2-gps-coordinates
            // http://www.movable-type.co.uk/scripts/latlong.html

            var earthRadiusKm = 6371;
            var φ1 = DegreesToRadians(lat1);
            var φ2 = DegreesToRadians(lat2);

            var Δφ = DegreesToRadians(lat2 - lat1);
            var Δλ = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) + Math.Cos(φ1) * Math.Cos(φ2) * Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = earthRadiusKm * c;

            return distance;

            double DegreesToRadians(double value)
            {
                return (value * Math.PI) / 180;
            }
        }
    }
}