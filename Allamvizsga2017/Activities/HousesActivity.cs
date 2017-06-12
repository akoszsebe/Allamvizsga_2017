using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Allamvizsga2017.Models;
using System.Threading;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Views.Animations;
using Allamvizsga2017.Services;
using System.Collections;
using System.Linq;

namespace Allamvizsga2017.Activities
{
    [Activity(Label = "HousesActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class HousesActivity : AppCompatActivity
    {
        private Thread thread { get; set; }
        private ListView mlistview { get; set; }
        private MyHouseListAdapter adapter { get; set; }
        private string user_email { get; set; }
        private Android.Support.V4.Widget.SwipeRefreshLayout refresher { get; set; }

        private bool isfabopend = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            NotificationStarter.SetContext(this);
            // Create your application here
            SetContentView(Resource.Layout.Houses);

            user_email = Intent.GetStringExtra("User_email");

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar1);

            SetSupportActionBar(toolbar);
            SupportActionBar.SetTitle(Resource.String.toolbar_hi);
            SupportActionBar.Title += " " + user_email;

            toolbar.SetTitleTextAppearance(this, Resource.Style.ActionBarTitle);

            mlistview = FindViewById<ListView>(Resource.Id.listViewHouses);

            refresher = FindViewById<Android.Support.V4.Widget.SwipeRefreshLayout>(Resource.Id.refresher);
            refresher.Refresh += delegate
            {
                FeedFromDb();
            };

            var fabopen = FindViewById<FloatingActionButton>(Resource.Id.fab_1);
            var fabsearchhouse = FindViewById<FloatingActionButton>(Resource.Id.fab_2);
            var fabsmartwatch = FindViewById<FloatingActionButton>(Resource.Id.fab_3);
            var fabregisterhouse = FindViewById<FloatingActionButton>(Resource.Id.fab_4);

            var tvfabadd = FindViewById<TextView>(Resource.Id.textView_fab_add);
            var tvfabsearch = FindViewById<TextView>(Resource.Id.textView_fab_search);
            var tvfabregister = FindViewById<TextView>(Resource.Id.textView_fab_registerhouse);


            var openFab = AnimationUtils.LoadAnimation(this, Resource.Animation.fab_open);
            var openFab_startoffset100 = AnimationUtils.LoadAnimation(this, Resource.Animation.fab_open_startoffset_100);
            var openFab_startoffset200 = AnimationUtils.LoadAnimation(this, Resource.Animation.fab_open_startoffset_200);
            var closeFab = AnimationUtils.LoadAnimation(this, Resource.Animation.fab_close);
            var rotateFab = AnimationUtils.LoadAnimation(this, Resource.Animation.fab_rotate_forward);
            var rotatebackFab = AnimationUtils.LoadAnimation(this, Resource.Animation.fab_rotate_backward);

            var layouttransparent = FindViewById<LinearLayout>(Resource.Id.layouttransparent);

            var layoutcontainer = FindViewById<LinearLayout>(Resource.Id.layoutcontainer);

            fabopen.Click += delegate
            {
                if (isfabopend)
                {
                    fabopen.StartAnimation(rotatebackFab);
                    fabsearchhouse.StartAnimation(closeFab);
                    fabsmartwatch.StartAnimation(closeFab);
                    fabregisterhouse.StartAnimation(closeFab);
                    tvfabadd.StartAnimation(closeFab);
                    tvfabsearch.StartAnimation(closeFab);
                    tvfabregister.StartAnimation(closeFab);
                    fabsearchhouse.Clickable = false;
                    fabsmartwatch.Clickable = false;
                    fabregisterhouse.Clickable = false;
                    tvfabadd.Clickable = false;
                    tvfabsearch.Clickable = false;
                    tvfabregister.Clickable = false;
                    isfabopend = false;
                    layouttransparent.SetBackgroundColor(Color.ParseColor("#00000000"));
                    layouttransparent.Clickable = false;
                }
                else
                {
                    fabopen.StartAnimation(rotateFab);
                    fabsearchhouse.StartAnimation(openFab);
                    fabsmartwatch.StartAnimation(openFab_startoffset100);
                    fabregisterhouse.StartAnimation(openFab_startoffset200);
                    tvfabsearch.StartAnimation(openFab);
                    tvfabadd.StartAnimation(openFab_startoffset100);
                    tvfabregister.StartAnimation(openFab_startoffset200);
                    fabsearchhouse.Clickable = true;
                    fabsmartwatch.Clickable = true;
                    fabregisterhouse.Clickable = true;
                    tvfabadd.Clickable = true;
                    tvfabsearch.Clickable = true;
                    tvfabregister.Clickable = true;
                    isfabopend = true;
                    layouttransparent.SetBackgroundColor(Color.ParseColor("#AA000000"));
                    layouttransparent.Clickable = true;
                }
            };

            layouttransparent.Click += delegate
            {
                fabopen.CallOnClick();
            };
            layouttransparent.Clickable = false;

            tvfabsearch.Click += delegate
            {
                fabopen.CallOnClick();
                SearchAddHouse();
            };
            fabsearchhouse.Click += delegate
            {
                fabopen.CallOnClick();
                SearchAddHouse();
            };

            tvfabadd.Click += delegate
            {
                fabopen.CallOnClick();
                AddSmarWatch();
            };
            fabsmartwatch.Click += delegate
            {
                fabopen.CallOnClick();
                AddSmarWatch();
            };

            fabregisterhouse.Click += delegate
            {
                fabopen.CallOnClick();
                RegisterHouse();
            };

            tvfabregister.Click += delegate
            {
                fabopen.CallOnClick();
                RegisterHouse();
            };

            adapter = new MyHouseListAdapter(this, user_email);
            mlistview.Adapter = adapter;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_devicespage, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
            }
            if (item.ItemId == Resource.Id.menu_logout)
            {
                NotificationStarter.StopNotificationService();
                NotificationStarter.SetNotification_Enabled(false);
                var loginactivity = new Android.Content.Intent(this, typeof(LoginActivity));
                ISharedPreferences sharedPref = GetSharedPreferences("user_email", FileCreationMode.Private);
                ISharedPreferencesEditor editor = sharedPref.Edit();
                editor.PutString("user_email", null);
                editor.Commit();
                FinishAffinity();
                this.StartActivity(loginactivity);                
                Finish();
            }
            return base.OnOptionsItemSelected(item);
        }

        private void SearchAddHouse()
        {
            var searchactivity = new Android.Content.Intent(this, typeof(HouseSearchActivity));
            searchactivity.PutExtra("User_email", user_email);
            this.StartActivity(searchactivity);
            OverridePendingTransition(Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_slide_out_bottom);
        }

        private void RegisterHouse()
        {
            var houseregistration = new Android.Content.Intent(this, typeof(HouseRegistrationActivity));
            houseregistration.PutExtra("User_email", user_email);
            this.StartActivity(houseregistration);
            OverridePendingTransition(Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_slide_out_bottom);
        }

        private void AddSmarWatch()
        {
            var addsmartwatch = new Android.Content.Intent(this, typeof(AddSmartWatchActivity));
            addsmartwatch.PutExtra("User_email", user_email);
            this.StartActivity(addsmartwatch);
            OverridePendingTransition(Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_slide_out_bottom);
        }

        protected override void OnPause()
        {
            if (thread != null)
                thread.Abort();
            base.OnPause();
        }


        protected override void OnStart()
        {
            base.OnStart();
            NotificationStarter.StopNotificationService();
//            NotificationStarter.SetContext(this);
            if (NotificationStarter.GetNotification_Enabled())
                NotificationStarter.StartNotificationService();
            else NotificationStarter.StopNotificationService(); 
            FeedFromDb();
        }

        

        protected override void OnStop()
        {
            if (thread != null)
                thread.Abort();
            base.OnStop();
        }


        protected override void OnResume()
        {
            base.OnResume();
        }


        private void FeedFromDb()
        {
            if (thread != null)
                thread.Abort();
            thread = new Thread(threadGetDataDb);
            thread.Start();
        }

        private void threadGetDataDb()
        {
            List<ListViewItemHouse> items;
            List<House> houses = null;
            while (houses == null)
            {
                houses = RestClient.GetUserHouses(user_email);
                Thread.Sleep(1000);
            }
            items = new List<ListViewItemHouse>();
            RunOnUiThread(() =>
            {
                for (int i = 0; i < houses.Count; i++)
                {
                    items.Add(new ListViewItemHouse(i, houses[i].house_id, houses[i].house_name, houses[i].password));
                }
                adapter.AddData(items);
                adapter.NotifyDataSetChanged();
                if (refresher != null)
                {
                    refresher.Refreshing = false;
                }
            });
            int inc = 0;
            foreach (var h in houses)
            {
                try
                {
                    var d = RestClient.GetActualDevicesWithSetting(h.house_id);
                    var activedevicescount = d.Count;
                    var sumwat = 0;
                    foreach (var r in d)
                    {
                        sumwat += r.value;
                    }

                    adapter.UpdateData(activedevicescount* inc, sumwat*inc, h.house_id);
                    inc++;
                    RunOnUiThread(() => { adapter.NotifyDataSetChanged(); });
                }
                catch (System.Exception e)
                {

                }
            }

        }


    }
}