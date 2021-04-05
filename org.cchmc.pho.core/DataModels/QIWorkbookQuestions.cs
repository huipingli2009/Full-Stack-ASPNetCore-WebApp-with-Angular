
using System.Collections.Generic;

namespace org.cchmc.pho.core.DataModels
{
    public class QIWorkbookQuestions
    {
        public int FormResponseId { get; set; }     
        public List<Section> QiSection { get; set; }       
             
    }

    public class Section
    {
        public int SectionId { get; set; }
        public string SectionHeader { get; set; }
        public List<Question> QiQuestion { get; set; }
        public bool DataEntered { get; set; }
    }
    public class Question
    {
        public int QuestionId { get; set; }
        public string QuestionDEN { get; set; }
        public string QuestionNUM { get; set; }
        public string NumeratorLabel { get; set; }
        public int Numerator { get; set; }
        public int Denominator { get; set; }       
    }
}
