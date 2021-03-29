using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Android.Support.V4.Content;
using AndroidX.Work;

namespace GpsTracker
{
    public class UploaderWorker : AndroidX.Work.Worker
    {
        SettingsService _settingsService;
        LocationService _locationService;
        NetworkLogService _networkLogService;

        private static object _isRunningLockObject = new object();

        private static bool _isRunning = false;
        public bool IsRunning
        {
            get
            {
                lock (_isRunningLockObject)
                {
                    return _isRunning;
                }
            }
            set
            {
                lock (_isRunningLockObject)
                {
                    _isRunning = value;
                }
            }
        }

        LocalBroadcastManager _localBroadcastManager;

        public UploaderWorker(Context context, WorkerParameters workerParameters) : base(context, workerParameters)
        {
            _settingsService = new SettingsService();
            _locationService = new LocationService();
            _networkLogService = new NetworkLogService();

            _localBroadcastManager = LocalBroadcastManager.GetInstance(context);
        }

        public override Result DoWork()
        {
            if (IsRunning)
            {
                return Result.InvokeSuccess();
            }

            try
            {
                IsRunning = true;

                var settings = _settingsService.GetSettings();

                if (!settings.IsEmailSendingEnabled)
                {
                    return Result.InvokeSuccess();
                }

                if (!settings.UploadOnMobileNetwork && !CustomNetworkCallback.IsConnected) // TODO
                {
                    return Result.InvokeSuccess();
                }

                var locations = _locationService.QueryNotUploadedLocations();
                var networkLogs = _networkLogService.QueryNotUploaded();

                if (!locations.Any() && !networkLogs.Any())
                {
                    return Result.InvokeSuccess();
                }

                var exporter = new Exporter();

                var json = exporter.CreateJson(locations, networkLogs);

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

                foreach (var networkLog in networkLogs)
                {
                    networkLog.IsUploaded = true;
                    networkLog.UploadDateTime = DateTime.UtcNow;
                }

                _networkLogService.Update(networkLogs);

                var intent = new Intent("testAction");
                intent.PutExtra("x", $"{DateTime.Now.ToString("HH:mm:ss")} - email sent: {locations.Count} location(s)");
                _localBroadcastManager.SendBroadcast(intent);

                _locationService.RemoveLocations(settings.KeepLocationsForDays);
                _networkLogService.Remove(settings.KeepLocationsForDays);

                return Result.InvokeSuccess();
            }
            catch (Exception ex)
            {
                var intent = new Intent("testAction");
                intent.PutExtra("x", $"{DateTime.Now.ToString("HH:mm:ss")} - email sending failed");
                _localBroadcastManager.SendBroadcast(intent);
                return Result.InvokeSuccess(); // TODO?
            }
            finally
            {
                IsRunning = false;
            }
        }

    }
}