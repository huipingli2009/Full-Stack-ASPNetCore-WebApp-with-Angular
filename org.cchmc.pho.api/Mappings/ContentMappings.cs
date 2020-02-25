using AutoMapper;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;

namespace org.cchmc.pho.api.Mappings
{
    public class ContentMappings:Profile
    {
        public ContentMappings()
        {
            CreateMap<SpotLight, SpotLightViewModel>();
        }        
    }
}
