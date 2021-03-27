using System;
using System.IO;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using SQLite;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using GpsTracker.Database;

namespace GpsTracker
{
    [Activity(Label = "ExportActivity", Theme = "@style/AppTheme")]
    public class ExportActivity : AppCompatActivity
    {
        private Button _exportButton;
        private EditText _textDateFrom;
        private EditText _textDateTo;
        private Spinner _spinnerFormat;
        private EventHandler<DatePickerDialog.DateSetEventArgs> _setDateFromEventHandler;
        private EventHandler<DatePickerDialog.DateSetEventArgs> _setDateToEventHandler;

        private List<string> _exportFormats = new List<string>
        {
            "gpx",
            "json"
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_export);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarExport);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Export";
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _setDateFromEventHandler = new EventHandler<DatePickerDialog.DateSetEventArgs>(SetDateFromEvent);
            _setDateToEventHandler = new EventHandler<DatePickerDialog.DateSetEventArgs>(SetDateToEvent);

            _exportButton = FindViewById<Button>(Resource.Id.buttonExport);
            _exportButton.Click += exportButton_Click;

            _textDateFrom = FindViewById<EditText>(Resource.Id.textDateFrom);
            _textDateFrom.Click += textDateFrom_Click;
            _textDateFrom.Enabled = true;
            _textDateFrom.Clickable = true;
            _textDateFrom.Focusable = false;

            _textDateTo = FindViewById<EditText>(Resource.Id.textDateTo);
            _textDateTo.Click += textDateTo_Click;
            _textDateTo.Enabled = true;
            _textDateTo.Clickable = true;
            _textDateTo.Focusable = false;

            _spinnerFormat = FindViewById<Spinner>(Resource.Id.spinnerFormat);
            var formatAdapter = new ArrayAdapter<string>(this, Resource.Layout.support_simple_spinner_dropdown_item, _exportFormats);
            _spinnerFormat.Adapter = formatAdapter;
        }

        private void textDateTo_Click(object sender, EventArgs e)
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month - 1; // java.util.Calendar works this way!
            var day = DateTime.Now.Day;

            new DatePickerDialog(this, _setDateToEventHandler, year, month, day).Show();
        }

        private void textDateFrom_Click(object sender, EventArgs e)
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month -1; // java.util.Calendar works this way!
            var day = DateTime.Now.Day;

            new DatePickerDialog(this, _setDateFromEventHandler, year, month, day).Show();
        }

        private void SetDateFromEvent(object sender, DatePickerDialog.DateSetEventArgs args)
        {
            var date = GetDateFromDatePicker(args, false);

            _textDateFrom.Text = date.ToString("yyyy-MM-dd");
        }

        private void SetDateToEvent(object sender, DatePickerDialog.DateSetEventArgs args)
        {
            var date = GetDateFromDatePicker(args, true);

            _textDateTo.Text = date.ToString("yyyy-MM-dd");
        }

        private DateTime GetDateFromDatePicker(DatePickerDialog.DateSetEventArgs args, bool isEnd)
        {
            var year = args.Year == 0 ? 1 : args.Year;
            var month = args.Month == 0 ? 1 : args.Month +1; // java.util.Calendar works this way!
            var day = args.DayOfMonth == 0 ? 1 : args.DayOfMonth;
            var hour = isEnd ? 23 : 0;
            var minute = isEnd ? 59 : 0;
            var second = isEnd ? 59 : 0;

            return new DateTime(year, month, day, hour, minute, second);
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            var sendIntent = new Intent();
            sendIntent.SetAction(Intent.ActionSend);

            var dateFromValid = DateTime.TryParse(_textDateFrom.Text, out var fromDate);
            var dateToValid = DateTime.TryParse(_textDateTo.Text, out var toDate);

            if (!dateFromValid || !dateToValid)
            {
                return;
            }

            // Write file to external folder (Android/data/.../)
            if (Android.OS.Environment.ExternalStorageState == Android.OS.Environment.MediaMounted)
            {
                try
                {
                    _exportButton.Clickable = false;

                    var format = _spinnerFormat.SelectedItem.ToString();

                    var path = Path.Combine(GetExternalFilesDir(null).Path, $"file-{DateTime.Now.Ticks}.{format}");

                    var content = CreateContent(format);

                    File.WriteAllText(path, content);

                    Toast.MakeText(this, $@"File exported to: {path}", ToastLength.Short).Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Failed to export file", ToastLength.Short).Show();
                }
                finally
                {
                    _exportButton.Clickable = true;
                }
            }
            else
            {
                Toast.MakeText(this, "External storage is not available", ToastLength.Short).Show();
            }

            string CreateContent(string format)
            {
                var exporter = new Exporter();

                switch (format)
                {
                    case "gpx":
                        return exporter.CreateGpx(fromDate, toDate);
                    case "json":
                        return exporter.CreateJson(fromDate, toDate);
                    default:
                        return string.Empty;
                }
            }
        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }
    }
}