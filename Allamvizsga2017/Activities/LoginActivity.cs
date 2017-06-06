using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using System.Threading;
using Android.Views.InputMethods;
using Android.Content.PM;
using Allamvizsga2017.Models;
using System;

namespace Allamvizsga2017.Activities
{
    [Activity(Label = "Allamvizsga", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoginActivity : AppCompatActivity
    {
        EditText tiemail;
        EditText tipasswd;
        Button btlogin;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Create your application here
            SetContentView(Resource.Layout.Login);
            btlogin = FindViewById<Button>(Resource.Id.buttonLogin);
            tiemail = FindViewById<EditText>(Resource.Id.textInputEmail);
            tipasswd = FindViewById<EditText>(Resource.Id.textInputPassword);
            var tvnoaccount = FindViewById<TextView>(Resource.Id.textViewNoAccount);

            btlogin.Click += delegate
            {
                Login();
            };


            var passwordtrnsform = tipasswd.TransformationMethod;
            bool longclicked = false;
            tipasswd.LongClick += (v,e) =>
            {
                if (longclicked)
                {
                    tipasswd.TransformationMethod = passwordtrnsform;
                    tipasswd.SetSelection(tipasswd.Text.Length);
                    longclicked = false;
                }
                else
                {
                    tipasswd.TransformationMethod = null;
                    tipasswd.SetSelection(tipasswd.Text.Length);
                    longclicked = true;
                }
                
            };

            tipasswd.FocusChange += (e, s) =>
            {
                if (!s.HasFocus)
                {
                    tipasswd.TransformationMethod = passwordtrnsform;
                }
            };

            tipasswd.KeyPress += (v, e) =>
            {
                if (e.Event.Action == KeyEventActions.Down &&  e.Event.KeyCode == Keycode.Enter)
                {
                    Login();
                    e.Handled = true;
                }
                else
                    e.Handled = false;
                  
            };

            tvnoaccount.Click += delegate
            {
                var registeractivity = new Intent(this, typeof(RegistrationActivity));
                this.StartActivity(registeractivity);
            };
        }

        protected override void OnStart()
        {
            ISharedPreferences sharedPref = GetSharedPreferences("user_email", FileCreationMode.Private);
            string user_email = sharedPref.GetString("user_email", null);
            if (user_email != null)
            {
                var housesactivity = new Intent(this, typeof(HousesActivity));
                housesactivity.PutExtra("User_email", user_email);
                this.StartActivity(housesactivity);
                this.Finish();
            }
            base.OnStart();
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, 0);
            return base.OnTouchEvent(e);
        }

        private void Login()
        {
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Authenticating...");
            progress.SetCancelable(true);
            progress.Show();
            new Thread(new ThreadStart(() =>
            {
                var canlogin = RestClient.Login(new LoginUser(tiemail.Text, tipasswd.Text));
                if (canlogin)
                {
                    RunOnUiThread(() =>
                    {
                        progress.Dismiss();
                        ISharedPreferences sharedPref = GetSharedPreferences("user_email", FileCreationMode.Private);
                        ISharedPreferencesEditor editor = sharedPref.Edit();
                        editor.PutString("user_email", tiemail.Text);
                        editor.Commit();
                        var housesactivity = new Intent(this, typeof(HousesActivity));
                        housesactivity.PutExtra("User_email", tiemail.Text);
                        this.StartActivity(housesactivity);
                        this.Finish();
                    });
                }
                else
                {
                    RunOnUiThread(() =>
                    {
                        progress.Dismiss();
                        Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.MyAlertDialogStyle);
                        alert.SetTitle("Not Registerd");
                        alert.SetMessage("Please Register");
                        alert.SetPositiveButton("Register", (senderAlert, args) =>
                        {
                            var registeractivity = new Intent(this, typeof(RegistrationActivity));
                            this.StartActivity(registeractivity);
                        });

                        alert.SetNeutralButton("Cancel", (senderAlert, args) =>
                        {
                        });
                        Dialog dialog = alert.Create();
                        dialog.Show();
                    });
                }
            })).Start();
        }
    }
}