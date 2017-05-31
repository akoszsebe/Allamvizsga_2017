using System.Collections.Generic;

using Android.Content;
using Android.Views;
using Android.Widget;
using System.Threading;
using Android.App;

namespace Allamvizsga2017.Models
{
    class MyHouseSearchAdapter: BaseAdapter
    {

        List<ListViewItemHouse> itemList;
        Context context;
        Activity activity;
        string user_email;

        public MyHouseSearchAdapter(Activity context,string user_email)
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
            var view = inflater.Inflate(Resource.Layout.housesearchlistview_item, parent, false);
            var tvhouse_name = view.FindViewById<TextView>(Resource.Id.tvHouseName);
            var tvhouse_id = view.FindViewById<TextView>(Resource.Id.tvHouseId);
            var btadd = view.FindViewById<ImageView>(Resource.Id.imageviewHouseSearch);

            tvhouse_name.Text = itemList[position].house_name;
            tvhouse_id.Text += itemList[position].house_id.ToString();

            int tmp = position;

            if (!itemList[position].users_house)
            {
                btadd.Click += delegate
                {
                    ProgressDialog progress = new ProgressDialog(context);
                    progress.Indeterminate = true;
                    progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progress.SetMessage("Adding House...");
                    progress.SetCancelable(true);
                    progress.Show();
                    new Thread(new ThreadStart(() =>
                    {
                        var successed = RestClient.AddUserHouse(user_email, itemList[position].house_id);
                        if (successed)
                        {
                            activity.RunOnUiThread(() =>
                            {
                                progress.Dismiss();                     
                                btadd.LayoutParameters.Width += 10;
                                btadd.SetImageResource(Resource.Drawable.done_green);
                            });
                        }
                        else
                        {
                            activity.RunOnUiThread(() =>
                            {
                                progress.Dismiss();
                                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(context, Resource.Style.MyAlertDialogStyle);
                                alert.SetTitle("House was added");
                                alert.SetPositiveButton("OK", (senderAlert, args) =>
                                {
                                });
                                Dialog dialog = alert.Create();
                                dialog.Show();
                            });
                        }
                    })).Start();
                };
            }
            else
            {
                btadd.LayoutParameters.Width += 10;
                btadd.Left  += 10;
                btadd.SetImageResource(Resource.Drawable.done_green);
            }
            return view;
        }

        public void AddData(List<ListViewItemHouse> items)
        {
            itemList.Clear();
            itemList.AddRange(items);
        }
    }
}