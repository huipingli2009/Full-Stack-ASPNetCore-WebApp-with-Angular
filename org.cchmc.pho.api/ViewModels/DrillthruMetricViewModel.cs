using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.ViewModels
{
    public class DrillthruMetricTableViewModel
    {
        public List<DrillthruRowViewModel> Rows { get; set; }
    }

    public class DrillthruRowViewModel
    {
        public int RowNumber { get; set; }
        public int Order { get; set; }
        public List<DrillthruColumnViewModel> Columns { get; set; }
    }

    public class DrillthruColumnViewModel
    {
        public int Order { get; set; }
        public string Value { get; set; }
        public string ColumnName { get; set; }
    }
}
