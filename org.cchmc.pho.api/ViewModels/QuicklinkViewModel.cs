using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.ViewModels
{
    public class QuicklinkViewModel
    {
        public int PlacementOrder { get; set; }
        public string Body { get; set; }
        public string Hyperlink { get; set; }
        public int LocationId { get; set; }
    }
}
