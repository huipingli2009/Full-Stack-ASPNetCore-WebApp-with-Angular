using AutoMapper;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;

namespace org.cchmc.pho.api.Mappings
{
    public class SelectPracticeMappings:Profile
    {
        public SelectPracticeMappings()
        {
            CreateMap<SelectPractice, SelectPracticeViewModel>();
        }        
    }
}
