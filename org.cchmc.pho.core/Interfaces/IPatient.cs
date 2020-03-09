using org.cchmc.pho.core.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.Interfaces
{
    public interface IPatient
    {   
        Task<List<Patient>> ListActivePatient(int userId, int? staffID, int? popmeasureID, bool? watch, bool? chronic, string conditionIDs, string namesearch, string sortcolumn, string sortdirection, int? pagenumber, int? rowsperpage);
        Task<PatientDetails> GetPatientDetails(int patientId);
        Task<PatientDetails> UpdatePatientDetails(int userId, PatientDetails patientDetail);
        Task<List<PatientCondition>> GetPatientConditionsAll();
        Task<List<PatientInsurance>> GetPatientInsuranceAll(int userId);
        bool IsPatientInSamePractice(int userId, int patientId);
    }
}
