using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using Android.Support.V7.App;
using System.Threading;
using Allamvizsga2017.Models;
using Android.Views.InputMethods;

namespace Allamvizsga2017.Activities
{
    [Activity(Label = "ResetPasswordActivity", WindowSoftInputMode = SoftInput.StateHidden, ScreenOrientation = ScreenOrientation.Portrait)]
    public class ForgetPasswordActivity : AppCompatActivity
    {
        EditText tiemail;
        Button sendrequest;
        bool inmail = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ForgetPassword);


            tiemail = FindViewById<EditText>(Resource.Id.textInputEmail);
            sendrequest = FindViewById<Button>(Resource.Id.buttonSend);

            var cbinmail = FindViewById<RadioButton>(Resource.Id.checkBox2);
            var cbinsms = FindViewById<RadioButton>(Resource.Id.checkBox1);

            cbinmail.Checked = true;

            cbinmail.CheckedChange += (s, e) =>
            {
                if (e.IsChecked)
                {
                    inmail = true;
                //    cbinmail.Checked = false;
                //    cbinsms.Checked = true;
                }
                else
                {
                    inmail = false;
                //    cbinmail.Checked = true;
                //    cbinsms.Checked = false;
                }
            };

            sendrequest.Click += delegate
            {
                RequestReset();
            };
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, 0);
            return base.OnTouchEvent(e);
        }


        private void RequestReset()
        {
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Sending Mail...");
            progress.SetCancelable(true);
            progress.Show();
            new Thread(new ThreadStart(() =>
            {
                if (IsValidEmail(tiemail.Text))
                {
                    var resetsend = RestClient.RequestResetCode(tiemail.Text, inmail);
                    if (resetsend)
                    {
                        RunOnUiThread(() =>
                        {
                            progress.Dismiss();
                            var resetpasswordactivity = new Intent(this, typeof(ResetPasswordActivity));
                            resetpasswordactivity.PutExtra("user_email", tiemail.Text);
                            this.StartActivity(resetpasswordactivity);
                            this.Finish();
                        });
                    }
                    else
                    {
                        RunOnUiThread(() =>
                        {
                            progress.Dismiss();
                            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.MyAlertDialogStyle);
                            alert.SetTitle("Error");
                            alert.SetMessage("Check your internet connection!");
                            alert.SetPositiveButton("Rety", (senderAlert, args) =>
                            {
                                RequestReset();
                            });

                            alert.SetNeutralButton("Cancel", (senderAlert, args) =>
                            {
                            });
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
                        tiemail.Error = "Email Required";
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