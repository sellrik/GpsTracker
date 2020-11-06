using System;
using System.Collections.Generic;
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
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleInstance)]
    public class MainActivity : AppCompatActivity
    {
        private const int AccessFineLocationPermissionCode = 123;
        private bool _accessFineLocationPermissionGranted = false;
        private Button _startStopButton;

        private string _startStopButtonStartedText = "STOP";
        private string _startStopButtonStoppedText = "START";

        private CustomBroadcastReceiver _broadcastReceiver;

        private ListView _listViewLog;
        private static List<string> _listViewData = new List<string>();
        private CustomAdapter _adapter;

        private LocalBroadcastManager LocalBroadcastManager
        {
            get
            {
                return LocalBroadcastManager.GetInstance(this);
            }
        }

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

            _listViewLog = FindViewById<ListView>(Resource.Id.listViewLog);
            var inflater = (LayoutInflater)GetSystemService(Context.LayoutInflaterService);
            _adapter = new CustomAdapter(inflater, _listViewData);
            _listViewLog.Adapter = _adapter;

            RegisterBroadcastReceiver();
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
            RegisterBroadcastReceiver();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnRegisterBroadcastReceiver();
            StopService();
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

        private void RegisterBroadcastReceiver()
        {
            if (_broadcastReceiver == null)
            {
                var intentFilter = new IntentFilter();
                intentFilter.AddAction("testAction");

                _broadcastReceiver = new CustomBroadcastReceiver(AddLog);

                LocalBroadcastManager.RegisterReceiver(_broadcastReceiver, intentFilter);
            }
        }

        private void UnRegisterBroadcastReceiver()
        {
            if (_broadcastReceiver != null)
            {
                LocalBroadcastManager.UnregisterReceiver(_broadcastReceiver);
                _broadcastReceiver = null;
            }
        }

        object _lockObject = new object();
        private void AddLog(string data)
        {
            RunOnUiThread(() =>
            {
                _listViewData.Add(data);
                _adapter.NotifyDataSetChanged();

                lock (_lockObject)
                {
                    if (_adapter.Count == 11)
                    {
                        _listViewData.RemoveAt(0);
                    }
                }
            });
        }
    }

    internal class CustomAdapter : BaseAdapter<string>
    {
        LayoutInflater _infalter;
        private List<string> _items = new List<string>();

        public CustomAdapter(LayoutInflater inflater, List<string> items)
        {
            _infalter = inflater;
            _items = items;
            _items.Reverse();
        }

        public override string this[int position] => _items[position];

        public override int Count => _items.Count;

        public override Java.Lang.Object GetItem(int position)
        {
            return _items[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var index = Math.Abs(_items.Count - 1 - position);
            var item = _items[index];

            convertView = _infalter.Inflate(Android.Resource.Layout.SimpleListItem1, null);

            convertView.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item;

            return convertView;
        }
    }
}

