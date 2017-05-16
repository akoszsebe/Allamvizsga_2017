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
using Java.IO;

namespace Allamvizsga2017.Services
{
    [Service(Name= "com.xamarin.NotificationService")]
    class NotificationService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return new NotificationpBinder(this); ;
        }

        public override void OnCreate()
        {
            Android.Util.Log.Debug("nagy","mak -----------------");
        }

        public override bool OnUnbind(Intent intent)
        {
            // This method is optional
            Android.Util.Log.Debug("nagy", "OnUnbind");
            return base.OnUnbind(intent);
        }

        public override void OnDestroy()
        {
            // This method is optional
            Android.Util.Log.Debug("nagy", "OnDestroy");
            base.OnDestroy();
        }

        private class NotificationpBinder : IBinder
        {
            private NotificationService notificationService;

            public NotificationpBinder(NotificationService notificationService)
            {
                this.notificationService = notificationService;
            }

            public IntPtr Handle
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string InterfaceDescriptor
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsBinderAlive
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public void Dump(FileDescriptor fd, string[] args)
            {
                throw new NotImplementedException();
            }

            public void DumpAsync(FileDescriptor fd, string[] args)
            {
                throw new NotImplementedException();
            }

            public void LinkToDeath(IBinderDeathRecipient recipient, int flags)
            {
                throw new NotImplementedException();
            }

            public bool PingBinder()
            {
                throw new NotImplementedException();
            }

            public IInterface QueryLocalInterface(string descriptor)
            {
                throw new NotImplementedException();
            }

            public bool Transact(int code, Parcel data, Parcel reply, [GeneratedEnum] TransactionFlags flags)
            {
                throw new NotImplementedException();
            }

            public bool UnlinkToDeath(IBinderDeathRecipient recipient, int flags)
            {
                throw new NotImplementedException();
            }
        }
    }
}