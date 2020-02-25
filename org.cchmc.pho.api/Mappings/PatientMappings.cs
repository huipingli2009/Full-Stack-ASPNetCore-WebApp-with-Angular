using AutoMapper;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;
using System.Collections.Generic;
using static org.cchmc.pho.api.ViewModels.PatientViewModel;
using static org.cchmc.pho.core.DataModels.Patient;

namespace org.cchmc.pho.api.Mappings
{
    public class PatientMappings:Profile
    {
        public PatientMappings()
        {
            CreateMap<Patient, PatientViewModel>();
            CreateMap<PatientStatus,PatientStatusViewModel>();
            CreateMap<PatientCondition, PatientConditionViewModel>();
            CreateMap<PatientDetails, PatientDetailsViewModel>();
        }       
    }
}
