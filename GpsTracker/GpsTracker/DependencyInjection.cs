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
using GpsTracker.Database;
using Unity;

namespace GpsTracker
{
    public static class DependencyInjection
    {
        public static IUnityContainer Container { get; private set; }

        public static void Setup()
        {
            Container = new UnityContainer();
            Container.RegisterSingleton<DatabaseService>();
            Container.RegisterSingleton<LocationUploaderService>();
        }
    }
}