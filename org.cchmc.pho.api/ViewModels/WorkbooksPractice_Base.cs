using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.ViewModels
{
    public abstract class WorkbooksPractice_Base
    {
        public int FormResponseId { get; set; }
        public string Header { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string JobAidURL { get; set; }
        public string Line3 { get; set; }
    }
}
