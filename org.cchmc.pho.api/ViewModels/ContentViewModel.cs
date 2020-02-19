using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.ViewModels
{
    public class ContentViewModel
    {
        public string Header { get; set; }
        public int PlacementOrder { get; set; }
        public string Body { get; set; }
        public string Hyperlink { get; set; }
        public string ImageHyperlink { get; set; }
        public string ContentPlacement { get; set; }
    }
}
