
using Allamvizsga2017.Activities;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Threading;

namespace Allamvizsga2017.Models
{
    class MyHouseListAdapter : BaseAdapter
    {

        List<ListViewItemHouse> itemList;
        Context context;
        Activity activity;
        string user_email;

        public MyHouseListAdapter(Activity context, string user_email)
        {
            this.activity = context;
            this.context = context;
            this.user_email = user_email;
            itemList = new List<ListViewItemHouse>();
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
            var view = inflater.Inflate(Resource.Layout.houselistview_item, parent, false);
            var tvhousename = view.FindViewById<TextView>(Resource.Id.tvHouseName);
            var tvactivedevicenumber = view.FindViewById<TextView>(Resource.Id.tvActiveDevicesNumber);
            var tvValue = view.FindViewById<TextView>(Resource.Id.tvValue);
            var ivdelete = view.FindViewById<ImageView>(Resource.Id.imageviewDeleteUserHouse);
            var ivhouseicon = view.FindViewById<ImageView>(Resource.Id.imageviewHouseColoricon);

            tvhousename.Text = itemList[position].house_name;
            tvactivedevicenumber.Text = "Active " + itemList[position].activedevices.ToString() + " devices";
            tvValue.Text = itemList[position].sumwat + "W";
            ivhouseicon.SetImageResource(HouseSelector.GetIconId(itemList[position].sumwat));

            var backgroundcolor = view.Background;

            view.Touch += (s, e) =>
            {
                view.SetBackgroundColor(Android.Graphics.Color.Rgb(224, 226, 225));
                if (e.Event.Action == MotionEventActions.Up)
                {
                    var mainactivity = new Intent(context, typeof(MainActivity));
                    mainactivity.PutExtra("House_name", itemList[position].house_name);
                    mainactivity.PutExtra("House_id", itemList[position].house_id);
                    context.StartActivity(mainactivity);
                    view.Background = backgroundcolor;
                }
                else if (e.Event.Action == MotionEventActions.Cancel)
                {
                    view.Background = backgroundcolor;
                }
            };

            ivdelete.Click += delegate
            {
                ProgressDialog progress = new ProgressDialog(context);
                progress.Indeterminate = true;
                progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                progress.SetMessage("Deleting House...");
                progress.SetCancelable(true);
                progress.Show();
                new Thread(new ThreadStart(() =>
                {
                    var successed = RestClient.DeleteUserHouse(user_email, itemList[position].house_id);
                    if (successed)
                    {
                        activity.RunOnUiThread(() =>
                        {
                            itemList.Remove(itemList[position]);
                            NotifyDataSetChanged();
                            progress.Dismiss();
                            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(context,Resource.Style.MyAlertDialogStyle);
                            alert.SetTitle("House Successfully Deleted");
                            alert.SetPositiveButton("OK", (senderAlert, args) =>
                            {
                            });
                            Dialog dialog = alert.Create();
                            dialog.Show();
                        });
                    }
                    else
                    {
                        activity.RunOnUiThread(() =>
                        {
                            progress.Dismiss();
                            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(context, Resource.Style.MyAlertDialogStyle);
                            alert.SetTitle("Please Try Again");
                            alert.SetPositiveButton("OK", (senderAlert, args) =>
                            {
                            });
                            Dialog dialog = alert.Create();
                            dialog.Show();
                        });
                    }
                })).Start();
            };

            return view;
        }

        public void AddData(List<ListViewItemHouse> items)
        {
            itemList.Clear();
            itemList.AddRange(items);
        }

        public void UpdateData(int devicenumber, int s_wat, int h_id)
        {
            itemList.Find(x => x.house_id == h_id).activedevices = devicenumber;
            itemList.Find(x => x.house_id == h_id).sumwat = s_wat;
        }



    }

    class HouseSelector
    {

        public static int GetIconId(int value)
        {
            if (value <= 1000)
            {
                return Resource.Drawable.house_green;
            }
            if (value <= 5000)
            {
                return Resource.Drawable.house_yellow;
            }
            if (value <= 10000)
            {
                return Resource.Drawable.house_orange;
            }
            return Resource.Drawable.house_red;
        }
    }


    class ListViewItemHouse
    {
        public long Id { get; set; }
        public int house_id { get; set; }
        public string house_name { get; set; }
        public int activedevices { get; set; } = 0;
        public int sumwat { get; set; } = 0;
        public string password { get; set; }
        public bool users_house { get; set; } = false;

        public ListViewItemHouse(long id, int house_id, string house_name, string password)
        {
            this.Id = id;
            this.house_id = house_id;
            this.house_name = house_name;
            this.password = password;
        }


        public ListViewItemHouse(long id, int house_id, string house_name, string password, bool users_house)
        {
            this.Id = id;
            this.house_id = house_id;
            this.house_name = house_name;
            this.password = password;
            this.users_house = users_house;
        }

        public ListViewItemHouse(long id, int house_id, string house_name, string password, int activedevices,int sumwat)
        {
            this.Id = id;
            this.house_id = house_id;
            this.house_name = house_name;
            this.password = password;
            this.activedevices = activedevices;
            this.sumwat = sumwat;
        }
    }
}