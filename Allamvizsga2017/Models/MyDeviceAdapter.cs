using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace Allamvizsga2017.Models
{
    class MyDeviceAdapter : BaseAdapter
    {
        List<ListViewItemDevice> itemList;
        Context context;
        long house_id;
        string house_name = "";

        public MyDeviceAdapter(Context context, long house_id,string house_name)
        {
            this.context = context;
            this.house_id = house_id;
            this.house_name = house_name;
            itemList = new List<ListViewItemDevice>();
        }

        public override int Count
        {
            get { return itemList.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return itemList[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater inflater = (LayoutInflater)context
                .GetSystemService(Context.LayoutInflaterService);


            var view = inflater.Inflate(Resource.Layout.devicelistview_item, parent, false);
            var deviceName = view.FindViewById<TextView>(Resource.Id.DeviceName);
            var deviceImage = view.FindViewById<ImageView>(Resource.Id.DeviceImage);
            var deviceSetting = view.FindViewById<ImageView>(Resource.Id.DeviceSetting);
           
            int icon_id = 0;
            if (itemList[position].iconid != 0)
            {
                deviceImage.SetImageResource(itemList[position].iconid);
                icon_id = itemList[position].iconid;
            }
            else
            {
                deviceImage.SetImageResource(Resource.Drawable.unknownicon);
                icon_id = Resource.Drawable.unknownicon;
            }

            if (itemList[position].active)
            {
                deviceImage.SetBackgroundResource(Resource.Drawable.active);
            }

            if (itemList[position].value == -1)
            {
                deviceName.Text = itemList[position].name;

                deviceSetting.SetImageResource(Resource.Drawable.abc_tab_indicator_material);


            }
            else
            {
                deviceName.Text = itemList[position].name + " " + itemList[position].value;
                int tmpposition = position;
                deviceSetting.Click += delegate
                {
                    var settingsactivity = new Intent(context, typeof(DeviceSettingActivity));
                    settingsactivity.PutExtra("house_name", house_name);
                    settingsactivity.PutExtra("house_id", house_id);
                    settingsactivity.PutExtra("device_name", itemList[tmpposition].name);
                    settingsactivity.PutExtra("device_value", itemList[tmpposition].value);
                    settingsactivity.PutExtra("original_value", itemList[tmpposition].original_value);
                    settingsactivity.PutExtra("icon_id", icon_id);
                    context.StartActivity(settingsactivity);
                };
            }


            view.Click += delegate { };
            return view;
        }
        
        public void AddData(List<ListViewItemDevice> items)
        {
            itemList.Clear();
            itemList.AddRange(items);
        }

    }
    class ListViewItemDevice
    {
        public long Id { get; set; }

        public int iconid { get; set; }

        public string name { get; set; }

        public int value { get; set; }

        public int original_value { get; set; }

        public bool active { get; set; } = false;

        public ListViewItemDevice(long id, int iconid, string name,int value, int original_value)
        {
            this.Id = id;
            this.iconid = iconid;
            this.name = name;
            this.value = value;
            this.original_value = original_value;
        }

        public ListViewItemDevice(long id, int iconid, string name, int value,int original_value,bool active)
        {
            this.Id = id;
            this.iconid = iconid;
            this.name = name;
            this.value = value;
            this.active = active;
            this.original_value = original_value;
        }

        public ListViewItemDevice(long id,Device d,bool active)
        {
            this.Id = id;
            this.iconid = d.icon_id;
            this.name = d.name;
            this.value = d.value;
            this.active = active;
            this.original_value = d.original_value;
        }

    }
}