using Android.App;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Quobject.SocketIoClientDotNet.Client;
using Allamvizsga2017.Activities;
using Android.Content;

namespace Allamvizsga2017
{
    [Activity(Label = "Allamvizsga" , ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        public Socket socket = IO.Socket("http://192.168.0.10:3000");
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //socket.Connect().Emit("new user", "Akomaister");

            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var house_name = Intent.GetStringExtra("House_name");
            string house_id = Intent.GetStringExtra("House_id");


            var tabLayout = FindViewById<TabLayout>(Resource.Id.sliding_tabs);
            var pager = FindViewById<ViewPager>(Resource.Id.pager);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar1);
            var adapter = new CustomPagerAdapter(this, SupportFragmentManager, house_name, house_id, socket);
            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);

            if (house_name == "")
                SupportActionBar.Title = "Id: "+house_id;
            else
                SupportActionBar.Title = house_name;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            toolbar.SetTitleTextAppearance(this, Resource.Style.ActionBarTitle);

            adapter.SetTabTitles("Devices", "Statistics");
            adapter.SetTabIcons(Resource.Drawable.selector_devices,Resource.Drawable.selector_statistics);

            pager.Adapter = adapter;
            pager.SetCurrentItem(1, true);
            // Setup tablayout with view pager

            tabLayout.SetupWithViewPager(pager);
            
            // Iterate over all tabs and set the custom view

            TabLayout.Tab tab1 = tabLayout.GetTabAt(0);
            tab1.SetCustomView(adapter.GetTabView(0));
            tabLayout.GetTabAt(0).Select();
            TabLayout.Tab tab2 = tabLayout.GetTabAt(1);
            tab2.SetCustomView(adapter.GetTabView(1));

            tabLayout.TabSelected += delegate
            {
                int pos = tabLayout.SelectedTabPosition;
                pager.SetCurrentItem(pos, true);
                adapter.StartThreads(pos);
                adapter.StopThreads((pos + 1) % 2);
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_devicespage, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                Finish();
            if (item.ItemId == Resource.Id.menu_logout)
            {
                ISharedPreferences sharedPref = GetSharedPreferences("user_email", FileCreationMode.Private);
                ISharedPreferencesEditor editor = sharedPref.Edit();
                editor.PutString("user_email", null);
                editor.Commit();
                var loginactivity = new Android.Content.Intent(this, typeof(LoginActivity));
                FinishAffinity();
                this.StartActivity(loginactivity);
                Finish();
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }
    }
}

