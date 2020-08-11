using AutoMapper;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;

namespace org.cchmc.pho.api.Mappings
{
    public class WorkbooksMappings : Profile
    {
        public WorkbooksMappings()
        {
            CreateMap<core.DataModels.WorkbooksDepressionPatient, WorkbooksDepressionPatientViewModel>();
            CreateMap<WorkbooksPractice, WorkbooksPracticeViewModel>();
            CreateMap<WorkbooksProvider, WorkbooksProviderViewModel>();
            CreateMap<WorkbooksLookup, WorkbooksLookupViewModel>();
            CreateMap<WorkbooksPatientFollowup, WorkbooksPatientFollowupViewModel>();
            CreateMap<WorkbooksInitiatives, WorkbooksInitiativesViewModel>();
        }        
    }
}
