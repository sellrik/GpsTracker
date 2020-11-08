using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GpsTracker
{
    internal class CustomNetworkCallback : ConnectivityManager.NetworkCallback
    {
        WifiManager _wifiManager;
        Action _startLocationUpdates;
        Action _stopLocationUpdates;
        NetworkLogService _networkLogService;
        SettingsService _settingsService;

        private static object _isConnectedLockObject = new object();
        private static bool _isConnected;

        public static bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                lock (_isConnectedLockObject)
                {
                    _isConnected = value;
                }
            }
        }

        public CustomNetworkCallback(WifiManager wifiManager, Action startLocationUpdates, Action stopLocationUpdates)
        {
            _wifiManager = wifiManager;
            _startLocationUpdates = startLocationUpdates;
            _stopLocationUpdates = stopLocationUpdates;

            _networkLogService = new NetworkLogService();
            _settingsService = new SettingsService();
        }

        public override void OnAvailable(Network network)
        {
            base.OnAvailable(network);
            //var info = _wifiManager.ConnectionInfo; TODO: permission

            _isConnected = true;

            if (DisableTrackingOnWifi())
            {
                _stopLocationUpdates();
                _networkLogService.Add(DateTime.UtcNow, true);
            }
        }

        public override void OnLost(Network network)
        {
            _isConnected = false;

            base.OnLost(network);

            if (DisableTrackingOnWifi())
            {
                _startLocationUpdates();

                _networkLogService.Add(DateTime.UtcNow, false);
            }
        }

        private bool DisableTrackingOnWifi()
        {
            var settings = _settingsService.GetSettings();
            return settings.DisableTrackingOnWifi;
        }
    }
}