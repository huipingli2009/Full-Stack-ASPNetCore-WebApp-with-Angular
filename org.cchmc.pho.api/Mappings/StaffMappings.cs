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
            CreateMap<StaffDetails, StaffDetailsViewModel>();
            CreateMap<StaffDetailsViewModel, StaffDetails> ();
            CreateMap<Credential, CredentialViewModel>();
            CreateMap<Position, PositionViewModel>();
            CreateMap<PositionViewModel, Position> ();
            CreateMap<Responsibility, ResponsibilityViewModel>();
            CreateMap<ResponsibilityViewModel, Responsibility>();
            CreateMap<Provider, ProviderViewModel>();
            CreateMap<Location, LocationViewModel>();
            CreateMap<LocationViewModel, Location>();
        }
    }
}
