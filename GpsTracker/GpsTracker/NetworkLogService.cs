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
using GpsTracker.Database.Entity;
using Unity;

namespace GpsTracker
{
    public class NetworkLogService
    {
        private readonly DatabaseService _databaseService;

        public NetworkLogService()
        {
            _databaseService = DependencyInjection.Container.Resolve<DatabaseService>();
        }

        public void Add(DateTime dateTime, bool isConnected)
        {
            var entity = new NetworkLogEntity
            {
                IsConnected = isConnected,
                DateTime = dateTime
            };

            _databaseService.Insert(entity);
        }

        public List<NetworkLogEntity> Query(DateTime from, DateTime to)
        {
            from = from.Date;
            to = to.Date.AddDays(1).AddSeconds(-1);

            return _databaseService.Query<NetworkLogEntity>()
                .Where(i => i.DateTime >= from && i.DateTime <= to)
                .ToList();
        }

        public List<NetworkLogEntity> QueryNotUploaded()
        {
            return _databaseService.Query<NetworkLogEntity>()
              .Where(i => !i.IsUploaded)
              .ToList();
        }

        public void Update(List<NetworkLogEntity> entities)
        {
            _databaseService.UpdateAll(entities);
        }

        public void Remove(int days)
        {
            var to = DateTime.Now.AddDays(-days);

            var entities = _databaseService.Query<NetworkLogEntity>()
                .Where(i => i.DateTime <= to)
                .ToList();

            _databaseService.RemoveNetworkLogs(entities);
        }
    }
}