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
        Dictionary<int, string> _networkDictionary = new Dictionary<int, string>();

        private static object _isConnectedLockObject = new object();
        private static bool _isConnected;

        public static bool IsConnected
        {
            get
            {
                lock (_isConnectedLockObject)
                {
                    return _isConnected;
                }
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
            IsConnected = false;
        }

        public override void OnAvailable(Network network)
        {
            var info = _wifiManager.ConnectionInfo;
            var ssid = info.SSID.Replace("\"", "");

            var hashCode = network.GetHashCode();

            if (!_networkDictionary.TryGetValue(hashCode, out var value))
            {
                _networkDictionary.Add(hashCode, ssid);
            }

            _networkLogService.Add(DateTime.UtcNow, true, ssid, hashCode);

            base.OnAvailable(network);

            IsConnected = true;

            if (DisableTrackingOnWifi())
            {
                _stopLocationUpdates();
            }

        }

        public override void OnLost(Network network)
        {
            IsConnected = false;

            var hashCode = network.GetHashCode();

            string ssid = string.Empty;

            if (_networkDictionary.TryGetValue(hashCode, out var value))
            {
                _networkDictionary.Remove(hashCode);
                ssid = value;
            }

            _networkLogService.Add(DateTime.UtcNow, false, ssid, hashCode); // TODO?

            base.OnLost(network);

            if (DisableTrackingOnWifi())
            {
                _startLocationUpdates();
            }
        }

        public override void OnLosing(Network network, int maxMsToLive)
        {
            base.OnLosing(network, maxMsToLive);
        }

        public override void OnUnavailable()
        {
            base.OnUnavailable();
        }

        private bool DisableTrackingOnWifi()
        {
            var settings = _settingsService.GetSettings();
            return settings.DisableTrackingOnWifi;
        }
    }
}