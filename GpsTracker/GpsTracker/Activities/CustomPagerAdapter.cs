using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Adapter;

namespace GpsTracker.Activities
{
    public class CustomPagerAdapter : FragmentStateAdapter
    {
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
                        return new TrackingFragment();
                    }
                case 1:
                    {
                        return new TracksFragment();
                    }
                case 2:
                    {
                        return new LogsFragment();
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