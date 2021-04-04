using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GpsTracker.Database;
using GpsTracker.Database.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;

namespace GpsTracker.Services
{
    public class TrackService
    {
        private readonly DatabaseService _databaseService;

        public TrackService()
        {
            _databaseService = DependencyInjection.Container.Resolve<DatabaseService>();
        }

        public void Add(TrackEntity entity)
        {
            _databaseService.Insert(entity);
        }

        public List<TrackEntity> Query()
        {
            return _databaseService.Query<TrackEntity>()
              .ToList();
        }
    }
}