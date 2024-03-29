﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.LocalBroadcastManager.Content;
using AndroidX.Work;
using GpsTracker.Database;
using GpsTracker.Models;
using GpsTracker.Services;
using Newtonsoft.Json;
using Unity;

namespace GpsTracker
{
    [Service(Name = "com.companyname.gpstracker.BackgroundLocationService")]
    public class BackgroundLocationService : Service
    {
        private readonly int NotificationId = 9999;

        private readonly string ChannelId = "GpsTrackerNotificationChannel";

        public static bool IsStarted
        {
            get; private set;
        }

        private const int ForegroundId = 1234;

        private SettingsService _settingsService;

        private LocationManager LocationManager
        {
            get
            {
                return (LocationManager)GetSystemService(LocationService);
            }
        }

        private LocalBroadcastManager LocalBroadcastManager
        {
            get
            {
                return LocalBroadcastManager.GetInstance(this);
            }
        }

        private ConnectivityManager ConnectivityManager
        {
            get
            {
                return (ConnectivityManager)GetSystemService(ConnectivityService);
            }
        }

        private BackgroundLocationListener _backgroundLocationListener;

        private CustomNetworkCallback _customNetworkCallback;

        public override void OnCreate()
        {
            base.OnCreate();

            var notification = CreateNotification();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                StartForeground(ForegroundId, notification);
            }
            else
            {
                var notificationManagerCompat = NotificationManagerCompat.From(Application.Context);
                notificationManagerCompat.Notify(NotificationId, notification);
            }

            _backgroundLocationListener = new BackgroundLocationListener(LocalBroadcastManager);
            _settingsService = new SettingsService();

            CheckGpsEnabled();

            Start();
        }

        public override void OnTaskRemoved(Intent rootIntent)
        {
            base.OnTaskRemoved(rootIntent);

            Stop();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            Stop();
        }

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        private void Start()
        {
            var settings = _settingsService.GetSettings();

            StartLocationUpdates();

            var builder = new NetworkRequest.Builder();
            builder.AddTransportType(TransportType.Wifi);
            var networkRequest = builder.Build();

            _customNetworkCallback = new CustomNetworkCallback((WifiManager)GetSystemService(WifiService), StartLocationUpdates, StopLocationUpdates);
            ConnectivityManager.RegisterNetworkCallback(networkRequest, _customNetworkCallback);

            if (settings.IsEmailSendingEnabled)
            {
                var request = PeriodicWorkRequest
                    .Builder
                    .From<UploaderWorker>(TimeSpan.FromMinutes(settings.EmailSendingInterval))
                    //.SetBackoffCriteria(BackoffPolicy.Linear, TimeSpan.FromMinutes(5))
                    .SetConstraints(GetConstraints(settings))
                    .Build();

                WorkManager.Instance.EnqueueUniquePeriodicWork("GpsTrackerUploaderWorker", ExistingPeriodicWorkPolicy.Keep, request); // TODO: ExistingPeriodicWorkPolicy.Replace?
            }

            IsStarted = true;
        }

        private void Stop()
        {
            StopLocationUpdates();

            StopForeground(true);
            IsStarted = false;

            WorkManager.Instance.CancelAllWork();
            //WorkManager.Instance.CancelUniqueWork("GpsTrackerUploaderWorker");

            //var settings = _settingsService.GetSettings();

            var request = OneTimeWorkRequest
                .Builder
                .From<UploaderWorker>()
                //.SetConstraints(GetConstraints(settings))
                .Build();

            WorkManager.Instance.EnqueueUniqueWork("GpsTrackerUploaderWorkerOneTime", ExistingWorkPolicy.Keep, request);

            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.CancelAll();

            if (_customNetworkCallback != null)
            {
                ConnectivityManager.UnregisterNetworkCallback(_customNetworkCallback);
                _customNetworkCallback = null;
            }
        }

        private Constraints GetConstraints(SettingsModel settings)
        {
            var contstraints = new Constraints
                .Builder()
                .SetRequiredNetworkType(settings.UploadOnMobileNetwork ? NetworkType.Connected : NetworkType.Unmetered)
                .Build();

            return contstraints;
        }

        private void StartLocationUpdates(SettingsModel settings)
        {
            LocationManager.RequestLocationUpdates(LocationManager.GpsProvider, settings.MinTime * 1000, settings.MinDistance, _backgroundLocationListener);
        }

        private void StartLocationUpdates()
        {
            var settings = _settingsService.GetSettings();
            StartLocationUpdates(settings);
        }

        private void StopLocationUpdates()
        {
            LocationManager.RemoveUpdates(_backgroundLocationListener);
        }

        private Notification CreateNotification()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var notificationManager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);

                var name = "GPS tracker";
                var description = "GPS tracker";
                var importance = NotificationImportance.Default;
                var channel = new NotificationChannel(ChannelId, name, importance);
                channel.Description = description;

                // Register the channel with the system; you can't change the importance
                // or other notification behaviors after this

                notificationManager.CreateNotificationChannel(channel);
            }

            var context = Application.Context;
            var intent = context.PackageManager.GetLaunchIntentForPackage(context.PackageName);

            intent.AddFlags(ActivityFlags.ClearTop);

            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);

            var builder = new NotificationCompat.Builder(this, ChannelId);

            //builder.SetContentTitle("Tracking started");
            builder.SetContentText("Tracking started");
            builder.SetContentIntent(pendingIntent);
            builder.SetSmallIcon(Resource.Mipmap.ic_launcher);
            builder.SetOngoing(true);

            return builder.Build();
        }

        private void CheckGpsEnabled()
        {
            if (!LocationManager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                Toast.MakeText(this, "GPS is disabled!", ToastLength.Long).Show();
            }
        }
    }
}