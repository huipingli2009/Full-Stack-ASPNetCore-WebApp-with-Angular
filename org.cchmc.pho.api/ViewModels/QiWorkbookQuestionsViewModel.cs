
using System.Collections.Generic;

namespace org.cchmc.pho.api.ViewModels
{
    public class QIWorkbookQuestionsViewModel
    {
        public int FormResponseId { get; set; }     
        public List<SectionViewModel> QiSection { get; set; }       
    }

    public class SectionViewModel
   {
        public int SectionId { get; set; }
        public string SectionHeader { get; set; }
        public List<QuestionViewModel> QiQuestion { get; set; }
        public bool DataEntered { get; set; }
    }
   public class QuestionViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionDEN { get; set; }
        public string QuestionNUM { get; set; }
        public string NumeratorLabel { get; set; }
        public int Numerator { get; set; }
        public int Denominator { get; set; }
        
    }

}
