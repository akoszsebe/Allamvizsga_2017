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
            SupportActionBar.SetTitle(Resource.String.toolbar_houseregistartion);

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


            var passwordtransform = tihouse_password.TransformationMethod;
            bool longclicked = false;
            tihouse_password.LongClick += (v, e) =>
            {
                if (longclicked)
                {
                    tihouse_password.TransformationMethod = passwordtransform;
                    tihouse_password.SetSelection(tihouse_password.Text.Length);
                    tihouse_password.SetCompoundDrawablesWithIntrinsicBounds(null, null, Resources.GetDrawable(Resource.Drawable.Lock_24), null);
                    longclicked = false;
                }
                else
                {
                    tihouse_password.TransformationMethod = null;
                    tihouse_password.SetSelection(tihouse_password.Text.Length);
                    tihouse_password.SetCompoundDrawablesWithIntrinsicBounds(null, null, Resources.GetDrawable(Resource.Drawable.Unlock_24), null);
                    longclicked = true;
                }

            };

            tihouse_password.FocusChange += (e, s) =>
            {
                if (!s.HasFocus)
                {
                    tihouse_password.TransformationMethod = passwordtransform;
                    tihouse_password.SetCompoundDrawablesWithIntrinsicBounds(null, null, Resources.GetDrawable(Resource.Drawable.Lock_24), null);
                }
            };


            btregistration.Click += delegate
            {
                if (tihouse_password.Text.ToString().Trim().Equals(""))
                {
                    tihouse_password.Error = "Password is required";
                }
                else 
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
                                    Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.MyAlertDialogStyle);
                                    alert.SetTitle("Successful");
                                    alert.SetCancelable(true);
                                    Dialog dialog = alert.Create();
                                    dialog.Show();
                                });
                            }
                            else
                            {
                                RunOnUiThread(() =>
                                {
                                    progress.Dismiss();
                                    Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.MyAlertDialogStyle);
                                    alert.SetTitle("Error");
                                    alert.SetMessage("Registerd but don`t added to yout house list");
                                    alert.SetCancelable(true);
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
                                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.MyAlertDialogStyle);
                                alert.SetTitle("Can`t Registerd");
                                alert.SetMessage("Try again");
                                alert.SetCancelable(true);
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
            {
                Finish();
            }
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
                Finish();
            }
            return base.OnOptionsItemSelected(item);
        }

        public bool IsMatchIdPattern(string test)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"([0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{6})");
        }
    }
}