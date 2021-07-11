
using GpsTracker.Database;
using Unity;

namespace GpsTracker
{
    public static class DependencyInjection
    {
        private static IUnityContainer _container;

        public static IUnityContainer Container
        {
            get
            {
                if (_container == null)
                {
                    _container = new UnityContainer();
                    _container.RegisterSingleton<DatabaseService>();
                    _container.RegisterSingleton<LocationUploaderService>();
                }

                return _container;
            }
        }
    }
}