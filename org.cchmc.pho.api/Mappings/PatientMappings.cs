using AutoMapper;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;

namespace org.cchmc.pho.api.Mappings
{
    public class PatientMappings:Profile
    {
        public PatientMappings()
        {
            CreateMap<Patient, PatientViewModel>().ForMember(dest => dest.PendingStatusConfirmation, action => action.MapFrom(source => source.PotentiallyActiveStatus));
            CreateMap<PatientViewModel, Patient>();
            CreateMap<PatientCondition, PatientConditionViewModel>();
            CreateMap<PatientConditionViewModel, PatientCondition>();
            CreateMap<PatientInsurance, PatientInsuranceViewModel>();
            CreateMap<PatientDetails, PatientDetailsViewModel>().ForMember(dest => dest.ClarityPatientId, action => action.MapFrom(source => source.PatId))
                                                                 .ForMember(dest => dest.PendingStatusConfirmation, action => action.MapFrom(source => source.PotentiallyActiveStatus));
            CreateMap<PatientDetailsViewModel, PatientDetails>().ForMember(dest => dest.PatId, action => action.MapFrom(source => source.ClarityPatientId))
                                                                 .ForMember(dest => dest.PotentiallyActiveStatus, action => action.MapFrom(source => source.PendingStatusConfirmation));
            CreateMap<PMCA, PMCAViewModel>();
            CreateMap<Gender, GenderViewModel>();
            CreateMap<State, StateViewModel>();
            CreateMap<SimplifiedPatient, SimplifiedPatientViewModel>();           
        }
    }
}
