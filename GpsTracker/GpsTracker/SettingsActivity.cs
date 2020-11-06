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
using Android.Support.V7.App;
using GpsTracker.Database;
using Android;

namespace GpsTracker
{
    [Activity(Label = "SettingsActivity", Theme = "@style/AppTheme")]
    public class SettingsActivity : AppCompatActivity
    {
        private Button _buttonSave;
        private Spinner _spinnerMinTime;
        private Spinner _spinnerMinDistance;
        private CheckBox _checkBoxIsTelegramUploadEnabled;
        //private EditText _editTextUploadUrl;
        private EditText _editTextTelegramBotToken;
        private EditText _editTextTelegramChatId;

        private CheckBox _checkBoxIsEmailSendingEnabled;
        private EditText _editTextSmtpPort;
        private EditText _editTextSmtpHost;
        private EditText _editTextSmtpUsername;
        private EditText _editTextSmtpPassword;
        private EditText _editTextEmailRecipient;
        private EditText _editTextEmailSendingInterval;
        private EditText _editTextEmailSubject;

        private SettingsService _settingsService;


        private List<KeyValuePair<int, string>> _minTimeMapping = new List<KeyValuePair<int, string>>
        {
            new KeyValuePair<int, string>(0, "0 sec"),
            new KeyValuePair<int, string>(1, "1 sec"),
            new KeyValuePair<int, string>(5, "5 sec"),
            new KeyValuePair<int, string>(10, "10 sec"),
            new KeyValuePair<int, string>(30, "30 sec"),
            new KeyValuePair<int, string>(60, "1 min"),
            new KeyValuePair<int, string>(300, "5 min"),
            new KeyValuePair<int, string>(600, "10 min"),
            new KeyValuePair<int, string>(1800, "30 min")
        };

        private List<KeyValuePair<int, string>> _minDistanceMapping = new List<KeyValuePair<int, string>>
        {
            new KeyValuePair<int, string>(0, "0 m"),
            new KeyValuePair<int, string>(1, "1 m"),
            new KeyValuePair<int, string>(5, "5 m"),
            new KeyValuePair<int, string>(10, "10 m"),
            new KeyValuePair<int, string>(25, "25 m"),
            new KeyValuePair<int, string>(50, "50 m"),
            new KeyValuePair<int, string>(100, "100 m"),
            new KeyValuePair<int, string>(500, "500 m"),
            new KeyValuePair<int, string>(1000, "1 km")
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_settings);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarExport);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Settings";
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _settingsService = new SettingsService();

            _spinnerMinTime = FindViewById<Spinner>(Resource.Id.spinnerMinTime);
            var spinnerMinTimeAdapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, _minTimeMapping.Select(i => i.Value).ToList());
            _spinnerMinTime.Adapter = spinnerMinTimeAdapter;

            _spinnerMinDistance = FindViewById<Spinner>(Resource.Id.spinnerMinDistance);
            var spinnerMinDistanceAdapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, _minDistanceMapping.Select(i => i.Value).ToArray());
            _spinnerMinDistance.Adapter = spinnerMinDistanceAdapter;

            _checkBoxIsTelegramUploadEnabled = FindViewById<CheckBox>(Resource.Id.checkBoxIsTelegramUploadEnabled);
            //_editTextUploadUrl = FindViewById<EditText>(Resource.Id.editTextUploadUrl);

            _editTextTelegramBotToken = FindViewById<EditText>(Resource.Id.editTextTelegramBotToken);
            _editTextTelegramChatId = FindViewById<EditText>(Resource.Id.editTextTelegramChatId);

            _checkBoxIsEmailSendingEnabled = FindViewById<CheckBox>(Resource.Id.checkBoxIsEmailSendingEnabled);
            _editTextSmtpPort = FindViewById<EditText>(Resource.Id.editTextSmtpPort);
            _editTextSmtpHost = FindViewById<EditText>(Resource.Id.editTextSmtpHost);
            _editTextSmtpUsername = FindViewById<EditText>(Resource.Id.editTextSmtpUsername);
            _editTextSmtpPassword = FindViewById<EditText>(Resource.Id.editTextSmtpPassword);
            _editTextEmailRecipient = FindViewById<EditText>(Resource.Id.editTextSmtpRecipient);
            _editTextEmailSendingInterval = FindViewById<EditText>(Resource.Id.editTextEmailSendingInterval);
            _editTextEmailSubject = FindViewById<EditText>(Resource.Id.editTextEmailSubject);

            LoadSettings();

            _buttonSave = FindViewById<Button>(Resource.Id.buttonSave);
            _buttonSave.Click += ButtonSave_Click;
        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            var minTime = _minTimeMapping.First(i => i.Value == _spinnerMinTime.SelectedItem.ToString());
            var minDistance = _minDistanceMapping.First(i => i.Value == _spinnerMinDistance.SelectedItem.ToString());

            var settings = new SettingsModel
            {
                MinTime = minTime.Key,
                MinDistance = minDistance.Key,
                IsTelegramUploadEnabled = _checkBoxIsTelegramUploadEnabled.Checked,
                TelegramBotToken = _editTextTelegramBotToken.Text,
                TelegramChatId = _editTextTelegramChatId.Text,
                IsEmailSendingEnabled = _checkBoxIsEmailSendingEnabled.Checked,
                SmtpPort = int.Parse(_editTextSmtpPort.Text),
                SmtpHost = _editTextSmtpHost.Text,
                SmtpUsername = _editTextSmtpUsername.Text,
                SmtpPassword = _editTextSmtpPassword.Text,
                EmailRecipient = _editTextEmailRecipient.Text,
                EmailSendingInterval = int.Parse(_editTextEmailSendingInterval.Text),
                EmailSubject = _editTextEmailSubject.Text
            };

            _settingsService.SaveSettings(settings);
        }

        private void LoadSettings()
        {
            var settings = _settingsService.GetSettings();

            var minTimeItem = _minTimeMapping.First(i => i.Key == settings.MinTime);
            _spinnerMinTime.SetSelection(_minTimeMapping.IndexOf(minTimeItem));

            var minDistanceItem = _minDistanceMapping.First(i => i.Key == settings.MinDistance);
            _spinnerMinDistance.SetSelection(_minDistanceMapping.IndexOf(minDistanceItem));

            _checkBoxIsTelegramUploadEnabled.Checked = settings.IsTelegramUploadEnabled;
            //_editTextUploadUrl.Text = settings.UploadUrl;

            _editTextTelegramBotToken.Text = settings.TelegramBotToken;
            _editTextTelegramChatId.Text = settings.TelegramChatId;

            _checkBoxIsEmailSendingEnabled.Checked = settings.IsEmailSendingEnabled;
            _editTextSmtpPort.Text = settings.SmtpPort.ToString();
            _editTextSmtpHost.Text = settings.SmtpHost;
            _editTextSmtpUsername.Text = settings.SmtpUsername;
            _editTextSmtpPassword.Text = settings.SmtpPassword;
            _editTextEmailRecipient.Text = settings.EmailRecipient;
            _editTextEmailSendingInterval.Text = settings.EmailSendingInterval.ToString();
            _editTextEmailSubject.Text = settings.EmailSubject;
        }
    }
}