using System;

namespace org.cchmc.pho.core.DataModels
{
    public class EDChart
    {
        public int PracticeId { get; set; }
        public DateTime AdmitDate { get; set; }
        public string ChartLabel { get; set; }
        public string ChartTitle { get; set; }
        public int EDVisits { get; set; }
        public string ChartTopLeftLabel { get; set; }
    }
}
