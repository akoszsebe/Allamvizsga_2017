using System;
using Java.IO;
using Quobject.SocketIoClientDotNet.Client;
using Java.Interop;
using Android.Runtime;

namespace Allamvizsga2017.Models
{
    class PassSocket: Java.Lang.Object, ISerializable
    {
        public PassSocket()
        {
        }

        public PassSocket(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public Socket socket { get; set; }
    }
}