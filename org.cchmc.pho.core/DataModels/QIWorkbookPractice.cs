using System;
using System.Collections.Generic;
using System.Text;

namespace org.cchmc.pho.core.DataModels
{
    public class QIWorkbookPractice
    {
        public int FormResponseId { get; set; }
        public string Header { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string JobAidURL { get; set; }
        public string Line3 { get; set; }
    }




    public class QIWorkbookSection
    {
        public int FormResponseId { get; set; }
        public int SectionId { get; set; }
        public string SectionHeader { get; set; }
        public bool DataEntered { get; set; }

        public List<QIWorkbookQuestions> Questions { get; set; }
    }
    public class QIWorkbookQuestions
    {
        public int QuestionId { get; set; }
        public string QuestionDescription { get; set; }
        public int Numerator { get; set; }
        public int Denominator { get; set; }
        public bool DataEntered { get; set; }

    }





}
