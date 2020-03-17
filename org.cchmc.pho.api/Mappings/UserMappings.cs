using AutoMapper;
using org.cchmc.pho.identity.Models;
using org.cchmc.pho.api.ViewModels;

namespace org.cchmc.pho.api.Mappings
{
    public class UserMappings : Profile
    {
        public UserMappings()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.NewPassword, action => action.Ignore())
                .ForMember(dest => dest.RefreshToken, action => action.Ignore());
            CreateMap<UserViewModel, User>();
            CreateMap<Role, RoleViewModel>();
            CreateMap<RoleViewModel, Role>();
        }
    }
}
