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

namespace Allamvizsga2017.Models
{
    class MySmartWatchListAdapter : BaseAdapter
    {

        List<ListViewItemSmartWatch> itemList;
        Context context;
        Activity activity;
        string user_email;

        public MySmartWatchListAdapter(Activity context, string user_email)
        {
            this.activity = context;
            this.context = context;
            this.user_email = user_email;
            itemList = new List<ListViewItemSmartWatch>();
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
            var btadd = view.FindViewById<ImageView>(Resource.Id.imageviewHouseSearch);
            var viewfordelete = view.FindViewById<ImageView>(Resource.Id.viewfordelete);

            tvhouse_name.Text = itemList[position].smartwatch_id;

            int tmp = position;

            btadd.LayoutParameters.Width += 10;
            btadd.Left += 10;
            btadd.SetImageResource(Resource.Drawable.done_green);


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

        public void AddData(List<ListViewItemSmartWatch> items)
        {
            itemList.Clear();
            itemList.AddRange(items);
        }


    }


    class ListViewItemSmartWatch
    {
        public long Id { get; set; }
        public string smartwatch_id { get; set; }

        public ListViewItemSmartWatch(long id, string smartwatch_id)
        {
            this.Id = id;
            this.smartwatch_id = smartwatch_id;

        }
    }
}