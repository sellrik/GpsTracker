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
    public class CustomPagerAdapter : FragmentStateAdapter
    {
        AndroidX.Fragment.App.Fragment[] items = new AndroidX.Fragment.App.Fragment[3];

        public CustomPagerAdapter(FragmentActivity fragmentActivity) : base(fragmentActivity)
        {
        }

        public CustomPagerAdapter(AndroidX.Fragment.App.Fragment fragment) : base(fragment)
        {

        }

        public override int ItemCount => 3;

        public override AndroidX.Fragment.App.Fragment CreateFragment(int p0)
        {
            switch (p0)
            {
                case 0:
                    {
                        if (items[0] == null)
                        {
                            items[0] = new TrackingFragment();
                        }

                        return items[0];
                    }
                case 1:
                    {
                        if (items[1] == null)
                        {
                            items[1] = new TracksFragment();
                        }

                        return items[1];
                    }
                case 2:
                    {
                        if (items[2] == null)
                        {
                            items[2] = new LogsFragment();
                        }

                        return items[2];
                    }
                default:
                    throw new System.Exception("Invalid fragment number!");
            }
        }

        //public override long GetItemId(int position)
        //{
        //}

        //public override bool ContainsItem(long itemId)
        //{
        //}
    }
}