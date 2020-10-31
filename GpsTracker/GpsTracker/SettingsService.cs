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
using Unity;

namespace GpsTracker
{
    public class SettingsService
    {
        private readonly DatabaseService _databaseService;

        private readonly int DefaultMinTime = 5;
        private readonly int DefaultMinDistance = 5;

        public SettingsService()
        {
            _databaseService = DependencyInjection.Container.Resolve<DatabaseService>();
        }

        public SettingsModel GetSettings()
        {
            var minTimeSetting = GetSetting(SettingTypeEnum.MinTime);
            var minDistanceSetting = GetSetting(SettingTypeEnum.MinDistance);
            var isUploadEnabledSetting = GetSetting(SettingTypeEnum.IsUploadEnabled);
            var uploadUrlSetting = GetSetting(SettingTypeEnum.UploadUrl);
            var telegramBotTokenSetting = GetSetting(SettingTypeEnum.TelegramBotToken);
            var telegramChatIdSetting = GetSetting(SettingTypeEnum.TelegramChatId);

            var mintTime = int.Parse(minTimeSetting.Value);
            var minDistance = int.Parse(minDistanceSetting.Value);
            var uploadUrl = uploadUrlSetting.Value;
            var isUploadEnabled = bool.Parse(isUploadEnabledSetting.Value);
            var telegramBotToken = telegramBotTokenSetting.Value;
            var telegramChatId = telegramChatIdSetting.Value;

            return new SettingsModel
            {
                MinTime = mintTime,
                MinDistance = minDistance,
                IsUploadEnabled = isUploadEnabled,
                UploadUrl = uploadUrl,
                TelegramBotToken = telegramBotToken,
                TelegramChatId = telegramChatId
            };
        }

        public void SaveSettings(SettingsModel settings)
        {
            if (settings.MinTime < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(settings.MinTime));
            }

            if (settings.MinDistance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(settings.MinDistance));
            }

            var minTimeSetting = new SettingEntity
            {
                Setting = SettingTypeEnum.MinTime,
                Value = settings.MinTime.ToString()
            };

            _databaseService.Update(minTimeSetting);

            var minDistanceSetting = new SettingEntity
            {
                Setting = SettingTypeEnum.MinDistance,
                Value = settings.MinDistance.ToString()
            };

            _databaseService.Update(minDistanceSetting);

            var isUploadEnabledSetting = new SettingEntity
            {
                Setting = SettingTypeEnum.IsUploadEnabled,
                Value = settings.IsUploadEnabled.ToString()
            };

            _databaseService.Update(isUploadEnabledSetting);

            //var uploadUrlSetting = new SettingEntity
            //{
            //    Setting = SettingTypeEnum.UploadUrl,
            //    Value = settings.UploadUrl
            //};

            //_databaseService.Update(uploadUrlSetting);

            var telegramBotTokenSetting = new SettingEntity
            {
                Setting = SettingTypeEnum.TelegramBotToken,
                Value = settings.TelegramBotToken
            };

            _databaseService.Update(telegramBotTokenSetting);

            var telegramChatIdSetting = new SettingEntity
            {
                Setting = SettingTypeEnum.TelegramChatId,
                Value = settings.TelegramChatId
            };

            _databaseService.Update(telegramChatIdSetting);
        }

        private SettingEntity GetSetting(SettingTypeEnum settingType)
        {
            var setting = _databaseService.Find<SettingEntity>(settingType);
            if (setting == null)
            {
                setting = new SettingEntity
                {
                    Setting = settingType,
                    Value = GetSettingDefaultValue(settingType)
                };

                _databaseService.InsertOrReplace(setting);
            }

            return setting;
        }

        private string GetSettingDefaultValue(SettingTypeEnum settingType)
        {
            switch (settingType)
            {
                case SettingTypeEnum.MinTime:
                    return DefaultMinTime.ToString();
                case SettingTypeEnum.MinDistance:
                    return DefaultMinDistance.ToString();
                case SettingTypeEnum.IsUploadEnabled:
                    return false.ToString();
                case SettingTypeEnum.UploadUrl:
                case SettingTypeEnum.TelegramBotToken:
                case SettingTypeEnum.TelegramChatId:
                    return string.Empty;
                default:
                    throw new ArgumentOutOfRangeException(nameof(settingType));
            }
        }
    }
}