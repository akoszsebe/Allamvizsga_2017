using System.Collections.Generic;

using Android.Content;
using Android.Views;
using Android.Widget;
using System.Threading;
using Android.App;

namespace Allamvizsga2017.Models
{
    class MyHouseSearchAdapter : BaseAdapter
    {

        List<ListViewItemHouse> itemList;
        Context context;
        Activity activity;
        string user_email;

        public MyHouseSearchAdapter(Activity context, string user_email)
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
            var viewfordelete = view.FindViewById<ImageView>(Resource.Id.viewfordelete);

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
                                alert.SetTitle("House adding error");
                                alert.SetMessage("Try again later");
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
                btadd.Left += 10;
                btadd.SetImageResource(Resource.Drawable.done_green);
            }

            int width = 0;
            int initialx = 0;
            int currentx = 0;
            Android.Support.Design.Widget.AppBarLayout.LayoutParams lparams = new Android.Support.Design.Widget.AppBarLayout.LayoutParams(width, ViewGroup.LayoutParams.MatchParent);
            view.Touch += (v, e) =>
                {
                    if (e.Event.Action == MotionEventActions.Down)
                    {
                        width = 0;
                        lparams.Width = width;
                        viewfordelete.LayoutParameters = lparams;
                        initialx = (int)e.Event.GetX();
                        currentx = (int)e.Event.GetX();
                    }
                    if (e.Event.Action == MotionEventActions.Move)
                    {
                        currentx = (int)e.Event.GetX();
                        width = -(currentx - initialx);
                        if (width < 150)
                            lparams.Width = width;
                        viewfordelete.LayoutParameters = lparams;
                    }
                    if (e.Event.Action == MotionEventActions.Up)
                    {

                    }
                };
            return view;
        }

        public void AddData(List<ListViewItemHouse> items)
        {
            itemList.Clear();
            itemList.AddRange(items);
        }
    }
}