using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.ViewModels
{
    public class WorkbooksProvidersViewModel
    {
        public int FormResponseID { get; set; }

        public int StaffID { get; set; }

        public string Provider { get; set; }

        public string PHQS { get; set; }

        public int TOTAL { get; set; }
    }
}
