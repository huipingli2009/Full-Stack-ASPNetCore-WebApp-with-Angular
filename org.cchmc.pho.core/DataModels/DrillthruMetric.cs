using System;
using System.Collections.Generic;
using System.Text;

namespace org.cchmc.pho.core.DataModels
{
    public class DrillthruMetricTable
    {
        public List<DrillthruRow> Rows { get; set; }
    }

    public class DrillthruRow
    {
        public int RowNumber { get; set; }
        public int Order { get; set; }
        public List<DrillthruColumn> Columns { get; set; }
    }

    public class DrillthruColumn
    {
        public int Order { get; set; }
        public string Value { get; set; }
        public string ColumnName { get; set; }
    }

}
