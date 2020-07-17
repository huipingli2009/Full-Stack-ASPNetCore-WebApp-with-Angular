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
            CreateMap<FileTypeViewModel, FileType>();
            CreateMap<ResourceType, ResourceTypeViewModel>();
            CreateMap<ResourceTypeViewModel, ResourceType>();
            CreateMap<Initiative, InitiativeViewModel>();
            CreateMap<InitiativeViewModel, Initiative>();
            CreateMap<Resource, ResourceViewModel>();
            CreateMap<FileAction, FileActionViewModel>();
            CreateMap<FileActionViewModel, FileAction>();
            CreateMap<WebPlacement, WebPlacementViewModel>();
            CreateMap<WebPlacementViewModel, WebPlacement>();
        }
    }
}
