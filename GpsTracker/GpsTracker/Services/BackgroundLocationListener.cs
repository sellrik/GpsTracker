using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.LocalBroadcastManager.Content;
using GpsTracker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace GpsTracker.Services
{
    public class BackgroundLocationListener : Java.Lang.Object, ILocationListener
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

            Task.Run(() =>
            {
                var model = new LocationChangedModel(location);
                var json = JsonConvert.SerializeObject(model);

                var intent = new Intent("LocationChanged");
                intent.PutExtra("Location", json);

                _localBroadcastManager.SendBroadcast(intent);
            });

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
}