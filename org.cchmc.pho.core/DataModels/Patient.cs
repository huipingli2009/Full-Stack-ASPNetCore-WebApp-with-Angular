using System;
using System.Collections.Generic;

namespace org.cchmc.pho.core.DataModels
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PracticeID { get; set; }        
        public int PCP_StaffID { get; set; }
        public DateTime? DOB { get; set; }
        public PatientStatus Status { get; set; }
        public DateTime? LastEDVisit { get; set; }
        public bool Chronic { get; set; }
        public bool WatchFlag { get; set; }
        public List<PatientCondition> Conditions { get; set; }
    }
    public class PatientStatus
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public PatientStatus(string id, string name)
        {
            int.TryParse(id, out int intId);
            ID = intId;
            Name = name;
        }
        public PatientStatus()
        {
        }
    }

    public class PatientCondition
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public PatientCondition(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public PatientCondition()
        {
        }
    }
}
