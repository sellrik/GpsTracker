using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using AndroidX.Work;
using GpsTracker.Models;
using Newtonsoft.Json;

namespace GpsTracker
{
    public class UploaderWorker : Worker
    {
        SettingsService _settingsService;
        LocationService _locationService;

        static bool IsRunning = false;

        LocalBroadcastManager _localBroadcastManager;

        // TODO?
        public UploaderWorker(Context context, WorkerParameters workerParameters) : base(context, workerParameters)
        {
            _settingsService = new SettingsService();
            _locationService = new LocationService();

            _localBroadcastManager = LocalBroadcastManager.GetInstance(context);
        }

        public override Result DoWork()
        {
            if (IsRunning) // TODO: kell?
            {
                return Result.InvokeSuccess();
            }

            try
            {
                IsRunning = true;

                var settings = _settingsService.GetSettings();

                // TODO?
                if (!settings.IsEmailSendingEnabled)
                {
                    return Result.InvokeSuccess();
                }

                var locations = _locationService.QueryNotUploadedLocations();

                if (!locations.Any())
                {
                    return Result.InvokeSuccess();
                }

                var exporter = new Exporter();

                var json = exporter.CreateJson(locations);

                var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

                var attachment = new Attachment(stream, "data.json", MediaTypeNames.Application.Json); // TODO: using?

                var emailClient = new EmailClient();

                emailClient.SendMail(
                    settings.SmtpPort,
                    settings.SmtpHost,
                    settings.SmtpUsername,
                    settings.SmtpPassword,
                    settings.SmtpUsername,
                    settings.EmailRecipient,
                    settings.EmailSubject,
                    string.Empty,
                    attachment
                    );

                foreach (var location in locations)
                {
                    location.IsUploaded = true;
                    location.UploadDateTime = DateTime.UtcNow;
                }

                _locationService.UpdateLocations(locations);

                var intent = new Intent("testAction");
                intent.PutExtra("x", $"{DateTime.Now.ToString("HH:mm:ss")} - email sent: {locations.Count} location(s)");
                _localBroadcastManager.SendBroadcast(intent);

                _locationService.RemoveLocations(settings.KeepLocationsForDays);

                return Result.InvokeSuccess();
            }
            catch (Exception ex)
            {
                var intent = new Intent("testAction");
                intent.PutExtra("x", $"{DateTime.Now.ToString("HH:mm:ss")} - email sending failed");
                _localBroadcastManager.SendBroadcast(intent);

                if (RunAttemptCount >= 3)
                {
                    return Result.InvokeFailure();
                }
                else
                {
                    return Result.InvokeRetry();
                }
            }
            finally
            {
                IsRunning = false;
            }
        }

    }
}