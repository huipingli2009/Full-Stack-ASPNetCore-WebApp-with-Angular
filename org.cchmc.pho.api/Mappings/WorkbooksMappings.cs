using AutoMapper;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;

namespace org.cchmc.pho.api.Mappings
{
    public class WorkbooksMappings : Profile
    {
        public WorkbooksMappings()
        {
            CreateMap<WorkbooksPatient, WorkbooksPatientViewModel>();
            CreateMap<WorkbooksPractice, WorkbooksPracticeViewModel>();
            CreateMap<WorkbooksProvider, WorkbooksProviderViewModel>();
            CreateMap<WorkbooksLookup, WorkbooksLookupViewModel>();
            CreateMap<WorkbooksPatientFollowup, WorkbooksPatientFollowupViewModel>();
            CreateMap<WorkbookPatientNameSearch, WorkbookPatientNameSearchViewModel>();
        }        
    }
}
