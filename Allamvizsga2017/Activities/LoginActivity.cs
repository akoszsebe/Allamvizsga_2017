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
            var tvforgetpassword = FindViewById<TextView>(Resource.Id.textViewForgetPassword);
            var tvemail = FindViewById<TextView>(Resource.Id.textViewEmail);
            var tvpassword = FindViewById<TextView>(Resource.Id.textViewPassword);
            var btusericon = FindViewById<Button>(Resource.Id.buttonusericon);

            var passwordtrnsform = tipasswd.TransformationMethod;
            bool longclicked = false;

            btlogin.Click += delegate
            {
                tipasswd.TransformationMethod = passwordtrnsform;
                btusericon.SetBackgroundResource(Resource.Drawable.Lock_24);
                Login();
            };

            tiemail.FocusChange += (s, e) =>
            {
                if (e.HasFocus && tiemail.Text == "")
                {
                    tvemail.Visibility = ViewStates.Visible;
                    tiemail.Hint = "";
                }
                else
                {
                    tvemail.Visibility = ViewStates.Invisible;
                    tiemail.SetHint(Resource.String.input_email);
                }
            };


            btusericon.Click += (v, e) =>
            {
                if (longclicked)
                {
                    tipasswd.TransformationMethod = passwordtrnsform;
                    tipasswd.SetSelection(tipasswd.Text.Length);
                    btusericon.SetBackgroundResource(Resource.Drawable.Lock_24);
                    longclicked = false;
                }
                else
                {
                    tipasswd.TransformationMethod = null;
                    tipasswd.RequestFocus();
                    tipasswd.SetSelection(tipasswd.Text.Length);
                    btusericon.SetBackgroundResource(Resource.Drawable.Unlock_24);
                    longclicked = true;

                }

            };

            tipasswd.FocusChange += (e, s) =>
            {
                if (!s.HasFocus)
                {
                    tipasswd.TransformationMethod = passwordtrnsform;
                    btusericon.SetBackgroundResource(Resource.Drawable.Lock_24);
                    longclicked = false;
                }
                if (s.HasFocus && tipasswd.Text == "")
                {
                    tvpassword.Visibility = ViewStates.Visible;
                    tipasswd.Hint = "";
                }
                else
                {
                    tvpassword.Visibility = ViewStates.Invisible;
                    tipasswd.SetHint(Resource.String.input_password);
                }
            };

            tipasswd.KeyPress += (v, e) =>
            {
                if (e.Event.Action == KeyEventActions.Down && e.Event.KeyCode == Keycode.Enter)
                {
                    tipasswd.TransformationMethod = passwordtrnsform;
                    btusericon.SetBackgroundResource(Resource.Drawable.Lock_24);
                    longclicked = false;
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

            tvforgetpassword.Click += delegate
            {
                var forgetpassword = new Intent(this, typeof(ForgetPasswordActivity));
                this.StartActivity(forgetpassword);
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
            View current = this.CurrentFocus;
            if (current != null) current.ClearFocus();
            return base.OnTouchEvent(e);
        }

        private void Login()
        {
            if (tiemail.Text.Equals(string.Empty))
            {
                tiemail.Error = "Email required";
                return;
            }
            else
            {
                if (!IsValidEmail(tiemail.Text))
                {
                    tiemail.Error = "invalid email";
                    return;
                }
            }
            if (tipasswd.Text.Equals(string.Empty))
            {
                tipasswd.Error = "Password required";
                tipasswd.RequestFocus();
                return;
            }
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Authenticating...");
            progress.SetCancelable(true);
            progress.Show();
            new Thread(new ThreadStart(() =>
            {
                var canlogin = RestClient.Login(new LoginUser(tiemail.Text, tipasswd.Text));
                if (canlogin == "true")
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
                        alert.SetTitle("Failed");
                        alert.SetMessage(canlogin);
                        if (canlogin == "false")
                        {
                            alert.SetMessage("Please Register");
                            alert.SetPositiveButton("Register", (senderAlert, args) =>
                            {
                                var registeractivity = new Intent(this, typeof(RegistrationActivity));
                                this.StartActivity(registeractivity);
                            });
                            alert.SetNeutralButton("Cancel", (senderAlert, args) =>
                            {
                            });
                        }
                        else
                        {
                            alert.SetPositiveButton("Ok", (senderAlert, args) =>
                            {
                            });
                        }
                        
                        Dialog dialog = alert.Create();
                        dialog.Show();
                    });
                }
            })).Start();
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}