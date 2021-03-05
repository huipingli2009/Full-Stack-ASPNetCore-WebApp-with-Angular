using System;
using System.Collections.Generic;

namespace org.cchmc.pho.api.ViewModels
{
    public class WebChartViewModel
    {
        public int PracticeId { get; set; }
        public string Title { get; set; }
        public string HeaderLabel { get; set; }
        public List<WebChartDataSetViewModel> DataSets { get; set; }
    }
    public class WebChartDataSetViewModel
    {
        public string Label { get; set; }
        public string Type { get; set; }
        public List<WebChartDataPointViewModel> DataPoints { get; set; }
    }
    public class WebChartDataPointViewModel
    {
        public string DataPoint { get; set; }
        public int DataValue { get; set; }
    }
}
