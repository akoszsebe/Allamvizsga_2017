using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Allamvizsga2017.Models;
using System.Threading;
using com.refractored.fab;
using Android.Graphics;

namespace Allamvizsga2017.Activities
{
    [Activity(Label = "HousesActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class HousesActivity : AppCompatActivity
    {
        private Thread thread;
        ListView mlistview;
        MyHouseListAdapter adapter;
        string user_email;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Houses);

            user_email = Intent.GetStringExtra("User_email");

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar1);
           
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Hi, " + user_email;

            toolbar.SetTitleTextAppearance(this, Resource.Style.ActionBarTitle);

            mlistview = FindViewById<ListView>(Resource.Id.listViewHouses);
            var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //fab.AttachToListView(mlistview);
            fab.SetPadding(35, 35, 35, 35);

            var fabsmartwatch = FindViewById<FloatingActionButton>(Resource.Id.fabforaddSmartWatch);
            //fabsmartwatch.AttachToListView(mlistview);
            fabsmartwatch.Size = FabSize.Mini;
            fabsmartwatch.SetPadding(20, 20, 20, 20);

            fab.Click += delegate
            {
                var searchactivity = new Android.Content.Intent(this, typeof(HouseSearchActivity));
                searchactivity.PutExtra("User_email", user_email);
                this.StartActivity(searchactivity);
            };

            fabsmartwatch.Click += delegate
            {
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this,Resource.Style.MyAlertDialogStyle);
                alert.SetTitle("Add SmartWatch");
                TextView tvid = new TextView(this);
                tvid.Text = "Id :";
                tvid.PaintFlags = Android.Graphics.PaintFlags.FakeBoldText;
                tvid.TextSize = 20;
                EditText input = new EditText(this);
                input.Background.SetColorFilter(Color.Rgb(76, 201, 136), PorterDuff.Mode.SrcIn);
                LinearLayout container = new LinearLayout(this);
                LinearLayout.LayoutParams ll = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                ll.SetMargins(5, 0, 5, 0);
                input.LayoutParameters = ll;
                container.AddView(tvid);
                container.AddView(input);
                alert.SetView(container);
                alert.SetPositiveButton("Add", (senderAlert, args) =>
                {
                    ProgressDialog progress = new ProgressDialog(this);
                    progress.Indeterminate = true;
                    progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progress.SetMessage("Adding SmartWatch...");
                    progress.SetCancelable(true);
                    progress.Show();
                    new Thread(new ThreadStart(() =>
                    {
                        var successed = false;
                        if (input.Text != "")
                        {
                            successed = RestClient.AddUserSmartWatch(user_email, input.Text);
                        }
                        if (successed)
                        {
                            RunOnUiThread(() =>
                            {
                                progress.Dismiss();
                                Android.Support.V7.App.AlertDialog.Builder alertdilaog = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.MyAlertDialogStyle);
                                alertdilaog.SetTitle("SmartWatch successfully added");
                                alertdilaog.SetPositiveButton("OK", (s, a) =>
                                {
                                });
                                Dialog _dialog = alertdilaog.Create();
                                _dialog.Show();
                            });
                        }
                        else
                        {
                            RunOnUiThread(() =>
                            {
                                progress.Dismiss();
                                Android.Support.V7.App.AlertDialog.Builder alertdilaog = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.MyAlertDialogStyle);
                                alertdilaog.SetTitle("Failed");
                                alertdilaog.SetPositiveButton("OK", (s, a) =>
                                {
                                });
                                Dialog _dialog = alertdilaog.Create();
                                _dialog.Show();
                            });
                        }
                    })).Start();
                });
                alert.SetNeutralButton("Cancel", (senderAlert, args) =>
                {
                });
                Dialog dialog = alert.Create();
                dialog.Show();
            };          

            adapter = new MyHouseListAdapter(this,user_email);
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


        protected override void OnPause()
        {
            if (thread != null)
                thread.Abort();
            base.OnPause();
        }


        protected override void OnStart()
        {
            base.OnStart();
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
            });
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
                    adapter.UpdateData(activedevicescount,sumwat, h.house_id);
                    
                    RunOnUiThread(() => { adapter.NotifyDataSetChanged(); });
                }
                catch(System.Exception e)
                {

                }
            }

        }


    }
}