using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;

namespace GpsTracker.Activities
{
    public class TracksFragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_tracks, container, false);
        }
    }
}