﻿using System;
using System.Collections.Generic;

namespace org.cchmc.pho.core.DataModels
{
    public class ContactPracticeDetails
    {
        public int PracticeId { get; set; }
        public string PracticeName { get; set; }
        public DateTime? MemberSince { get; set; }
        public string PracticeManager { get; set; }
        public string PMEmail { get; set; }
        public string PIC { get; set; }
        public string PICEmail { get; set; }
        public List<ContactPracticeLocation> ContactPracticeLocations { get; set; }
    }
}
