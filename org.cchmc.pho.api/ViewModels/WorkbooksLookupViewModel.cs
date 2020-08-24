using System;

namespace org.cchmc.pho.api.ViewModels
{
    public class WorkbooksLookupViewModel
    {
        public int FormId { get; set; }
        public int FormResponseId { get; set; }
        public int PracticeId { get; set; }
        public int QuestionId { get; set; }
        public string ReportingPeriod { get; set; }
    }
}
