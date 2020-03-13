using org.cchmc.pho.core.DataModels;
using System.Collections.Generic;

namespace org.cchmc.pho.api.ViewModels
{
    public class SelectPracticeViewModel
    {
        public int CurrentPracticeId { get; set; }
        public List<Practice> PracticeList { get; set; }
    }
}
