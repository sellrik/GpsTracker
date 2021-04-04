using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.ViewPager.Widget;
using AndroidX.ViewPager2.Adapter;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GpsTracker.Activities
{
    public class CustomPagerAdapter : FragmentStatePagerAdapter
    {
        TrackingFragment trackingFragment;
        TracksFragment tracksFragment;
        LogsFragment logsFragment;

        public CustomPagerAdapter(AndroidX.Fragment.App.FragmentManager fm) : base(fm)
        {
            trackingFragment = new TrackingFragment();
            tracksFragment = new TracksFragment();
            logsFragment = new LogsFragment();
        }

        public override int Count => 3;

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            switch (position)
            {
                case 0:
                    {
                        return new Java.Lang.String("Track");
                    }
                case 1:
                    {
                        return new Java.Lang.String("Tracks");
                    }
                case 2:
                    {
                        return new Java.Lang.String("Logs");
                    }
                default:
                    return new Java.Lang.String();
            }
        }

        public override AndroidX.Fragment.App.Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    {
                        return trackingFragment;
                    }
                case 1:
                    {
                        return tracksFragment;
                    }
                case 2:
                    {
                        return logsFragment;
                    }
                default:
                    throw new System.Exception("Unsupported item!");
            }

        }
    }
}