using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.ViewModels
{
    public class EDChartViewModel
    {
        public int PracticeId { get; set; }
        public DateTime AdmitDate { get; set; }
        public string ChartLabel { get; set; }
        public string ChartTitle { get; set; }
        public int EDVisits { get; set; }
    }
}
