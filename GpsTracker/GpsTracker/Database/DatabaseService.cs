using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GpsTracker.Database.Entity;
using SQLite;
using Environment = System.Environment;

namespace GpsTracker.Database
{
    public class DatabaseService
    {
        private SQLiteConnection _connection { get; set; }
        private readonly string _connectionString;

        private object _lockObject = new object();

        public DatabaseService()
        {
            _connectionString = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "database.db");

            _connection = new SQLiteConnection(_connectionString);

            _connection.CreateTable<LocationEntity>();
            _connection.CreateTable<SettingEntity>();
            _connection.CreateTable<NetworkLogEntity>();
        }

        public int Insert(object entity)
        {
            return Lock(() =>
            {
                return _connection.Insert(entity);
            });
        }

        public int InsertOrReplace(object entity)
        {
            return Lock(() =>
            {
                return _connection.InsertOrReplace(entity);
            });
        }

        public TableQuery<T> Query<T>() where T : new()
        {
            return Lock(() =>
            {
                return _connection.Table<T>();
            });
        }

        public int Update(object entity)
        {
            return Lock(() =>
            {
                return _connection.Update(entity);
            });
        }

        public int UpdateAll(IEnumerable<object> entities)
        {
            return Lock(() =>
            {
                return _connection.UpdateAll(entities);
            });
        }

        public T Find<T>(object primaryKey) where T : new()
        {
            return Lock(() =>
            {
                return _connection.Find<T>(primaryKey);
            });
        }

        private T Lock<T>(Func<T> func)
        {
            lock (_lockObject)
            {
                return func();
            }
        }

        private void Lock<T>(Action action)
        {
            lock (_lockObject)
            {
                action();
            }
        }

        public void RemoveLocations(List<LocationEntity> entities)
        {
            Lock<LocationEntity>(() =>
            {
                if (!entities.Any())
                {
                    return;
                }

                var ids = string.Join(", ", entities.Select(i => i.Id));
                var query = $"delete from LocationEntity where Id in ({ids})";
                _connection.Execute(query);
            });
        }

        public void RemoveNetworkLogs(List<NetworkLogEntity> entities)
        {
            Lock<NetworkLogEntity>(() =>
            {
                if (!entities.Any())
                {
                    return;
                }

                var ids = string.Join(", ", entities.Select(i => i.Id));
                var query = $"delete from NetworkLogEntity where Id in ({ids})";
                _connection.Execute(query);
            });
        }
    }
}