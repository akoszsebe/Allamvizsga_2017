using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace Allamvizsga2017.Models
{
    class MyBarChartAdapter : BaseAdapter
    {

        List<ListViewItemBarChart> itemList;
        Context context;
        int itemlistmax = 0;

        public MyBarChartAdapter(Context context)
        {
            this.context = context;
            itemList = new List<ListViewItemBarChart>();
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
            var view = inflater.Inflate(Resource.Layout.barChartlistview_item, parent, false);
            var tvdate = view.FindViewById<TextView>(Resource.Id.tvDate);
            var tvvalueline = view.FindViewById<TextView>(Resource.Id.tvValueline);
            var tvvalue = view.FindViewById<TextView>(Resource.Id.tvValue);

            int linewidth = System.Convert.ToInt32(itemlistmax / (parent.Resources.DisplayMetrics.WidthPixels / parent.Resources.DisplayMetrics.Density - 110));

            tvdate.Text = itemList[position].date;
            tvvalueline.SetWidth(itemList[position].value/ linewidth + 10);
            tvvalueline.SetBackgroundResource(Resource.Drawable.Devicetheme);
            tvvalue.Text = itemList[position].value.ToString() + " KW ";
            return view;
        }

        public void AddData(List<ListViewItemBarChart> items,int max)
        {
            itemList.Clear();
            itemList.AddRange(items);
            itemlistmax = max;
        }

    }
    class ListViewItemBarChart
    {
        public long Id { get; set; }

        public string date { get; set; }

        public int value { get; set; }


        public ListViewItemBarChart(long id, string date, int value)
        {
            this.Id = id;
            this.date = date;
            this.value = value;
        }
    }
}