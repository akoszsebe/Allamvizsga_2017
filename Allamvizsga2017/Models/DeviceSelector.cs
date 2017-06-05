using Android.Graphics.Drawables;
using System.Collections.Generic;

namespace Allamvizsga2017.Models
{
    class Device
    {
        public string house_id { get; set; }
        public string name { get; set; }
        public int icon_id { get; set; }
        public int value { get; set; }
        public int valuedelay { get; set; } = 1;

        public int original_value { get; set; }

    }
}