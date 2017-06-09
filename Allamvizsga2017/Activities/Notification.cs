
using Allamvizsga2017.Models;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using System.Collections.Generic;

namespace Allamvizsga2017.Activities
{
    [Activity(Label = "Notification", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Notification : Activity
    {
        public Socket socket = IO.Socket("http://allamvizsga-akoszsebe.c9users.io");
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Notification);

            Button connect = FindViewById<Button>(Resource.Id.button1);
            TextView text = FindViewById<TextView>(Resource.Id.textView1);
            connect.Click += delegate
            {
                
            };
            

            socket.On("notification", data =>
            {
                RunOnUiThread(()=> {
                    var e = JsonConvert.DeserializeObject<List<Device>>(data.ToString());
                    Android.App.Notification.Builder builder = new Android.App.Notification.Builder(this)
                        .SetContentTitle("Turned On")
                        .SetContentText(e[0].name +" " + e[0].value + " W")
                        .SetDefaults(NotificationDefaults.Sound)
                        .SetSmallIcon(e[0].icon_id);

                    // Build the notification:
                    Android.App.Notification notification = builder.Build();

                    // Get the notification manager:
                    Android.App.NotificationManager notificationManager =
                        GetSystemService(Context.NotificationService) as NotificationManager;

                    // Publish the notification:
                    const int notificationId = 0;
                    notificationManager.Notify(notificationId, notification);
                });
                
            });



        }

        protected override void OnStart()
        {
            base.OnStart();
            socket.Connect().Emit("register", "{ \"id\" : \"12\" , \"house_ids\" : [ \"43ad-1234-5432\" ]}");
        }
    }
}