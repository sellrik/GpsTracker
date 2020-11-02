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

namespace GpsTracker.Database
{
    public class SettingsModel
    {
        public int MinTime { get; set; }

        public int MinDistance { get; set; }

        public bool IsTelegramUploadEnabled { get; set; }

        public string UploadUrl { get; set; }

        public string TelegramBotToken { get; set; }

        public string TelegramChatId { get; set; }

        public bool IsEmailSendingEnabled { get; set; }

        public int SmtpPort { get; set; }

        public string SmtpHost { get; set; }

        public string SmtpUsername { get; set; }

        public string SmtpPassword { get; set; }

        public string EmailRecipient { get; set; }
    }
}