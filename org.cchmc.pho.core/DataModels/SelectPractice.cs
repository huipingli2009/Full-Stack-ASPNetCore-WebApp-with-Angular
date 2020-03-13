using System.Collections.Generic;

namespace org.cchmc.pho.core.DataModels
{
    public class SelectPractice
    {
        public int CurrentPracticeId { get; set; }
        public List<Practice> PracticeList { get; set; }
        public SelectPractice()
        {

        }
    }
}
