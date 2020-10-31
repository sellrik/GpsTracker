using System;
using System.IO;
using System.Threading;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using SQLite;

namespace GpsTracker
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const int AccessFineLocationPermissionCode = 123;
        private bool _accessFineLocationPermissionGranted = false;
        private Button _startStopButton;
        private TextView _lastUpdateTextView;

        private string _startStopButtonStartedText = "STOP";
        private string _startStopButtonStoppedText = "START";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            DependencyInjection.Setup();

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "GPS tracker";

            _startStopButton = FindViewById<Button>(Resource.Id.buttonStartStop);
            _startStopButton.Click += startStopButton_Click;
            _lastUpdateTextView = FindViewById<TextView>(Resource.Id.textViewLastUpdate);
            _lastUpdateTextView.Text = "";
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menu_export)
            {
                var intent = new Intent(this, typeof(ExportActivity));
                StartActivity(intent);
                return true;
            }
            else if(item.ItemId == Resource.Id.menu_settings)
            {
                var intent = new Intent(this, typeof(SettingsActivity));
                StartActivity(intent);
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        protected override void OnRestart()
        {
            base.OnRestart();
        }

        protected override void OnResume()
        {
            base.OnResume();
            SetStartStopButtonText(BackgroundLocationService.IsStarted);
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        private void StartLocationService()
        {
            CheckPermission();

            if (!HasPermission())
            {
                return;
            }

            StartService();
        }

        private void startStopButton_Click(object sender, EventArgs e)
        {
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

        void StartService()
        {
            var intent = new Intent(this, typeof(BackgroundLocationService));

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                StartForegroundService(intent);
            }
            else
            {
                StartService(intent);
            }

        }

        void StopService()
        {
            var intent = new Intent(this, typeof(BackgroundLocationService));
            StopService(intent);
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
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                _accessFineLocationPermissionGranted = true;
            }
            else
            {
                Toast.MakeText(this, "'AccessFineLocation' permission is missing!", ToastLength.Short).Show();

                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessFineLocation }, AccessFineLocationPermissionCode);

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

        private void LocationUpdated(Intent intent)
        {
            var date = DateTime.Parse(intent.GetStringExtra("Date"));
            var location = intent.GetStringExtra("Location");
            _lastUpdateTextView.Text = $"Last location update: {date}";

        }

        private void LocationRequestStatusQueryResult(bool isStarted)
        {
            if (isStarted)
            {
                _startStopButton.Text = _startStopButtonStartedText;
            }
            else
            {
                _startStopButton.Text = _startStopButtonStoppedText;
            }

            _startStopButton.Enabled = true;
        }
    }
}

