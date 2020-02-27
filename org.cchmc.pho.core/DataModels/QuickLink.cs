using System;
using System.Collections.Generic;
using System.Text;

namespace org.cchmc.pho.core.DataModels
{
    public class Quicklink
    {
        public int PlacementOrder { get; set; }
        public string Body { get; set; }
        public string Hyperlink { get; set; }
        public int LocationId { get; set; }
    }
}
