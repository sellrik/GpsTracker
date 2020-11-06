using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Work;
using GpsTracker.Database;
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

        private BackgroundLocationListener _backgroundLocationListener;

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
            LocationManager.RequestLocationUpdates(LocationManager.GpsProvider, settings.MinTime * 1000, settings.MinDistance, _backgroundLocationListener);

            if (settings.IsEmailSendingEnabled)
            {
                var request = PeriodicWorkRequest
                    .Builder
                    .From<UploaderWorker>(TimeSpan.FromMinutes(settings.EmailSendingInterval))
                    .SetBackoffCriteria(BackoffPolicy.Linear, TimeSpan.FromMinutes(5))
                    .Build();

                WorkManager.Instance.EnqueueUniquePeriodicWork("GpsTrackerUploaderWorker", ExistingPeriodicWorkPolicy.Replace, request);
            }

            IsStarted = true;
        }

        private void Stop()
        {
            LocationManager.RemoveUpdates(_backgroundLocationListener);
            StopForeground(true);
            IsStarted = false;

            WorkManager.Instance.CancelAllWork();

            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.CancelAll();
        }

        private Notification CreateNotification()
        {
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                var notificationManager = (NotificationManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.NotificationService);

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

            builder.SetContentTitle("GPS tracker");
            builder.SetContentText("GPS tracker is running");
            builder.SetContentIntent(pendingIntent);
            builder.SetSmallIcon(Resource.Mipmap.ic_launcher);

            return builder.Build();
        }

        private class BackgroundLocationListener : Java.Lang.Object, ILocationListener
        {
            private LocalBroadcastManager _localBroadcastManager;
            private LocationUploaderService _locationUploaderService;
            private LocationService _locationService;
            private SettingsService _settingsService;

            private TelegramClient _telegramClient;

            public BackgroundLocationListener(LocalBroadcastManager localBroadcastManager)
            {
                _localBroadcastManager = localBroadcastManager;
                _locationUploaderService = DependencyInjection.Container.Resolve<LocationUploaderService>();
                _locationService = new LocationService();

                _settingsService = new SettingsService(); 

                var settings = _settingsService.GetSettings();
                _telegramClient = new TelegramClient(settings);
            }

            public void OnLocationChanged(Location location)
            {
                var intent = new Intent("testAction");
                intent.PutExtra("x", $"{DateTime.Now.ToString("HH:mm:ss")} - location set");
                //_localBroadcastManager.SendBroadcast(intent);

                _locationService.AddLocation(location);

                _locationUploaderService.UploadLocations();

                var settings = _settingsService.GetSettings();

                if (!settings.IsTelegramUploadEnabled)
                {
                    return;
                }

                Task.Run(() =>
                {
                    try
                    {
                        _telegramClient.SendLocation(location.Latitude, location.Longitude);

                        var intentTelegram = new Intent("testAction");
                        intentTelegram.PutExtra("x", $"{DateTime.Now.ToString("HH:mm:ss")} - location sent in Telegram");
                        //_localBroadcastManager.SendBroadcast(intentTelegram);
                    }
                    catch (Exception ex)
                    {
                        var intentTelegram = new Intent("testAction");
                        intentTelegram.PutExtra("x", $"{DateTime.Now.ToString("HH:mm:ss")} - location sending in Telegram failed");
                        _localBroadcastManager.SendBroadcast(intentTelegram);

                        throw;
                    }
                });
            }

            public void OnProviderDisabled(string provider)
            {
            }

            public void OnProviderEnabled(string provider)
            {
            }

            public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
            {
            }
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