
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
    [Activity(Label = "Notification")]
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
                socket.Connect().Emit("register", "{ \"id\" : \"12\" , \"house_ids\" : [ \"1\" ]}");
            };

            int notificationId = 0;
            socket.On("notification", data =>
            {
                var d = JsonConvert.DeserializeObject<List<Device>>(data.ToString());
                RunOnUiThread(()=> {
                    //text.Text = data.ToString() + '\n';
                    // Instantiate the builder and set notification elements:
                    Android.App.Notification.Builder builder = new Android.App.Notification.Builder(this)
                        .SetContentTitle("Turned On")
                        .SetContentText(d[0].name + " " + d[0].value + " W")
                        .SetSmallIcon(d[0].icon_id)
                        .SetDefaults(NotificationDefaults.Sound);

                    // Build the notification:
                    Android.App.Notification notification = builder.Build();

                    // Get the notification manager:
                    NotificationManager notificationManager =
                        GetSystemService(Context.NotificationService) as NotificationManager;

                    // Publish the notification:
                    
                    notificationManager.Notify(notificationId, notification);
                    notificationId++;
                    if (notificationId == 10) notificationId = 0;
                });
                
            });
        }
    }
}