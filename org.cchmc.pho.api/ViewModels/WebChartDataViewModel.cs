using System;
using System.Collections.Generic;

namespace org.cchmc.pho.api.ViewModels
{
    public class WebChartViewModel
    {
        public int PracticeId { get; set; }
        public string Title { get; set; }
        public string HeaderLabel { get; set; }
        public decimal? VerticalMax { get; set; }
        public List<WebChartDataSetViewModel> DataSets { get; set; }
    }
    public class WebChartDataSetViewModel
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
    }
}
