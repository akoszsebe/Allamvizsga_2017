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

        private TextView mTxtFirstName;
        private TextView mTxtLastName;
        private TextView mTxtName;
        private ProfilePictureView mProfilePic;
        private ShareButton mBtnShared;
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

            mTxtFirstName = FindViewById<TextView>(Resource.Id.txtFirstName);
            mTxtLastName = FindViewById<TextView>(Resource.Id.txtLastName);
            mTxtName = FindViewById<TextView>(Resource.Id.txtName);
            mProfilePic = FindViewById<ProfilePictureView>(Resource.Id.profilePic);
            mBtnShared = FindViewById<ShareButton>(Resource.Id.btnShare);
            mBtnGetEmail = FindViewById<Button>(Resource.Id.btnGetEmail);

            LoginButton button = FindViewById<LoginButton>(Resource.Id.login_button);

            button.SetReadPermissions(new List<string> { "public_profile", "user_friends", "email" });

            mCallBackManager = CallbackManagerFactory.Create();

            button.RegisterCallback(mCallBackManager, this);


            mBtnGetEmail.Click += (o, e) =>
            {
                GraphRequest request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);

                Bundle parameters = new Bundle();
                parameters.PutString("fields", "id,name,age_range,email");
                request.Parameters = parameters;
                request.ExecuteAsync();
            };

            LoginManager.Instance.RegisterCallback(mCallBackManager, this);

            ShareLinkContent content = new ShareLinkContent.Builder().Build();
            mBtnShared.ShareContent = content;
        }

        public void OnCompleted(Org.Json.JSONObject json, GraphResponse response)
        {
            string data = json.ToString();
            FacebookResult result = JsonConvert.DeserializeObject<FacebookResult>(data);
            mTxtName.Text = result.email;
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
                    mTxtFirstName.Text = e.mProfile.FirstName;
                    mTxtLastName.Text = e.mProfile.LastName;
                    mProfilePic.ProfileId = e.mProfile.Id;
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
                mTxtFirstName.Text = "First Name";
                mTxtLastName.Text = "Last Name";
                mTxtName.Text = "Name";
                mProfilePic.ProfileId = null;
            }
        }


        protected override void OnStart()
        {
            base.OnStart();
            mProfileTracker.StartTracking();
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