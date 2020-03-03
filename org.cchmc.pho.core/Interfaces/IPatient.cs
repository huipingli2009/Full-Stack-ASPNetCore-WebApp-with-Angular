﻿using org.cchmc.pho.core.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.Interfaces
{
    public interface IPatient
    {   
        Task<List<Patient>> ListActivePatient(int userId, int? staffID, int? popmeasureID, bool? watch, bool? chronic, string conditionIDs, string namesearch, string sortcolumn, string sortdirection, int? pagenumber, int? rowsperpage);
        Task<PatientDetails> GetPatientDetails(int patientId);
    }
}
