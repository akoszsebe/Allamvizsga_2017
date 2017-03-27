using MikePhil.Charting.Charts;

namespace Allamvizsga2017.Chart
{
    class MyLineChartComplex
    {
        private MyLineChart linechart;
        private MyLineChart linechartalldata;

        public MyLineChartComplex(LineChart chartforpartdata, LineChart chartforalldata)
        {
            linechart = new MyLineChart(chartforpartdata);
            linechartalldata = new MyLineChart(chartforalldata);

            linechart.mChart.Click += delegate
            {
                linechartalldata.RemoveDataSet(3);
                linechartalldata.AddEntry(linechart.mChart.LowestVisibleX, 0, 3);
                linechartalldata.AddEntry(linechart.mChart.LowestVisibleX, linechartalldata.mChart.YMax, 3);
                linechartalldata.AddEntry(linechart.mChart.HighestVisibleX, linechartalldata.mChart.YMax, 3);
                linechartalldata.AddEntry(linechart.mChart.HighestVisibleX, 0, 3);
                linechartalldata.mChart.Invalidate();
            };
        }


        public void AddEntry(float x, float y, int datasetindex)
        {
            linechart.AddEntry(x, (y + 10), datasetindex); 
            linechartalldata.AddEntry(x, (y + 10), datasetindex);
            linechart.SetVisibleXRangeMaximum(10);
        }

        public void ClearValues()
        {
            linechart.ClearValues();
            linechartalldata.ClearValues();
        }
    }
}