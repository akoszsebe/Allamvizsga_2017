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
using Allamvizsga2017.Services;

namespace Allamvizsga2017.Models
{
    class NotificationStarter
    {
        static Context context;
        public static string guid { get; } = System.Guid.NewGuid().ToString();
        static Intent notificationsevice;

        public static void SetContext(Context c)
        {
            context = c;
        }

        public static bool StartNotificationService()
        {
            ISharedPreferences sharedPref = context.GetSharedPreferences("house_ids", FileCreationMode.Private);
            string house_ids = sharedPref.GetString("house_ids", null);
            StopNotificationService();
            if (context != null && (house_ids != null && house_ids != ""))
            {
                notificationsevice = new Intent(context, typeof(NotificationService));
                notificationsevice.PutExtra("house_ids", house_ids);
                context.StartService(notificationsevice);
                return true;
            }
            else return false;
        }

        public static bool GetNotification_Enabled()
        {
            ISharedPreferences sharedPref = context.GetSharedPreferences("notification_enable", FileCreationMode.Private);
            bool r = sharedPref.GetBoolean("notification_enable", false);
            return r;
        }

        public static void SetNotification_Enabled(bool enabled)
        {
            ISharedPreferences sharedPref = context.GetSharedPreferences("notification_enable", FileCreationMode.Private);
            ISharedPreferencesEditor editor = sharedPref.Edit();
            editor.PutBoolean("notification_enable", enabled);
            editor.Commit();
        }

        public static void StopNotificationService()
        {
            var services = GetRunningServices();
            if (services.Contains("com.xamarin.NotificationService"))
            {
                context.StopService(notificationsevice);
            }
        }

        private static IEnumerable<string> GetRunningServices()
        {
            var manager = (ActivityManager)context.GetSystemService(Context.ActivityService);
            return manager.GetRunningServices(int.MaxValue).Select(
                service => service.Service.ClassName).ToList();
        }
    }
}