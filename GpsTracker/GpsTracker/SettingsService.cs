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
using Unity.Injection;

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
            return new SettingsModel
            {
                MinTime = GetIntValue(SettingTypeEnum.MinTime),
                MinDistance = GetIntValue(SettingTypeEnum.MinDistance),
                IsTelegramUploadEnabled = GetBoolValue(SettingTypeEnum.IsTelegramUploadEnabled),
                UploadUrl = GetStringValue(SettingTypeEnum.UploadUrl),
                TelegramBotToken = GetStringValue(SettingTypeEnum.TelegramBotToken),
                TelegramChatId = GetStringValue(SettingTypeEnum.TelegramChatId),
                IsEmailSendingEnabled = GetBoolValue(SettingTypeEnum.IsEmailSendingEnabled),
                SmtpPort = GetIntValue(SettingTypeEnum.SmtpPort),
                SmtpHost = GetStringValue(SettingTypeEnum.SmtpHost),
                SmtpUsername = GetStringValue(SettingTypeEnum.SmtpUsername),
                SmtpPassword = GetStringValue(SettingTypeEnum.SmtpPassword),
                EmailRecipient = GetStringValue(SettingTypeEnum.EmailRecipient),
                EmailSendingInterval = GetIntValue(SettingTypeEnum.EmailSendingInterval),
                EmailSubject = GetStringValue(SettingTypeEnum.EmailSubject),
                KeepLocationsForDays = GetIntValue(SettingTypeEnum.KeepLocationsForDays)
            };

            int GetIntValue(SettingTypeEnum settingType)
            {
                return int.Parse(GetSetting(settingType).Value);
            }

            bool GetBoolValue(SettingTypeEnum settingType)
            {
                return bool.Parse(GetSetting(settingType).Value);
            }

            string GetStringValue(SettingTypeEnum settingType)
            {
                return GetSetting(settingType).Value;
            }
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

            SaveSetting(SettingTypeEnum.MinTime, settings.MinTime);
            SaveSetting(SettingTypeEnum.MinDistance, settings.MinDistance);
            SaveSetting(SettingTypeEnum.IsTelegramUploadEnabled, settings.IsTelegramUploadEnabled);
            SaveSetting(SettingTypeEnum.TelegramBotToken, settings.TelegramBotToken);
            SaveSetting(SettingTypeEnum.TelegramChatId, settings.TelegramChatId);
            SaveSetting(SettingTypeEnum.IsEmailSendingEnabled, settings.IsEmailSendingEnabled);
            SaveSetting(SettingTypeEnum.SmtpPort, settings.SmtpPort);
            SaveSetting(SettingTypeEnum.SmtpHost, settings.SmtpHost);
            SaveSetting(SettingTypeEnum.SmtpUsername, settings.SmtpUsername);
            SaveSetting(SettingTypeEnum.SmtpPassword, settings.SmtpPassword);
            SaveSetting(SettingTypeEnum.EmailRecipient, settings.EmailRecipient);
            SaveSetting(SettingTypeEnum.EmailSendingInterval, settings.EmailSendingInterval);
            SaveSetting(SettingTypeEnum.EmailSubject, settings.EmailSubject);
            SaveSetting(SettingTypeEnum.KeepLocationsForDays, settings.KeepLocationsForDays);

            void SaveSetting<T>(SettingTypeEnum typeEnum, T value)
            {
                var entity = new SettingEntity
                {
                    Setting = typeEnum,
                    Value = value.ToString()
                };

                _databaseService.Update(entity);
            }
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
                case SettingTypeEnum.IsTelegramUploadEnabled:
                case SettingTypeEnum.IsEmailSendingEnabled:
                    return false.ToString();
                case SettingTypeEnum.UploadUrl:
                case SettingTypeEnum.TelegramBotToken:
                case SettingTypeEnum.TelegramChatId:
                case SettingTypeEnum.SmtpUsername:
                case SettingTypeEnum.SmtpPassword:
                case SettingTypeEnum.EmailRecipient:
                    return string.Empty;
                case SettingTypeEnum.SmtpPort:
                    return "587";
                case SettingTypeEnum.SmtpHost:
                    return "smtp.gmail.com";
                case SettingTypeEnum.EmailSendingInterval:
                    return "30";
                case SettingTypeEnum.EmailSubject:
                    return "GPS tracker";
                case SettingTypeEnum.KeepLocationsForDays:
                    return 30.ToString();
                default:
                    throw new ArgumentOutOfRangeException(nameof(settingType));
            }
        }
    }
}