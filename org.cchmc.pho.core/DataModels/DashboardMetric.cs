
namespace org.cchmc.pho.core.DataModels
{
    public class DashboardMetric
    {
        public int PracticeId { get; set; }
        public string DashboardLabel { get; set; }
        public int MeasureId { get; set; }
        public string MeasureDesc { get; set; }
        public string MeasureType { get; set; }
        public int PracticeTotal { get; set; }
        public int NetworkTotal { get; set; }
        public string OpDefURL { get; set; }  
        
    }
}
