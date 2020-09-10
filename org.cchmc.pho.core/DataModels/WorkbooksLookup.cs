using System;

namespace org.cchmc.pho.core.DataModels
{
    public class WorkbooksLookup
    {
        public int FormId { get; set; }
        public int FormResponseId { get; set; }
        public int PracticeId { get; set; }
        public int QuestionId { get; set; }
        public string ReportingPeriod { get; set; }
    }
}
