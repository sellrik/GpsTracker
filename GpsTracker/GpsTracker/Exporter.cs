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
using GpsTracker.Models;
using Newtonsoft.Json;

namespace GpsTracker
{
    public class Exporter
    {
        public string CreateGpx(DateTime from, DateTime to)
        {
            var locations = GetLocations(from, to);

            var builder = new StringBuilder();

            builder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            builder.AppendLine("<gpx xmlns=\"http://www.topografix.com/GPX/1/1\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd\" xmlns:gpxtpx=\"http://www.garmin.com/xmlschemas/TrackPointExtension/v1\" creator=\"GPS Tracker\" version=\"1.1\">");

            builder.AppendLine("<trk>");

            builder.AppendLine("<name>Recorded locations</name>");

            builder.AppendLine("<trkseg>");

            foreach (var location in locations)
            {
                builder.AppendLine($"<trkpt lat=\"{Math.Round(location.Latitude, 6, MidpointRounding.AwayFromZero)}\" lon=\"{Math.Round(location.Longitude, 6, MidpointRounding.AwayFromZero)}\">");

                builder.AppendLine($"<ele>{Math.Round(location.Altitude, 6, MidpointRounding.AwayFromZero)}</ele>");
                builder.AppendLine($"<time>{location.DateTime.ToString("yyyy-MM-ddTHH:mm:ssZ")}</time>");

                builder.AppendLine("</trkpt>");
            }

            builder.AppendLine("</trkseg>");

            builder.AppendLine("</trk>");

            builder.AppendLine("</gpx>");

            return builder.ToString();
        }
    
        public string CreateJson(List<LocationEntity> locations)
        {
            var locationsModel = locations.Select(i => new LocationJsonModel(i)).ToList();

            var json = JsonConvert.SerializeObject(locationsModel);

            return json;
        }

        public string CreateJson(DateTime from, DateTime to)
        {
            var locations = GetLocations(from, to);
            return CreateJson(locations);
        }

        private List<LocationEntity> GetLocations(DateTime from, DateTime to)
        {
            var locationServie = new LocationService();

            var locations = locationServie
                .QueryLocation(from, to)
                .OrderBy(i => i.Time)
                .ToList();

            return locations;
        }
    }
}