
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
using Android.Support.V4.Content;

namespace Allamvizsga2017.Activities
{
    [Activity(Label = "RegistrationActivity", WindowSoftInputMode = SoftInput.StateHidden, ScreenOrientation = ScreenOrientation.Portrait)]
    public class RegistrationActivity : AppCompatActivity
    {
        EditText tiemail;
        EditText tipasswd;
        Button btlogin;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Create your application here
            SetContentView(Resource.Layout.Registration);
            btlogin = FindViewById<Button>(Resource.Id.buttonLogin);
            tiemail = FindViewById<EditText>(Resource.Id.textInputEmail);
            tipasswd = FindViewById<EditText>(Resource.Id.textInputPassword);
            var tipasswdconfirm = FindViewById<EditText>(Resource.Id.textInputConfirmPassword);
            var tvhaveaccount = FindViewById<TextView>(Resource.Id.textViewHaveAccount);
            var btpasswdicon = FindViewById<Button>(Resource.Id.buttonpasswdicon);
            var btpasswd2icon = FindViewById<Button>(Resource.Id.buttonpasswd2icon);

            var passwordtrnsform = tipasswd.TransformationMethod;
            bool longclicked = false;
            btpasswdicon.Click += (v, e) =>
            {
                if (longclicked)
                {
                    tipasswd.TransformationMethod = passwordtrnsform;
                    tipasswd.SetSelection(tipasswd.Text.Length);
                    btpasswdicon.SetBackgroundResource(Resource.Drawable.Lock_24);
                    longclicked = false;
                }
                else
                {
                    tipasswd.TransformationMethod = null;
                    tipasswd.SetSelection(tipasswd.Text.Length);
                    btpasswdicon.SetBackgroundResource(Resource.Drawable.Unlock_24);
                    longclicked = true;
                }

            };

            var passwordtrnsform2 = tipasswdconfirm.TransformationMethod;
            bool longclicked2 = false;
            btpasswd2icon.Click += (v, e) =>
            {
                if (longclicked2)
                {
                    tipasswdconfirm.TransformationMethod = passwordtrnsform2;
                    tipasswdconfirm.SetSelection(tipasswdconfirm.Text.Length);
                    btpasswd2icon.SetBackgroundResource(Resource.Drawable.Lock_24);
                    longclicked2 = false;
                }
                else
                {
                    tipasswdconfirm.TransformationMethod = null;
                    tipasswdconfirm.SetSelection(tipasswdconfirm.Text.Length);
                    btpasswd2icon.SetBackgroundResource(Resource.Drawable.Unlock_24);
                    longclicked2 = true;
                }

            };

            tipasswdconfirm.FocusChange += (e, s) =>
            {
                if (!s.HasFocus)
                {
                    tipasswdconfirm.TransformationMethod = passwordtrnsform2;
                    btpasswd2icon.SetBackgroundResource(Resource.Drawable.Lock_24);
                    longclicked2 = false;
                    tipasswdconfirm.SetCursorVisible(false);
                }
                else tipasswdconfirm.SetCursorVisible(true);
            };

            tipasswd.FocusChange += (e, s) =>
            {
                if (!s.HasFocus)
                {
                    tipasswd.TransformationMethod = passwordtrnsform;
                    btpasswdicon.SetBackgroundResource(Resource.Drawable.Lock_24);
                    longclicked = false;
                    tipasswd.SetCursorVisible(false);
                }
                else tipasswd.SetCursorVisible(true);
            };

            tiemail.FocusChange += (s, e) =>
            {
                if (e.HasFocus)
                    tiemail.SetCursorVisible(true);
                else
                    tiemail.SetCursorVisible(false);
            };

            btlogin.Click += delegate
            {
                ProgressDialog progress = new ProgressDialog(this);
                progress.Indeterminate = true;
                progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                progress.SetMessage("Registring...");
                progress.SetCancelable(true);
                progress.Show();
                new Thread(new ThreadStart(() =>
                {
                    var canlogin = RestClient.Register(new LoginUser(tiemail.Text, tipasswd.Text));
                    if (canlogin)
                    {
                        RunOnUiThread(() =>
                        {
                            progress.Dismiss();
                            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.MyAlertDialogStyle);
                            alert.SetTitle("Succesful");
                            alert.SetPositiveButton("Login", (senderAlert, args) =>
                            {
                                var loginactivity = new Intent(this, typeof(LoginActivity));
                                this.StartActivity(loginactivity);
                                this.Finish();
                            });
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
                            alert.SetTitle("Can`t Registerd");
                            alert.SetMessage("Try again");
                            Dialog dialog = alert.Create();
                            dialog.Show();
                        });
                    }
                })).Start();
            };

            tvhaveaccount.Click += delegate
            {
                this.Finish();
            };
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, 0);
            return base.OnTouchEvent(e);
        }
    }
}