using System;
using MikePhil.Charting.Formatter;
using Java.Text;
using MikePhil.Charting.Components;
using Java.Util.Concurrent;
using Java.Sql;

namespace Allamvizsga2017.Models
{
    class AxisValueFormatter : IAxisValueFormatter
    {
        private SimpleDateFormat mFormat = new SimpleDateFormat("dd MMM HH:mm");

        public IntPtr Handle
        {
            get
            {
                return IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            
        }

        public string GetFormattedValue(float value, AxisBase axis)
        {
            long millis = TimeUnit.Hours.ToMillis((long)value);
            return mFormat.Format(new Date(millis));
        }

    }
}