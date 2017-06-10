
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
            var tvhaveaccount = FindViewById<TextView>(Resource.Id.textViewHaveAccount);

            var passwordtrnsform = tipasswd.TransformationMethod;
            bool longclicked = false;
            tipasswd.LongClick += (v, e) =>
            {
                if (longclicked)
                {
                    tipasswd.TransformationMethod = passwordtrnsform;
                    tipasswd.SetSelection(tipasswd.Text.Length);
                    tipasswd.SetCompoundDrawablesWithIntrinsicBounds(null, null, Resources.GetDrawable(Resource.Drawable.Lock_24), null);
                    longclicked = false;
                }
                else
                {
                    tipasswd.TransformationMethod = null;
                    tipasswd.SetSelection(tipasswd.Text.Length);
                    tipasswd.SetCompoundDrawablesWithIntrinsicBounds(null, null, Resources.GetDrawable(Resource.Drawable.Unlock_24), null);
                    longclicked = true;
                }

            };

            tipasswd.FocusChange += (e, s) =>
            {
                if (!s.HasFocus)
                {
                    tipasswd.TransformationMethod = passwordtrnsform;
                    tipasswd.SetCompoundDrawablesWithIntrinsicBounds(null, null, Resources.GetDrawable(Resource.Drawable.Lock_24), null);
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