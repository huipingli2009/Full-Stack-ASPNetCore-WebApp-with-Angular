using AutoMapper;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;

namespace org.cchmc.pho.api.Mappings
{
    public class WorkbooksMappings : Profile
    {
        public WorkbooksMappings()
        {
            CreateMap<WorkbooksDepressionPatient, WorkbooksDepressionPatientViewModel>();
            CreateMap<WorkbooksDepressionConfirmation, WorkbooksDepressionConfirmationViewModel>();
            CreateMap<WorkbooksDepressionConfirmationViewModel, WorkbooksDepressionConfirmation>();
            CreateMap<WorkbooksPractice, WorkbooksPracticeViewModel>();
            CreateMap<WorkbooksProvider, WorkbooksProviderViewModel>();
            CreateMap<WorkbooksLookup, WorkbooksLookupViewModel>();
            CreateMap<WorkbooksPatientFollowup, WorkbooksPatientFollowupViewModel>();
            CreateMap<WorkbooksForms, WorkbooksFormsViewModel>();
            CreateMap<AsthmaTreatmentPlan, AsthmaTreatmentPlanViewModel>();
            CreateMap<AsthmaWorkbooksPractice, AsthmaWorkbooksPracticeViewModel>();
            CreateMap<WorkbooksAsthmaPatient, WorkbooksAsthmaPatientViewModel>();
            CreateMap<AsthmaTreatmentPlanViewModel, AsthmaTreatmentPlan>();
            CreateMap<QIWorkbookPractice, QIWorkbookPracticeViewModel>();
            
        }        
    }
}
