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
            CreateMap<PopularFile, PopularFileViewModel>();
            CreateMap<FileDetails, FileDetailsViewModel>();
            CreateMap<FileDetailsViewModel, FileDetails>();
            CreateMap<FileTag, FileTagViewModel>();
            CreateMap<FileTagViewModel, FileTag>();
            CreateMap<FileType, FileTypeViewModel>();
            CreateMap<ResourceType, ResourceTypeViewModel>();
            CreateMap<Initiative, InitiativeViewModel>();
            CreateMap<Resource, ResourceViewModel>();
            CreateMap<FileAction, FileActionViewModel>();
            CreateMap<FileActionViewModel, FileAction>();
        }
    }
}
