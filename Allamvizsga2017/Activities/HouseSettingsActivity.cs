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
using SatelliteMenu;

namespace Allamvizsga2017.Activities
{
    [Activity(Label = "HouseSettingsActivity")]
    public class HouseSettingsActivity : AppCompatActivity
    {
        string house_name { get; set; }
        string user_email { get; set; }
        long house_id { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.HouseSettings);
            house_name = Intent.GetStringExtra("house_name");
            user_email = Intent.GetStringExtra("user_email");
            house_id = Intent.GetLongExtra("house_id", 0);
            var tbhousename = FindViewById<TextView>(Resource.Id.textViewHouseName);

            tbhousename.Text = house_name + " " + user_email + " " + house_id;

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar1);


            toolbar.SetTitleTextAppearance(this, Resource.Style.ActionBarTitle);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "House Settings";
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
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