using org.cchmc.pho.core.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.ViewModels
{
    public class PatientViewModel
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PracticeID { get; set; }
        //public int SortCol { get; set; }

        public int PCP_StaffID { get; set; }
        //public int SortCol { get; set; }
        //public int SortCol { get; set; }

        //public int SortCol { get; set; }
        //public int SortCol { get; set; }
       

    }
}
