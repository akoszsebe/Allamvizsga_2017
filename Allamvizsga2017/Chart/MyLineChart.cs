using System;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Util;
using MikePhil.Charting.Data;
using Android.Graphics;
using MikePhil.Charting.Components;
using static MikePhil.Charting.Components.YAxis;
using Allamvizsga2017.Models;

namespace Allamvizsga2017.Chart
{


    class MyLineChart
    {
       
        public string[] pointColor { get; set; } = { "#2bd85f", "#23d85f", "2b185f", "#0e0f0f" };
        public string[] fillColor { get; set; } = { "#32f26c", "#23d85f", "2b185f" , "#e1ede4" };

        public LineChart mChart;

        public MyLineChart(LineChart linechart)
        {
            mChart = linechart;
            ChartSettings();
        }

        private void ChartSettings()
        {
            mChart.SetTouchEnabled(true);
            // enable scaling and dragging
            mChart.DragEnabled = true;
            mChart.SetScaleEnabled(false);
            mChart.SetDrawGridBackground(false);
            // if disabled, scaling can be done on x- and y-axis separately
            mChart.SetPinchZoom(true);
            // set an alternative background color
            mChart.SetBackgroundColor(Color.Gray);
            LineData data = new LineData();
            data.SetValueTextColor(Color.White);

            // add empty data
            mChart.Data = data;
            mChart.HighlightPerDragEnabled = false;
            mChart.HighlightPerTapEnabled = false;

            // get the legend (only possible after setting data)
            Legend l = mChart.Legend;

            // modify the legend ...
            l.Form = Legend.LegendForm.Line;
            l.TextColor = Color.White;

            XAxis xl = mChart.XAxis;
            xl.TextColor = Color.Black;
            xl.SetDrawGridLines(false);
            xl.SetAvoidFirstLastClipping(true);
            xl.Enabled = true;
            xl.ValueFormatter = new AxisValueFormatter();

            YAxis leftAxis = mChart.AxisRight;
            leftAxis.TextColor = Color.White;
            leftAxis.SetAxisMaxValue(100f);
            leftAxis.SetAxisMinValue(0f);
            leftAxis.SetDrawGridLines(true);

            YAxis rightAxis = mChart.AxisRight;
            rightAxis.Enabled = false;
        }


        public void AddEntry(float xtime,float yvalue,int datasetindex)
        {
            LineData data = (MikePhil.Charting.Data.LineData)mChart.Data;

            if (data != null)
            {
                LineDataSet set = (LineDataSet)data.GetDataSetByIndex(datasetindex);
                if (set == null)
                {
                    set = CreateSet(datasetindex, "TV " + datasetindex);
                    data.AddDataSet(set);
                }

                data.AddEntry(new Entry(xtime, yvalue), datasetindex);
                data.NotifyDataChanged();
                mChart.NotifyDataSetChanged();
                mChart.MoveViewTo(data.EntryCount - 7, 55f, AxisDependency.Left);
            }
        }

        public void RemoveDataSet(int datasetindex)
        {
            LineData data = (MikePhil.Charting.Data.LineData)mChart.Data;

            if (data != null)
            {
                LineDataSet set = (LineDataSet)data.GetDataSetByIndex(datasetindex);
                if (set == null)
                {
                    set = CreateSet(datasetindex,"TV "+ datasetindex );
                    data.AddDataSet(set);
                }

                data.RemoveDataSet(set);
                data.NotifyDataChanged();
                mChart.NotifyDataSetChanged();
            }
        }


        private LineDataSet CreateSet(int datasetindex,String datasetname)
        {
            LineDataSet set = new LineDataSet(null, datasetname);
            set.AxisDependency = AxisDependency.Left;
            set.SetCircleColor(ColorTemplate.Rgb(pointColor[datasetindex]));
            set.LineWidth = 2f;
            set.CircleRadius = 4f;
            set.SetDrawFilled(true);
            set.SetDrawCircles(false);
            set.SetMode(LineDataSet.Mode.HorizontalBezier);// .CubicBezier);
            set.SetColors(ColorTemplate.Rgb(pointColor[datasetindex]));
            set.FillColor = ColorTemplate.Rgb(fillColor[datasetindex]);
            set.HighLightColor = Color.Rgb(244, 117, 117);
            set.ValueTextColor = Color.White;
            set.ValueTextSize = 9f;
            set.SetDrawValues(false);
            return set;
        }

        public void ClearValues()
        {
            mChart.ClearValues();
            ChartSettings();
        }
        
        public void SetVisibleXRangeMaximum(float max)
        {
            mChart.RefreshDrawableState();
            mChart.SetVisibleXRangeMinimum(max);
            mChart.SetVisibleXRangeMaximum(max);
            mChart.Invalidate();
        }
    }



}