using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.ViewModels
{
    public class AlertViewModel
    {
        public int AlertId { get; set; }
        public int AlertScheduleId { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public string LinkText { get; set; }
        public string Definition { get; set; }
    }
}
