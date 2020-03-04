using AutoMapper;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;

namespace org.cchmc.pho.api.Mappings
{
    public class StaffMappings : Profile
    {
        public StaffMappings()
        {
            CreateMap<Staff, StaffViewModel>();
            CreateMap<StaffDetail, StaffDetailViewModel>();
            CreateMap<StaffDetailViewModel, StaffDetail> ();
            CreateMap<Credential, CredentialViewModel>();
            CreateMap<Position, PositionViewModel>();
            CreateMap<Responsibility, ResponsibilityViewModel>();
        }
    }
}
