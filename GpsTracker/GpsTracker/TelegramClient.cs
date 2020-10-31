using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GpsTracker.Database;

namespace GpsTracker
{
    public class TelegramClient
    {
        private readonly HttpClient client = new HttpClient();
        //private readonly string token = "1111459003:AAFLwhppxSNEAbhHQoMTjncAbv7k2624ce4";
        private readonly string baseUrl = "https://api.telegram.org/bot";
        private readonly string baseUrlWithToken;

        //private readonly string channelId = "-1001204414229";

        private readonly SettingsModel settings;

        public TelegramClient(SettingsModel settings)
        {
            this.settings = settings;
            baseUrlWithToken = $"{baseUrl}{this.settings.TelegramBotToken}";
        }

        public void SendMessage(string text)
        {
            var url = $"{baseUrlWithToken}/sendMessage?chat_id={settings.TelegramChatId}"; 

            var values = new Dictionary<string, string>
            {
                { "text", $"\"{text}\"" } // TODO: too long!
            };

            var content = new FormUrlEncodedContent(values);

            var result = client.PostAsync(url, content).Result;

            if (result.IsSuccessStatusCode)
            {

            }
            else
            {
                throw new Exception("Telegram request failed!");
            }
        }

        public void SendLocation(double latitude, double longitude)
        {
            var url = $"{baseUrlWithToken}/sendLocation?chat_id={settings.TelegramChatId}";

            var values = new Dictionary<string, string>
            {
                { "latitude", latitude.ToString(CultureInfo.InvariantCulture)},
                { "longitude", longitude.ToString(CultureInfo.InvariantCulture)}
            };

            var content = new FormUrlEncodedContent(values);

            var result = client.PostAsync(url, content).Result;

            #if DEBUG
                Console.WriteLine(url);
            #endif

            if (result.IsSuccessStatusCode)
            {

            }
            else
            {
                throw new Exception("Telegram request failed!");
            }
        }
    }
}