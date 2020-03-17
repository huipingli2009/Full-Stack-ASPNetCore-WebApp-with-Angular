using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.Interfaces
{
    public interface IPatient
    {   
        Task<SearchResults<Patient>> ListActivePatient(int userId, int? staffID, int? popmeasureID, bool? watch, bool? chronic, string conditionIDs, string namesearch, string sortcolumn, string sortdirection, int? pagenumber, int? rowsperpage);
        Task<PatientDetails> GetPatientDetails(int patientId);
        Task<PatientDetails> UpdatePatientDetails(int userId, PatientDetails patientDetail);
        Task<bool> UpdatePatientWatchlist(int userId, int patientId);
        Task<List<SimplifiedPatient>> SearchSimplifiedPatients(int userId, string search);
        Task<List<PatientCondition>> GetPatientConditionsAll();
        Task<List<PatientInsurance>> GetPatientInsuranceAll(int userId);
        Task<List<Gender>> ListGender();
        Task<List<State>> ListState();
        Task<List<PMCA>> ListPMCA();
        bool IsPatientInSamePractice(int userId, int patientId);
    }
}
