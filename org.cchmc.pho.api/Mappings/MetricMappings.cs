using AutoMapper;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;

namespace org.cchmc.pho.api.Mappings
{
    public class MetricMappings : Profile
    {
        public MetricMappings()
        {
            CreateMap<Metric, MetricViewModel>();
            CreateMap<EDChart, EDChartViewModel>();
            CreateMap<EDDetail, EDDetailViewModel>();
        }
    }
}
