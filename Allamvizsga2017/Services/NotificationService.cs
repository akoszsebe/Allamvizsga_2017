using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;
using Allamvizsga2017.Models;
using System.Collections.Generic;

namespace Allamvizsga2017.Services
{
    [Service(Name = "com.xamarin.NotificationService")]
    class NotificationService : Service
    {
        static readonly string TAG = "X:" + typeof(NotificationService).Name;
        static readonly int TimerWait = 30000;
        Timer timer;
        DateTime startTime;
        bool isStarted = false;
        private Socket socket = IO.Socket("http://allamvizsga-akoszsebe.c9users.io");
        private string guid = NotificationStarter.guid;
        private bool emited = false;
        private string house_ids;

        public override void OnCreate()
        {

            base.OnCreate();
            socket.On("notification", data =>
            {

                var e = JsonConvert.DeserializeObject<List<Device>>(data.ToString());
                Android.App.Notification.Builder builder = new Android.App.Notification.Builder(this)
                    .SetContentTitle("Turned On")
                    .SetContentText(e[0].name + " " + e[0].value + " W")
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

        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            house_ids = intent.GetStringExtra("house_ids");
            Log.Debug(TAG, $"OnStartCommand called at {startTime}, flags={flags}, startid={startId}");
            socket.Connect().Emit("register", "{ \"guid\" : \"" + guid + "\" , \"house_ids\" : [" + house_ids + "] }");
            emited = true;
            if (isStarted)
            {
                TimeSpan runtime = DateTime.UtcNow.Subtract(startTime);
                Log.Debug(TAG, $"This service was already started, it's been running for {runtime:c}.");
            }
            else
            {
                startTime = DateTime.UtcNow;
                Log.Debug(TAG, $"Starting the service, at {startTime}.");
                timer = new Timer(HandleTimerCallback, startTime, 0, TimerWait);
                isStarted = true;
            }
            return StartCommandResult.StickyCompatibility;
        }

        public override IBinder OnBind(Intent intent)
        {
            // This is a started service, not a bound service, so we just return null.
            return null;
        }


        public override void OnDestroy()
        {
            timer.Dispose();
            timer = null;
            isStarted = false;

            TimeSpan runtime = DateTime.UtcNow.Subtract(startTime);
            Log.Debug(TAG, $"Simple Service destroyed at {DateTime.UtcNow} after running for {runtime:c}.");
            socket.Disconnect();
            base.OnDestroy();
        }

        void HandleTimerCallback(object state)
        {
            TimeSpan runTime = DateTime.UtcNow.Subtract(startTime);
            Log.Debug(TAG, $"This service has been running for {runTime:c} (since ${state}).");
            Log.Debug(TAG, $"Socket =-------------------------------- : " + (socket.Io().ReadyState.ToString()));
            if (socket.Io().ReadyState.ToString().Equals("CLOSED"))
            {
                emited = false;    
            }
            else if (socket.Io().ReadyState.ToString().Equals("OPEN"))
            {
                if (!emited)
                {
                    socket.Emit("register", "{ \"guid\" : \"" + guid + "\" , \"house_ids\" : [" + string.Join(",", house_ids) + "] }");
                }
                emited = true;
            }
        }
    }
}