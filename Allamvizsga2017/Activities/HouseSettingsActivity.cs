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
    [Activity(Label = "HouseSettingsActivity")]
    public class HouseSettingsActivity : AppCompatActivity
    {
        string house_name { get; set; }
        string user_email { get; set; }
        string house_id { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.HouseSettings);
            house_name = Intent.GetStringExtra("house_name");
            user_email = Intent.GetStringExtra("user_email");
            house_id = Intent.GetStringExtra("house_id");

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar1);
            toolbar.SetTitleTextAppearance(this, Resource.Style.ActionBarTitle);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetTitle(Resource.String.toolbar_housesettings);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);


            var tehouseid = FindViewById<TextView>(Resource.Id.textViewHouseId);
            var tihousename = FindViewById<EditText>(Resource.Id.textInputHouseName);
            var btswitchnotificationthishouse = FindViewById<Switch>(Resource.Id.switch1);
            var btswitchenable = FindViewById<Switch>(Resource.Id.switchEnable);

            tihousename.FocusChange += (s, e) =>
             {
                 if (e.HasFocus)
                 {
                     tihousename.SetCursorVisible(true);
                 }
                 else
                 {
                     tihousename.SetCursorVisible(false);
                 }
             };

            tehouseid.Text = house_id;
            tihousename.Text = house_name;
            tihousename.Hint = house_name;
            List<string> house_idarray = new List<string>();
            ISharedPreferences sharedPref = GetSharedPreferences("house_ids", FileCreationMode.Private);
            string house_ids = sharedPref.GetString("house_ids", null);
            if (house_ids != null && house_ids != "")
                house_idarray = house_ids.Split(',').ToList();

            if (!NotificationStarter.GetNotification_Enabled())
            {
                btswitchnotificationthishouse.Clickable = false;
                btswitchnotificationthishouse.Enabled = false;
            }
            else
                btswitchenable.Checked = true;

            if (house_idarray.Count == 0)
            {
                btswitchnotificationthishouse.Checked = false;
            }
            else
                if (house_idarray.ToList().Exists(x => x == ("\"" + house_id + "\"")))
            {
                btswitchnotificationthishouse.Checked = true;
            }
            

            btswitchnotificationthishouse.CheckedChange += (s, e) =>
            {
                if (e.IsChecked)
                {
                    house_idarray.Add("\"" + house_id + "\"");
                }
                else house_idarray.Remove("\"" + house_id + "\"");
                ISharedPreferencesEditor editor = sharedPref.Edit();
                editor.PutString("house_ids", string.Join(",",house_idarray));
                editor.Commit();
            };

            btswitchenable.CheckedChange += (s, e) =>
            {
                if (e.IsChecked)
                {
                    NotificationStarter.SetNotification_Enabled(true);
                    btswitchnotificationthishouse.Clickable = true;
                    btswitchnotificationthishouse.Enabled = true;
                }
                else
                {
                    NotificationStarter.SetNotification_Enabled(false);
                    btswitchnotificationthishouse.Clickable = false;
                    btswitchnotificationthishouse.Enabled = false;
                }
            };

        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_devicesetting, menu);
            return base.OnCreateOptionsMenu(menu);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                Finish();
            if (item.ItemId == Resource.Id.menu_save)
            {
                NotificationStarter.StartNotificationService();
                Finish();
            }
            if (item.ItemId == Resource.Id.menu_delete)
            {
                ProgressDialog progress = new ProgressDialog(this);
                progress.Indeterminate = true;
                progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                progress.SetMessage("Deleting House...");
                progress.SetCancelable(true);
                progress.Show();
                new Thread(new ThreadStart(() =>
                {
                    var successed = RestClient.DeleteUserHouse(user_email, house_id);
                    if (successed)
                    {
                        RunOnUiThread(() =>
                        {
                            progress.Dismiss();
                            Finish();
                        });
                    }
                    else
                    {
                        RunOnUiThread(() =>
                        {
                            progress.Dismiss();
                            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.MyAlertDialogStyle);
                            alert.SetTitle("Please Try Again");
                            alert.SetPositiveButton("OK", (senderAlert, args) =>
                            {
                                Finish();
                            });
                            Dialog dialog = alert.Create();
                            dialog.Show();
                        });
                    }
                })).Start();
            }
            return base.OnOptionsItemSelected(item);
        }

    }
}