﻿
namespace org.cchmc.pho.api.ViewModels
{
    public class DashboardMetricViewModel
    {
        public int PracticeId { get; set; }
        public string DashboardLabel { get; set; }
        public int MeasureId { get; set; }
        public int ConditionId { get; set; }
        public string MeasureDesc { get; set; }
        public string MeasureType { get; set; }
        public int PracticeTotal { get; set; }
        public int NetworkTotal { get; set; }
        public string OpDefURL { get; set; }
    }
}
