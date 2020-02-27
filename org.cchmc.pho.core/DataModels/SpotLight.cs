using System;
using System.Collections.Generic;
using System.Text;

namespace org.cchmc.pho.core.DataModels
{
    public class SpotLight
    {
        public string Header { get; set; }
        public int PlacementOrder { get; set; }
        public string Body { get; set; }
        public string Hyperlink { get; set; }
        public string ImageHyperlink { get; set; }
        public int LocationId { get; set; }
    }
}
