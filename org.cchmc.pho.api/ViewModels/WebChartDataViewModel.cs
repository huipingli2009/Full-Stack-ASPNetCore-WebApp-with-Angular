using System;

namespace org.cchmc.pho.api.ViewModels
{
    public class WebChartDataViewModel
    {
        public int PracticeId { get; set; }
        public DateTime AdmitDate { get; set; }
        public string ChartLabel { get; set; }
        public string ChartTitle { get; set; }
        public int BarValue1 { get; set; }
        public int LineValue1 { get; set; }
        public int LineValue2 { get; set; }
        public string ChartTopLeftLabel { get; set; }
    }
}
