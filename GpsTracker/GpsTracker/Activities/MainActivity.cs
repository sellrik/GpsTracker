using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.LocalBroadcastManager.Content;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.Tabs;
using GpsTracker.Activities;
using static Google.Android.Material.Tabs.TabLayoutMediator;

namespace GpsTracker
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleInstance)]
    public class MainActivity : AppCompatActivity
    {
        private LocalBroadcastManager LocalBroadcastManager => LocalBroadcastManager.GetInstance(this);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "GPS tracker";

            var viewPager = FindViewById<ViewPager2>(Resource.Id.viewPager1);
            var pagerAdapter = new CustomPagerAdapter(this);
            viewPager.OffscreenPageLimit = 3;
            viewPager.Adapter = pagerAdapter;

            var tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);
            var tabLayoutMediator = new TabLayoutMediator(tabLayout, viewPager, new TabConfigurationStrategy());
            tabLayoutMediator.Attach();
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
            else if (item.ItemId == Resource.Id.menu_settings)
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
        }

        private class TabConfigurationStrategy : Java.Lang.Object, ITabConfigurationStrategy
        {
            public void OnConfigureTab(TabLayout.Tab p0, int p1)
            {
                switch (p1)
                {
                    case 0:
                        {
                            p0.SetText("Track");
                            break;
                        }
                    case 1:
                        {
                            p0.SetText("Tracks");
                            break;
                        }
                    case 2:
                        {
                            p0.SetText("Logs");
                            break;
                        }
                    default:
                        p0.SetText("");
                        break;
                }
            }
        }
    }
}

