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
using Android.Support.V7.App;
using Allamvizsga2017.Models;
using System.Threading;

namespace Allamvizsga2017.Activities
{
    [Activity(Label = "HouseSearchActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class HouseSearchActivity : AppCompatActivity
    {
        ListView mlistview;
        MyHouseSearchAdapter adapter;
        string user_email;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.HouseSearch);
            user_email = Intent.GetStringExtra("User_email");

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar1);
            var searchbar = FindViewById<Android.Support.V7.Widget.SearchView>(Resource.Id.searchView1);
            mlistview = FindViewById<ListView>(Resource.Id.listViewSearchHouse);
            searchbar.QueryHint = "search for houses";

            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Search For Houses";
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            adapter = new MyHouseSearchAdapter(this, user_email);
            mlistview.Adapter = adapter;

            searchbar.Click += delegate
            {
                searchbar.OnActionViewExpanded();
            };

            searchbar.QueryTextChange += delegate
            {
                SearchHouse(searchbar.Query);
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

        void SearchHouse(string house_name)
        {
            List<ListViewItemHouse> items;
            new Thread(new ThreadStart(() =>
            {
                List<House> houses = RestClient.GetHousesByName(house_name);
                if (houses != null)
                {
                    items = new List<ListViewItemHouse>();
                    RunOnUiThread(() =>
                    {
                        for (int i = 0; i < houses.Count; i++)
                        {
                            items.Add(new ListViewItemHouse(i, houses[i].house_id, houses[i].house_name, houses[i].password));
                        }

                        adapter.AddData(items);
                        adapter.NotifyDataSetChanged();
                    });
                }
            })).Start();
        }
    }
}