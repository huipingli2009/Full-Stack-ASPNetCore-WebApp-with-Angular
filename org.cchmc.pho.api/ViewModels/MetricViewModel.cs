﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.ViewModels
{
    public class MetricViewModel
    {
        public int PracticeId { get; set; }
        public string DashboardLabel { get; set; }
        public string MeasureDesc { get; set; }
        public string MeasureType { get; set; }
        public int PracticeTotal { get; set; }
        public int NetworkTotal { get; set; }
    }
}
