using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Allamvizsga2017.Fragments;
using Quobject.SocketIoClientDotNet.Client;

namespace Allamvizsga2017
{
    public class CustomPagerAdapter : FragmentPagerAdapter
    {
        const int PAGE_COUNT = 2;
        readonly Context context;
        readonly string house_name;
        readonly int house_id;
        //private Fragment[] fragment = new Fragment[2];
        DevicesListViewFragment devicesfragment;
        StatisticsPageFragment statisticsfragemnt;
        private string[] tabtitles = { "", "" };
        private int[] tabicons = { 0, 0 };

        public CustomPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CustomPagerAdapter(Context context, FragmentManager fm,string house_name, int house_id, Socket socket) : base(fm)
        {
            this.context = context;
            this.house_name = house_name;
            this.house_id = house_id;
            //fragment[0] = DevicesListViewFragment.newInstance(house_name, house_id, socket);
            //fragment[1] = StatisticsPageFragment.newInstance(house_name, house_id);
            devicesfragment = DevicesListViewFragment.newInstance(house_name, house_id, socket);
            statisticsfragemnt = StatisticsPageFragment.newInstance(house_name, house_id);
        }

        public override int Count
        {
          
            get { return PAGE_COUNT; }
        }

       
        public override  Fragment GetItem(int position)
        {
            switch(position)
            {
                case 0:
                    return devicesfragment;//fragment[0];               
                case 1:
                    return statisticsfragemnt;//fragment[1];
                default:
                    return devicesfragment;// fragment[0];
            }
        }


        public void SetTabTitles(string tab1,string tab2)
        {
            tabtitles = new string[2];
            tabtitles[0] = tab1;
            tabtitles[1] = tab2;
        }

        public void SetTabIcons(int tab1, int tab2)
        {
            tabicons = new int[2];
            tabicons[0] = tab1;
            tabicons[1] = tab2;
        }

        public View GetTabView(int position)
        {
            var iv = (ImageView)LayoutInflater.From(context).Inflate(Resource.Layout.custom_tab, null);
            iv.SetImageResource(tabicons[position]);
            return iv;
        }

        public void StopThreads(int fragemntindex)
        {
            if (fragemntindex == 0)
            {
                devicesfragment.isSelected = false;
                devicesfragment.OnStop();
            }
            else
            {
                statisticsfragemnt.isSelected = false;
                statisticsfragemnt.OnStop();
            }
            
            //fragment[fragemntindex].OnStop();
        }


        public void StartThreads(int fragemntindex)
        {
            if (fragemntindex == 0)
            {
                devicesfragment.isSelected = true;
                devicesfragment.OnStart();
            }
            else
            {
                statisticsfragemnt.isSelected = true;
                statisticsfragemnt.OnStart();
            }

            //fragment[fragemntindex].OnStart();
        }


    }
}