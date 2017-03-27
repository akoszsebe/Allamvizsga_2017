
using Android.Content;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Data;
using MikePhil.Charting.Util;

namespace Allamvizsga2017.Chart
{

    class MyPieChart
    {
        private PieChart chart;
        private PieDataSet dataset = new PieDataSet(new System.Collections.Generic.List<PieEntry>(), "# of Calls");

        public MyPieChart(Context context)
        {
            chart = new PieChart(context);

            chart.RotationEnabled = true;
            chart.SetBackgroundColor(Android.Graphics.Color.WhiteSmoke);
            chart.CenterText = "Heti atlag";
            chart.SetHoleColor(Android.Graphics.Color.WhiteSmoke);
            chart.Description = null;

            dataset.SetColors(ColorTemplate.HoloBlue, ColorTemplate.Rgb("#123120"), ColorTemplate.Rgb("#223120"), ColorTemplate.Rgb("#129120"), ColorTemplate.Rgb("#5A8622"));
            PieData data = new PieData(dataset);
            chart.Data = data;
        }

        private PieEntry AddMokkData(int i)
        {
            System.Random r = new System.Random();
            return new PieEntry(r.Next(0, 10), "Tv "+i);
        }

        public void AddData(int i)
        {
            dataset.AddEntry(AddMokkData(i));
        }

        public PieChart GetPieChart()
        {

            return chart;
        }

    }
}