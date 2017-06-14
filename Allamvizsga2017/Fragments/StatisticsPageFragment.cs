using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using System.Threading;
using Java.Util.Concurrent;
using Allamvizsga2017.Models;
using Android.Widget;
using System.Collections.Generic;
using System;
using Java.Util;

namespace Allamvizsga2017.Fragments
{
    
    public class StatisticsPageFragment : Fragment, DatePicker.IOnDateChangedListener
    {
        private string house_name = "";
        private string house_id = "";
        private Thread thread;
        private ListView mlistview;
        private MyBarChartAdapter adapter;
        private TextView tvtotalkw;
        private DatePicker datepicker;
        public bool isSelected { get; set; } = false;


        public static StatisticsPageFragment newInstance(string house_name, string house_id)
        {
            var args = new Bundle();
            args.PutString("User_name", house_name);
            args.PutString("house_id", house_id);
            var fragment = new StatisticsPageFragment();
            fragment.Arguments = args;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            house_name = Arguments.GetString("house_name");
            house_id = Arguments.GetString("house_id");
        }

        long startTime;
        long endTime;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Statisticslayout, container, false);
            mlistview = view.FindViewById<ListView>(Resource.Id.listViewBarchart);
            datepicker = view.FindViewById<DatePicker>(Resource.Id.datePicker1);
            tvtotalkw = view.FindViewById<TextView>(Resource.Id.textViewTotalKW);

            adapter = new MyBarChartAdapter(this.Context);
            mlistview.Adapter = adapter;

            

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                int daySpinnerId = Resources.GetIdentifier("day", "id", "android");
                if (daySpinnerId != 0)
                {
                    View daySpinner = datepicker.FindViewById(daySpinnerId);
                    if (daySpinner != null)
                    {
                        daySpinner.Visibility = ViewStates.Gone;
                    }
                }
            }
            else
            {
                Java.Lang.Reflect.Field[] f = datepicker.Class.GetDeclaredFields();
                foreach (Java.Lang.Reflect.Field field in f)
                {
                    if (field.Name.Equals("mDaySpinner") || field.Name.Equals("mDayPicker"))
                    {
                        field.Accessible = true;
                        object dayPicker = null;
                        try
                        {
                            dayPicker = field.Get(datepicker);
                        }
                        catch (Java.Lang.IllegalAccessException e)
                        {
                            e.PrintStackTrace();
                        }
                        ((View)dayPicker).Visibility = ViewStates.Gone;
                    }
                }
                
            }
            Calendar calendar = Calendar.GetInstance(Locale.Default);
            datepicker.Init(calendar.Get(CalendarField.Year), calendar.Get(CalendarField.Month), calendar.Get(CalendarField.DayOfMonth),this);
            calendar.Set(CalendarField.Date, calendar.GetActualMinimum(CalendarField.Date));
            startTime = calendar.TimeInMillis;
            calendar.Set(CalendarField.Date, calendar.GetActualMaximum(CalendarField.Date));
            endTime = calendar.TimeInMillis;
            //FeedFromDb(startTime,endTime);
            return view;
        }

        public void OnDateChanged(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            Calendar calendar = Calendar.GetInstance(Locale.Default);
            calendar.Set(year, monthOfYear,1,0,0);
            startTime = calendar.TimeInMillis;
            calendar.Set(CalendarField.Date, calendar.GetActualMaximum(CalendarField.Date));
            endTime = calendar.TimeInMillis;
            FeedFromDb(startTime, endTime);
        }

        public void SetIsSelected() { }

        public override void OnPause()
        {
            //Console.WriteLine("---------------------- Pause ------------------");
            if (thread != null)
                thread.Abort();
            base.OnPause();
        }


        public override void OnStart()
        {
            if (isSelected)
            {
                FeedFromDb(startTime, endTime);
               // Console.WriteLine("---------------------- Start ------------------");
            }
            base.OnStart();

        }

        public override void OnStop()
        {
            //Console.WriteLine("---------------------- Stop ------------------");
            if (thread != null)
                thread.Abort();
            base.OnStop();
        }


        public override void OnResume()
        {
            if (isSelected)
            {
                //Console.WriteLine("---------------------- Resume ------------------");
            }
            base.OnResume();
        } 

        private void FeedFromDb(long datefrom,long dateto)
        {
            if (thread != null)
                thread.Abort();
            thread = new Thread(() => threadGetDataDb(datefrom,dateto));
            thread.Start();
        }

        private void threadGetDataDb(long datefrom, long dateto)
        {

            long now = TimeUnit.Milliseconds.ToHours(System.DateTime.Now.Millisecond);
            List<ListViewItemBarChart> items;
            List<Amper> ampers = RestClient.GetAmpers(house_id,datefrom,dateto);
            if (ampers != null)
            {
                items = new List<ListViewItemBarChart>();
                
                int index = 0;
                int max = 0;
                double totalkw = 0;
                for (int i = 0; i < ampers.Count; i++)
                {
                    int date = DayOfWeek.GetDay(ampers[i].amperdate);
                    string dataday = DayOfWeek.GetName(ampers[i].amperdate);
                    double sumvalue = ampers[i].ampervalue;
                    ampers.Remove(ampers[i]);
                    i = 0;
                    for (int j = 0; j < ampers.Count; j++)
                    {

                        if (DayOfWeek.GetDay(ampers[j].amperdate) == date)
                        {
                            sumvalue += ampers[j].ampervalue;
                            ampers.Remove(ampers[j]);
                            j--;
                        }

                    }
                    if (sumvalue > max) max = Convert.ToInt32(sumvalue);
                    totalkw += + sumvalue;
                    items.Add(new ListViewItemBarChart(index, dataday, System.Convert.ToInt32(sumvalue)));
                    index++;
                }
                Activity.RunOnUiThread(() =>
                {
                    tvtotalkw.Text = Context.Resources.GetString(Resource.String.textview_total)+" "+ totalkw + " kWh";
                    adapter.AddData(items, max);
                    adapter.NotifyDataSetChanged();
                    ListUtils.setDynamicHeight(mlistview);
                });
            }
        }

    }

    public class DayOfWeek
    {
        public static string GetName(long date)
        {
            Java.Util.Calendar cal = Java.Util.Calendar.GetInstance(Java.Util.Locale.English);
            cal.Time = new Java.Util.Date(date);
            int dayofmonth = cal.Get(Java.Util.CalendarField.DayOfMonth);
            int dayofweek = cal.Get(Java.Util.CalendarField.DayOfWeek);
            switch (dayofweek)
            {
                case 1:
                    return "Sun " + dayofmonth;
                case 2:
                    return "Mon " + dayofmonth;
                case 3:
                    return "Tue " + dayofmonth;
                case 4:
                    return "Wed " + dayofmonth;
                case 5:
                    return "Thu " + dayofmonth;
                case 6:
                    return "Fri " + dayofmonth;
                case 7:
                    return "Sat " + dayofmonth;
                default:
                    return "";
            }
        }

        public static int GetDay(long date)
        {
            Java.Util.Calendar cal = Java.Util.Calendar.GetInstance(Java.Util.Locale.English);
            cal.Time = new Java.Util.Date(date);
            return cal.Get(Java.Util.CalendarField.DayOfMonth);
        }

        

    }

}