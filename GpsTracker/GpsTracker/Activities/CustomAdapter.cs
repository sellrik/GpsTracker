using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GpsTracker.Activities
{
    internal class CustomAdapter : BaseAdapter<string>
    {
        LayoutInflater _infalter;
        private List<string> _items = new List<string>();

        public CustomAdapter(LayoutInflater inflater, List<string> items)
        {
            _infalter = inflater;
            _items = items;
            _items.Reverse();
        }

        public override string this[int position] => _items[position];

        public override int Count => _items.Count;

        public override Java.Lang.Object GetItem(int position)
        {
            return _items[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var index = Math.Abs(_items.Count - 1 - position);
            var item = _items[index];

            convertView = _infalter.Inflate(Android.Resource.Layout.SimpleListItem1, null);

            convertView.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item;

            return convertView;
        }
    }
}