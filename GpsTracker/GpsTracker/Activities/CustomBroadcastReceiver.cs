using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GpsTracker
{
    public class CustomBroadcastReceiver : BroadcastReceiver
    {
        Action<string> _addLog;

        public CustomBroadcastReceiver(Action<string> addLog)
        {
            _addLog = addLog;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            switch (intent.Action)
            {
                case "testAction":
                    {
                        _addLog(intent.GetStringExtra("x"));
                        break;
                    }
                default:
                    break;
            }
        }
    }
}