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
    public enum SettingTypeEnum
    {
        MinTime = 1,
        MinDistance = 2,
        IsUploadEnabled = 3,
        UploadUrl = 4,
        TelegramBotToken = 5,
        TelegramChatId = 6
    }
}