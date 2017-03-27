using Android.Graphics.Drawables;
using System.Collections.Generic;

namespace Allamvizsga2017.Models
{
    class DeviceSelector
    {

        List<Device> devices;

        public DeviceSelector(List<Device> d)
        {
            devices = d;
        }


        public Device GetDevice(int value)
        {
            if (devices != null)
                foreach (var device in devices)
                {
                    if (device.value - device.valuedelay <= value && device.value + device.valuedelay >= value)
                    {
                        return new Device() { name = device.name, icon_id = device.icon_id, value = value, valuedelay = device.valuedelay };
                    }
                }
            return new Device() { name = "Unknown", icon_id = Resource.Drawable.unknownicon, value = value, valuedelay = 1 };
        }

        public List<Device> GetAllDevices()
        {
            return devices;
        }
    }
    class Device
    {
        public int house_id { get; set; }
        public string name { get; set; }
        public int icon_id { get; set; }
        public int value { get; set; }
        public int valuedelay { get; set; } = 1;

    }
}