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
using Android.Views.InputMethods;
using System.Threading;
using Allamvizsga2017.Models;

namespace Allamvizsga2017.Activities
{
    [Activity(Label = "AddSmartWatchActivity", WindowSoftInputMode = SoftInput.StateHidden,
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class AddSmartWatchActivity : AppCompatActivity
    {
        ListView mlistview { get; set; }
        MySmartWatchListAdapter adapter { get; set; }
        string user_email { get; set; }
        List<SmartWatch> user_smartwatches { get; set; } = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddSmartWatch);
            user_email = Intent.GetStringExtra("User_email");

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar1);
            mlistview = FindViewById<ListView>(Resource.Id.listViewAddSmartWatch);
            var tismartwatch_id = FindViewById<EditText>(Resource.Id.textInputUserSmartWatch);
            var btadd = FindViewById<Button>(Resource.Id.buttonAddSmartWatch); 

            SetSupportActionBar(toolbar);
            SupportActionBar.SetTitle(Resource.String.textview_addsmartwatch);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            toolbar.SetTitleTextAppearance(this, Resource.Style.ActionBarTitle);

            adapter = new MySmartWatchListAdapter(this, user_email);
            mlistview.Adapter = adapter;

            GetUserSmartWatches();

            btadd.Click += delegate 
            {
                if (IsMatchIdPattern(tismartwatch_id.Text))
                {
                    AddSmarWatch(tismartwatch_id.Text);
                }
                else
                    tismartwatch_id.Error = "Id: XXXXX";
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_devicespage, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, 0);
            return base.OnTouchEvent(e);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                Finish();
            if (item.ItemId == Resource.Id.menu_logout)
            {
                NotificationStarter.SetNotification_Enabled(false);
                var loginactivity = new Android.Content.Intent(this, typeof(LoginActivity));
                ISharedPreferences sharedPref = GetSharedPreferences("user_email", FileCreationMode.Private);
                ISharedPreferencesEditor editor = sharedPref.Edit();
                editor.PutString("user_email", null);
                editor.Commit();
                FinishAffinity();
                this.StartActivity(loginactivity);
                Xamarin.Facebook.Login.LoginManager.Instance.LogOut();
                Finish();
            }
            return base.OnOptionsItemSelected(item);
        }

        private void GetUserSmartWatches()
        {
            new Thread(new ThreadStart(() =>
            {
                List<ListViewItemSmartWatch> items;
                user_smartwatches = RestClient.GetUserSmartWatches(user_email);
                if (user_smartwatches != null)
                {
                    items = new List<ListViewItemSmartWatch>();
                    RunOnUiThread(() =>
                    {
                        for (int i = 0; i < user_smartwatches.Count; i++)
                        {
                            items.Add(new ListViewItemSmartWatch(i, user_smartwatches[i].smartwatch_id));
                        }
                        adapter.AddData(items);
                        adapter.NotifyDataSetChanged();
                    });
                }
            })).Start();
        }

        private void AddSmarWatch(string smartwatch_id)
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
                if (smartwatch_id != "")
                {
                    successed = RestClient.AddUserSmartWatch(user_email, smartwatch_id);
                }
                if (successed)
                {
                    RunOnUiThread(() =>
                    {
                        GetUserSmartWatches();
                        progress.Dismiss();
                    });
                }
                else
                {
                    RunOnUiThread(() =>
                    {
                        progress.Dismiss();
                        Android.Support.V7.App.AlertDialog.Builder alertdilaog = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.MyAlertDialogStyle);
                        alertdilaog.SetTitle("Failed Try Again");
                        alertdilaog.SetPositiveButton("OK", (s, a) =>
                        {
                        });
                        Dialog _dialog = alertdilaog.Create();
                        _dialog.Show();
                    });
                }
            })).Start();
        }

        public bool IsMatchIdPattern(string test)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"([0-9a-zA-Z]{5})");
        }
    }
}