using AutoMapper;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;
using static org.cchmc.pho.api.ViewModels.PatientViewModel;
using static org.cchmc.pho.core.DataModels.Patient;

namespace org.cchmc.pho.api.Mappings
{
    public class PatientMappings:Profile
    {
        public PatientMappings()
        {
            CreateMap<Patient, PatientViewModel>();
            CreateMap<Patient, PatientViewModel>().ForMember(dest => dest.PendingStatusConfirmation, action => action.MapFrom(source => source.PotentiallyActiveStatus));
            CreateMap<PatientCondition, PatientConditionViewModel>();
            CreateMap<PatientInsurance, PatientInsuranceViewModel>();
            CreateMap<PatientDetails, PatientDetailsViewModel>();
            CreateMap<PatientDetails, PatientDetailsViewModel>().ForMember(dest => dest.ClarityPatientId, action => action.MapFrom(source => source.PatId))
                                                                 .ForMember(dest => dest.PendingStatusConfirmation, action => action.MapFrom(source => source.PotentiallyActiveStatus));
          

        }
    }
}
