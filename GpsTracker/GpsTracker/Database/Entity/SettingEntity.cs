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
using SQLite;

namespace GpsTracker.Database
{
    public class SettingEntity
    {
        [PrimaryKey]
        public SettingTypeEnum Setting { get; set; }

        public string Value { get; set; }
    }
}