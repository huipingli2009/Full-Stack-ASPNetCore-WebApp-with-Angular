using System;

namespace org.cchmc.pho.api.ViewModels
{
    public class ContactPracticeDetailsVidewModel
    {
        public int PracticeId { get; set; }
        public string PracticeName { get; set; }
        public DateTime? MemberSince { get; set; }
        public string PracticeManager { get; set; }
        public string PMEmail { get; set; }
        public string PIC { get; set; }
        public string PICEmail { get; set; }
    }
}
