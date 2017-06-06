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
using System.Threading;
using Allamvizsga2017.Models;
using Android.Views.InputMethods;

namespace Allamvizsga2017.Activities
{
    [Activity(Label = "HouseRegistrationActivity", WindowSoftInputMode = SoftInput.StateHidden,
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class HouseRegistrationActivity : AppCompatActivity
    {
        EditText tihouse_id;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.HouseRegistration);
            var user_email = Intent.GetStringExtra("User_email");
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar1);

            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "House Registration";

            toolbar.SetTitleTextAppearance(this, Resource.Style.ActionBarTitle);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            tihouse_id = FindViewById<EditText>(Resource.Id.textInputHouseId);
            var tihouse_name = FindViewById<EditText>(Resource.Id.textInputHouseName);
            var tihouse_password = FindViewById<EditText>(Resource.Id.textInputPassword);
            var btregistration = FindViewById<Button>(Resource.Id.buttonHouseRegister);

            tihouse_id.TextChanged += (e, s) =>
            {
                if (tihouse_id.Text.Length == 4 || tihouse_id.Text.Length == 9)
                {
                    tihouse_id.Append("-");
                }
            };

            btregistration.Click += delegate
            {
                if (tihouse_id.Text.ToString().Trim().Equals(""))
                {
                    tihouse_id.Error = "House id is required!";
                }
                else
                if (tihouse_id.Text.Length != 16)
                {
                    tihouse_id.Error = "Id format: xxxx-xxxx-yyxxxx ";
                }
                else
                if (IsMatchIdPattern(tihouse_id.Text))
                {

                    ProgressDialog progress = new ProgressDialog(this);
                    progress.Indeterminate = true;
                    progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progress.SetMessage("Registring...");
                    progress.SetCancelable(true);
                    progress.Show();
                    new Thread(new ThreadStart(() =>
                    {
                        var canlogin = RestClient.RegisterHouse(tihouse_id.Text,tihouse_name.Text);
                        if (canlogin)
                        {
                            var adduserhouse = RestClient.AddUserHouse(user_email, tihouse_id.Text);
                            if (adduserhouse)
                            {
                                RunOnUiThread(() =>
                                {
                                    progress.Dismiss();
                                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                                    alert.SetTitle("Successful");
                                    Dialog dialog = alert.Create();
                                    dialog.Show();
                                });
                            }
                            else
                            {
                                RunOnUiThread(() =>
                                {
                                    progress.Dismiss();
                                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                                    alert.SetTitle("Error");
                                    alert.SetMessage("Registerd but don`t added to yout house list");
                                    Dialog dialog = alert.Create();
                                    dialog.Show();
                                });
                            }
                        }
                        else
                        {
                            RunOnUiThread(() =>
                            {
                                progress.Dismiss();
                                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                                alert.SetTitle("Can`t Registerd");
                                alert.SetMessage("Try again");
                                Dialog dialog = alert.Create();
                                dialog.Show();
                            });
                        }
                    })).Start();
                }
                else
                {
                    tihouse_id.Error = "Id need to be hex number";
                }
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
            imm.HideSoftInputFromWindow(tihouse_id.WindowToken, 0);
            return base.OnTouchEvent(e);
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

        public bool IsMatchIdPattern(string test)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"([0-9a-zA-Z]{4}-[0-9a-zA-Z]{4}-[0-9a-zA-Z]{6})");
        }
    }
}