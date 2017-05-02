
using Android.App;
using Android.OS;
using Android.Widget;
using Quobject.SocketIoClientDotNet.Client;

namespace Allamvizsga2017.Activities
{
    [Activity(Label = "Notification", MainLauncher = true)]
    public class Notification : Activity
    {
        public Socket socket = IO.Socket("http://allamvizsga-akoszsebe.c9users.io:8080");
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Notification);

            Button connect = FindViewById<Button>(Resource.Id.button1);
            TextView text = FindViewById<TextView>(Resource.Id.textView1);
            connect.Click += delegate
            {
                socket.Connect().Emit("register", "{ \"id\" : \"12\" , \"house_ids\" : [ \"0\", \"1\" ]}");
            };
            

            socket.On("notification", data =>
            {
                RunOnUiThread(()=> {
                    text.Text = data.ToString() + '\n';
                });
                
            });
        }
    }
}