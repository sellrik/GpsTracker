using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using System.Collections.Generic;

namespace GpsTracker.Activities
{
    public class LogsFragment : Fragment
    {
        private ListView _listViewLog;
        private static List<string> _listViewData = new List<string>();
        private CustomAdapter _adapter;

        private CustomBroadcastReceiver _broadcastReceiver;

        private LocalBroadcastManager LocalBroadcastManager
        {
            get
            {
                return LocalBroadcastManager.GetInstance(Context);
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override void OnResume()
        {
            base.OnResume();
            RegisterBroadcastReceiver();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            UnRegisterBroadcastReceiver();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_logs, container, false);
            _listViewLog = view.FindViewById<ListView>(Resource.Id.listViewLog);

            _adapter = new CustomAdapter(inflater, _listViewData);
            _listViewLog.Adapter = _adapter;

            return view;
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
            Activity.RunOnUiThread(() =>
            {
                lock (_lockObject)
                {
                    _listViewData.Add(data);

                    if (_adapter.Count == 11)
                    {
                        _listViewData.RemoveAt(0);
                    }

                    _adapter.NotifyDataSetChanged();
                }
            });
        }
    }
}