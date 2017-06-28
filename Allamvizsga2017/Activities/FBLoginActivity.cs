using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Xamarin.Facebook;
using Xamarin.Facebook.Login.Widget;
using Xamarin.Facebook.Login;
using System.Collections.Generic;
using Xamarin.Facebook.Share.Model;
using Xamarin.Facebook.Share.Widget;
using Newtonsoft.Json;
using System.Net;

namespace Allamvizsga2017.Activities
{
    [Activity(Label = "Allamvizsga", MainLauncher = true, Icon = "@drawable/icon")]
    public class FBLoginActivity : Activity, IFacebookCallback, GraphRequest.IGraphJSONObjectCallback
    {
        private ICallbackManager mCallBackManager;
        private MyProfileTracker mProfileTracker;

        private TextView mTxtName;
        private Button mBtnGetEmail;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            FacebookSdk.SdkInitialize(this.ApplicationContext);

            mProfileTracker = new MyProfileTracker();
            mProfileTracker.mOnProfileChanged += mProfileTracker_mOnProfileChanged;
            mProfileTracker.StartTracking();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.FbLoginlayout);

            mTxtName = FindViewById<TextView>(Resource.Id.txtName);
            mBtnGetEmail = FindViewById<Button>(Resource.Id.btnGetEmail);

            LoginButton button = FindViewById<LoginButton>(Resource.Id.login_button);

            button.SetReadPermissions(new List<string> { "public_profile", "user_friends", "email" });

            mCallBackManager = CallbackManagerFactory.Create();

            button.RegisterCallback(mCallBackManager, this);


            mBtnGetEmail.Click += (o, e) =>
            {
                GraphRequest request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);

                Bundle parameters = new Bundle();
                parameters.PutString("fields", "id,name,email");
                request.Parameters = parameters;
                request.ExecuteAsync();
            };

            LoginManager.Instance.RegisterCallback(mCallBackManager, this);

        }

        public void OnCompleted(Org.Json.JSONObject json, GraphResponse response)
        {
            string data = json.ToString();
            FacebookResult result = JsonConvert.DeserializeObject<FacebookResult>(data);
            mTxtName.Text = result.email;
            if (result.email != null)
            {
                var housesactivity = new Intent(this, typeof(HousesActivity));
                housesactivity.PutExtra("User_email", result.email);
                this.StartActivity(housesactivity);
                this.Finish();
            }
            else
            {
                mTxtName.Text = "this user cant login";
            }
        }

        void client_UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void mProfileTracker_mOnProfileChanged(object sender, OnProfileChangedEventArgs e)
        {
            if (e.mProfile != null)
            {
                try
                {
                    mBtnGetEmail.CallOnClick();
                }

                catch (Java.Lang.Exception ex)
                {
                    //Handle error
                }
            }

            else
            {
                //the user must have logged out       
                mTxtName.Text = "Name";
            }
        }


        protected override void OnStart()
        {
            base.OnStart();
            mProfileTracker.StartTracking();
            mBtnGetEmail.CallOnClick();

        }

        public void OnCancel()
        {
            //throw new NotImplementedException();
        }

        public void OnError(FacebookException error)
        {
            //throw new NotImplementedException();
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            LoginResult loginResult = result as LoginResult;
            Console.WriteLine(AccessToken.CurrentAccessToken.UserId);

        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            mCallBackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        protected override void OnDestroy()
        {
            mProfileTracker.StopTracking();
            base.OnDestroy();
        }
    }

    public class MyProfileTracker : ProfileTracker
    {
        public event EventHandler<OnProfileChangedEventArgs> mOnProfileChanged;

        protected override void OnCurrentProfileChanged(Profile oldProfile, Profile newProfile)
        {
            if (mOnProfileChanged != null)
            {
                mOnProfileChanged.Invoke(this, new OnProfileChangedEventArgs(newProfile));
            }
        }
    }

    public class OnProfileChangedEventArgs : EventArgs
    {
        public Profile mProfile;

        public OnProfileChangedEventArgs(Profile profile) { mProfile = profile; }
    }

    internal class FacebookResult
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }
}