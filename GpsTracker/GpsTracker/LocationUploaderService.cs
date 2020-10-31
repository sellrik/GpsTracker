using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GpsTracker.Database;
using GpsTracker.Models;
using Newtonsoft.Json;
using SQLite;

namespace GpsTracker
{
    public class LocationUploaderService
    {
        private SettingsService _settingsService;
        private LocationService _locationService;

        private static bool IsRunning = false;

        public LocationUploaderService()
        {
            _settingsService = new SettingsService();
            _locationService = new LocationService();
        }

        public void UploadLocations()
        {
            if (IsRunning)
            {
                return;
            }

            IsRunning = true;

            Task.Run(() =>
            {
                try
                {
                    return; // TODO!

                    var settings = _settingsService.GetSettings();

                    if (!settings.IsUploadEnabled)
                    {
                        return;
                    }

                    while (true)
                    {
                        var locations = _locationService.QueryNotUploadedLocations();

                        if (!locations.Any())
                        {
                            break;
                        }

                        var model = new LocationUploadModel
                        {
                            Locations = locations
                        };

                        //var serializedModel = JsonConvert.SerializeObject(model);

                        var telegramClient = new TelegramClient(settings);
                        //telegramClient.SendMessage(serializedModel);

                        /*
                        using (var httpclient = new HttpClient())
                        using (var content = new StringContent(serializedModel))
                        {
                            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                            var result = httpclient.PostAsync(settings.UploadUrl, content).Result;

                            if (!result.IsSuccessStatusCode)
                            {
                                throw new InvalidOperationException($"Failed to upload locations. Status code {result.StatusCode}");
                            }
                        }*/

                        foreach (var location in locations)
                        {
                            telegramClient.SendLocation(location.Latitude, location.Longitude);

                            location.IsUploaded = true;
                            location.UploadDateTime = DateTime.UtcNow;

                            var tmp = new List<LocationEntity>();
                            tmp.Add(location);
                            _locationService.UpdateLocations(tmp);

                            Thread.Sleep(5000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    IsRunning = false;
                }
            });
        }
    }
}