using AutoMapper;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;

namespace org.cchmc.pho.api.Mappings
{
    public class FileMappings : Profile
    {
        public FileMappings()
        {
            CreateMap<File, FileViewModel>();
            //CreateMap<StaffDetail, StaffDetailViewModel>();
            //CreateMap<StaffDetailViewModel, StaffDetail>();
            CreateMap<FileTag, FileTagViewModel>();
            CreateMap<Initiative, InitiativeViewModel>();
            CreateMap<Resource, ResourceViewModel>();
        }
    }
}
