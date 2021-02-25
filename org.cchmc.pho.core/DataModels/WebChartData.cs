using System;
using System.Collections.Generic;

namespace org.cchmc.pho.core.DataModels
{

    public class WebChart
    {
        public int PracticeId { get; set; }
        public string Title { get; set; }
        public string HeaderLabel { get; set; }
        public List<WebChartDataSet> DataSets { get; set; }
    }
    public class WebChartDataSet
    {
        public string Label { get; set; }
        public string Type { get; set; }
        public List<WebChartDataPoint> DataPoints { get; set; }
    }
    public class WebChartDataPoint
    {
        public string DataPoint { get; set; }
        public int DataValue { get; set; }
    }
}
