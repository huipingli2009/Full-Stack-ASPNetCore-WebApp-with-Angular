﻿
namespace org.cchmc.pho.core.DataModels
{
    public class QIWorkbookQuestions
    {
        public int FormResponseId { get; set; }
        public int QuestionId { get; set; }
        public string SectionHeader { get; set; }
        public string QuestionDEN { get; set; }
        public string QuestionNUM { get; set; }
        public string NumeratorLabel { get; set; }
        public int Numerator { get; set; }
        public int Denominator { get; set; }
        public string DataEntered { get; set; }       
    }
}
