using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.LocalBroadcastManager.Content;
using GpsTracker.Database.Entity;
using GpsTracker.Services;
using System.Collections.Generic;
using System.Linq;

namespace GpsTracker.Activities
{
    public class TracksFragment : Fragment
    {
        private ListView listView;
        private static List<string> listViewData = new List<string>();
        private CustomAdapter adapter;

        private TracksBroadcastReceiver _broadcastReceiver;

        private TrackService _trackService;

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

            _trackService = new TrackService();
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
            var view = inflater.Inflate(Resource.Layout.fragment_tracks, container, false);
            listView = view.FindViewById<ListView>(Resource.Id.listViewTracks);

            adapter = new CustomAdapter(inflater, listViewData);
            listView.Adapter = adapter;

            LoadTracks();

            return view;
        }

        private void RegisterBroadcastReceiver()
        {
            if (_broadcastReceiver == null)
            {
                var intentFilter = new IntentFilter();
                intentFilter.AddAction("TrackingStopped");

                _broadcastReceiver = new TracksBroadcastReceiver(TrackingStopped);

                LocalBroadcastManager.RegisterReceiver(_broadcastReceiver, intentFilter);
            }
        }

        private void TrackingStopped(TrackEntity entity)
        {
            LoadTracks();
        }

        private void LoadTracks()
        {
            var tracks = _trackService.Query()
                .Take(10)
                .OrderBy(i => i.StartDate);

            var data = new List<string>();

            foreach (var track in tracks)
            {
                var text = $"{track.StartDate.ToLocalTime().ToString("MM.dd.yyyy HH:mm")} - {track.EndDate.ToLocalTime().ToString("MM.dd.yyyy HH:mm")}: {track.Distance?.ToString("0.00")}km";
                data.Add(text);
            }

            LoadData(data);
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

        private void LoadData(IEnumerable<string> data)
        {
            Activity.RunOnUiThread(() =>
            {
                lock (_lockObject)
                {
                    listViewData.Clear();

                    foreach (var item in data)
                    {
                        listViewData.Add(item);
                    }

                    adapter.NotifyDataSetChanged();
                }
            });
        }
    }
}