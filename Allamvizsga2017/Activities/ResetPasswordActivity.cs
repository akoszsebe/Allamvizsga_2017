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
using Android.Content.PM;
using Android.Support.V7.App;
using Android.Views.InputMethods;
using System.Threading;
using Allamvizsga2017.Models;

namespace Allamvizsga2017.Activities
{
    [Activity(Label = "ResetPasswordActivity", WindowSoftInputMode = SoftInput.StateHidden, ScreenOrientation = ScreenOrientation.Portrait)]
    public class ResetPasswordActivity : AppCompatActivity
    {
        EditText tiresetcode;
        EditText tinewpassword;
        Button btnewpassword;
        string user_email;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            user_email = Intent.GetStringExtra("user_email");
            SetContentView(Resource.Layout.ResetPassword);

            tiresetcode = FindViewById<EditText>(Resource.Id.textInputResetCode);
            tinewpassword = FindViewById<EditText>(Resource.Id.textInputNewPassword);
            btnewpassword = FindViewById<Button>(Resource.Id.buttonNewPassword);

            tinewpassword.Enabled = false;
            btnewpassword.Enabled = false;
            var passwordtrnsform = tinewpassword.TransformationMethod;
            bool longclicked = false;

            tiresetcode.TextChanged += (e, s) =>
            {
                if (tiresetcode.Text.Length == 5)
                    if (IsMatchIdPattern(tiresetcode.Text))
                    {
                        new Thread(new ThreadStart(() =>
                        {
                            var verifyresetcode = RestClient.VerifyResetCode(user_email,tiresetcode.Text);
                            if (verifyresetcode)
                            {
                                RunOnUiThread(() =>
                                {
                                    tiresetcode.SetCompoundDrawablesWithIntrinsicBounds(null, null, Resources.GetDrawable(Resource.Drawable.True_24), null);
                                    tinewpassword.Enabled = true;
                                    btnewpassword.Enabled = true;
                                });
                            }
                            else
                            {
                                RunOnUiThread(() =>
                                {
                                    tiresetcode.SetCompoundDrawablesWithIntrinsicBounds(null, null, Resources.GetDrawable(Resource.Drawable.False_24), null);
                                    tinewpassword.Enabled = false;
                                    btnewpassword.Enabled = false;
                                });
                            }
                        })).Start();
                        
                    }
                else
                    {
                        tiresetcode.SetCompoundDrawablesWithIntrinsicBounds(null, null, Resources.GetDrawable(Resource.Drawable.False_24), null);
                        tinewpassword.Enabled = false;
                        btnewpassword.Enabled = false;
                    }
            };

            tiresetcode.FocusChange += (s, e) =>
            {
                if (e.HasFocus)
                    tiresetcode.SetCursorVisible(true);
                else
                    tiresetcode.SetCursorVisible(false);
            };

            tinewpassword.LongClick += (v, e) =>
            {
                if (longclicked)
                {
                    tinewpassword.TransformationMethod = passwordtrnsform;
                    tinewpassword.SetSelection(tinewpassword.Text.Length);
                    tinewpassword.SetCompoundDrawablesWithIntrinsicBounds(null, null, Resources.GetDrawable(Resource.Drawable.Lock_24), null);
                    longclicked = false;
                }
                else
                {
                    tinewpassword.TransformationMethod = null;
                    tinewpassword.SetSelection(tinewpassword.Text.Length);
                    tinewpassword.SetCompoundDrawablesWithIntrinsicBounds(null, null, Resources.GetDrawable(Resource.Drawable.Unlock_24), null);
                    longclicked = true;
                }

            };

            tinewpassword.FocusChange += (e, s) =>
            {
                if (!s.HasFocus)
                {
                    tinewpassword.TransformationMethod = passwordtrnsform;
                    tinewpassword.SetCompoundDrawablesWithIntrinsicBounds(null, null, Resources.GetDrawable(Resource.Drawable.Lock_24), null);
                    tinewpassword.SetCursorVisible(false);
                }
                else tinewpassword.SetCursorVisible(true); 
            
            };

            btnewpassword.Click += delegate
            {

                tinewpassword.TransformationMethod = passwordtrnsform;
                tinewpassword.SetCompoundDrawablesWithIntrinsicBounds(null, null, Resources.GetDrawable(Resource.Drawable.Lock_24), null);
                longclicked = false;
                if (tinewpassword.Text != "")
                {
                    ProgressDialog progress = new ProgressDialog(this);
                    progress.Indeterminate = true;
                    progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progress.SetMessage("Saving New Password...");
                    progress.SetCancelable(true);
                    progress.Show();
                    new Thread(new ThreadStart(() =>
                    {
                        var paswdchanged = RestClient.Register(new LoginUser(user_email, tinewpassword.Text));
                        if (paswdchanged)
                        {
                            RunOnUiThread(() =>
                            {
                                progress.Dismiss();
                                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.MyAlertDialogStyle);
                                alert.SetTitle("Succesful");
                                alert.SetPositiveButton("Login", (senderAlert, args) =>
                                {
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
                                alert.SetTitle("Error");
                                alert.SetMessage("Try again");
                                Dialog dialog = alert.Create();
                                dialog.Show();
                            });
                        }
                    })).Start();
                }
            };
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, 0);
            return base.OnTouchEvent(e);
        }

        public bool IsMatchIdPattern(string test)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"([0-9a-fA-F]{5})");
        }
    }
}