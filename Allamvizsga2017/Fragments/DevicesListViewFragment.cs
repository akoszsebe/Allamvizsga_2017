using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using System.Threading;
using System.Collections.Generic;
using Allamvizsga2017.Models;
using Quobject.SocketIoClientDotNet.Client;
using System;
using Newtonsoft.Json;

namespace Allamvizsga2017.Fragments
{

    public class DevicesListViewFragment : Fragment
    {
        private string house_name = "";
        private string house_id = "";
        private Thread thread;
        private Thread devicethread;
        private ListView mlistview;
        private ListView mlistviewall;
        private MyDeviceAdapter adapter;
        private MyDeviceAdapter adapterall;
        private ProgressBar progressbar_loading;

        public bool isSelected { get; set; } = true;

        //Socket socket;// = IO.Socket("http://192.168.0.10:3000");


        public static DevicesListViewFragment newInstance(string house_name, string house_id, Socket socket)
        {
            PassSocket psocket = new PassSocket();
            psocket.socket = socket;
            var args = new Bundle();
            args.PutString("house_name", house_name);
            args.PutString("house_id", house_id);
            args.PutSerializable("Socket", psocket);
            var fragment = new DevicesListViewFragment();
            fragment.Arguments = args;
            return fragment;
        }


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            house_name = Arguments.GetString("house_name");
            house_id = Arguments.GetString("house_id");
            //socket = (Arguments.GetSerializable("Socket") as PassSocket).socket;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.DevicesListView, container, false);
            mlistview = view.FindViewById<ListView>(Resource.Id.listView1);
            mlistviewall = view.FindViewById<ListView>(Resource.Id.listView2);
            adapter = new MyDeviceAdapter(this.Context, house_id,house_name);
            adapterall = new MyDeviceAdapter(this.Context, house_id, house_name);
            progressbar_loading = view.FindViewById<ProgressBar>(Resource.Id.progressbar_loading);
            mlistview.Adapter = adapter;
            mlistviewall.Adapter = adapterall;
            progressbar_loading.Visibility = ViewStates.Gone;
            return view;
        }


        private void AmperActualizator()
        {
            if (thread != null)
                thread.Abort();
            thread = new Thread(threadGetDataDb);
            thread.Start();

        }
        private void DeviceActualizator()
        {
            if (devicethread != null)
                devicethread.Abort();
            devicethread = new Thread(threadGetDevicesDb);
            devicethread.Start();
        }

        private void threadGetDataDb()
        {
            List<ListViewItemDevice> items;
            try
            {
                while (true)
                {
                    List<Device> amper = RestClient.GetActualDevicesWithSetting(house_id);

                    if (amper != null)
                    {
                        items = new List<ListViewItemDevice>();
                        for (int i = 0; i < amper.Count; i++)
                        {
                            items.Add(new ListViewItemDevice(i, amper[i],true));
                        }
                        Activity.RunOnUiThread(() =>
                        {
                            progressbar_loading.Visibility = ViewStates.Gone;
                            adapter.AddData(items);
                            adapter.NotifyDataSetChanged();
                            ListUtils.setDynamicHeight(mlistview);
                        });
                    }
                    else
                    {
                        items = new List<ListViewItemDevice>();
                        Activity.RunOnUiThread(() =>
                        {
                           
                            progressbar_loading.Visibility = ViewStates.Visible;
                           
                            adapter.AddData(items);
                            adapter.NotifyDataSetChanged();
                            ListUtils.setDynamicHeight(mlistview);
                        });
                    }
                   
                    Thread.Sleep(3000);
                }
            }
            catch (ThreadInterruptedException ex)
            {
                Android.Util.Log.Error("MYAPP", "exception", ex);
            }
        }

        private void threadGetDevicesDb()
        {
            List<Device> device = null;
            while (device == null)
            {
                Thread.Sleep(1000);
                device = RestClient.GetDeviceSetting(house_id);
            }
            List<ListViewItemDevice> items;
            items = new List<ListViewItemDevice>();
            
            if (device != null)
                for (int i = 0; i < device.Count; i++)
                {
                    items.Add(new ListViewItemDevice(i, device[i].icon_id, device[i].name, device[i].value,device[i].value,device[i].valuedelay));
                }
            Activity.RunOnUiThread(() =>
            {
                adapterall.AddData(items);
                adapterall.NotifyDataSetChanged();
                ListUtils.setDynamicHeight(mlistviewall);
            });
        }

        public override void OnPause()
        {
            if (thread != null)
                thread.Abort();
            if (devicethread != null)
                devicethread.Abort();
            base.OnPause();
        }



        public override void OnStart()
        {
            base.OnStart();
            if (isSelected)
            {
                AmperActualizator();
                //NewAmperActualizator();
                DeviceActualizator();
            }
        }

        //private void NewAmperActualizator()
        //{
        //    socket.On("actualdevices", data =>
        //    {
        //        // var jobject = data as Newtonsoft.Json.Linq.JToken;
        //        // get the message data values
        //        var amper = JsonConvert.DeserializeObject<List<Amper>>(data.ToString());//= jobject.Value<List<Amper>>("msg");
        //        List<ListViewItemDevice> items;
        //        //var usr = jobject.Value<string>("user");
        //
        //        if (amper != null)
        //        {
        //            items = new List<ListViewItemDevice>();
        //            for (int i = 0; i < amper.Count; i++)
        //            {
        //                Device selector = d.GetDevice(System.Convert.ToInt32(amper[i].ampervalue));
        //                items.Add(new ListViewItemDevice(i, selector.icon_id, selector.name, selector.value, true));
        //            }
        //
        //
        //        }
        //        else
        //        {
        //            items = new List<ListViewItemDevice>();
        //            items.Add(new ListViewItemDevice(0, Resource.Drawable.Loading_icon, "connection failed", -1));
        //        }
        //        Activity.RunOnUiThread(() =>
        //        {
        //            adapter.AddData(items);
        //            adapter.NotifyDataSetChanged();
        //            ListUtils.setDynamicHeight(mlistview);
        //        });
        //    });
        //}

        public override void OnStop()
        {
            if (thread != null)
                thread.Abort();
            if (devicethread != null)
                devicethread.Abort();
            base.OnStop();
        }


        public override void OnResume()
        {
            if (isSelected) { }
            base.OnResume();
        }
    }


    public class ListUtils
    {
        public static void setDynamicHeight(ListView mListView)
        {
            IListAdapter mListAdapter = mListView.Adapter;
            if (mListAdapter == null)
            {
                return;
            }
            int height = 0;
            int desiredWidth = View.MeasureSpec.MakeMeasureSpec(mListView.Width, MeasureSpecMode.Unspecified);
            for (int i = 0; i < mListAdapter.Count; i++)
            {
                View listItem = mListAdapter.GetView(i, null, mListView);
                listItem.Measure(desiredWidth, System.Convert.ToInt32(MeasureSpecMode.Unspecified));
                height += listItem.MeasuredHeight;
            }
            ViewGroup.LayoutParams p = mListView.LayoutParameters;
            p.Height = height + (mListView.DividerHeight * (mListAdapter.Count - 1));
            mListView.LayoutParameters = p;
            mListView.RequestLayout();
        }
    }
}
