﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace org.cchmc.pho.core.DataModels
{

    public class WebChart
    {
        public WebChart() { }
        public WebChart(int practiceId, string[] title, string headerLabel)
        {
            PracticeId = practiceId;
            Title = title;
            HeaderLabel = headerLabel;
            DataSets = new List<WebChartDataSet>();
        }
        public int PracticeId { get; set; }
        public string[] Title { get; set; }
        public string HeaderLabel { get; set; }
        public decimal? VerticalMax { get; set; }
        public List<WebChartDataSet> DataSets { get; set; }
    }
    public class WebChartDataSet
    {
        public string Type { get; set; }
        public string[] XAxisLabels { get; set; }
        public decimal[] Values { get; set; }
        public string Legend { get; set; }
        public string BackgroundColor { get; set; }
        public string BackgroundHoverColor { get; set; }
        public string BorderColor { get; set; }
        public bool Fill { get; set; }
        public bool ShowLine { get; set; }
        public int[] BorderDash { get; set; }
        public string[] PointStyle { get; set; }
        public int[] PointRadius { get; set; }
        public string[] PointBackgroundColor { get; set; }
    }
}
