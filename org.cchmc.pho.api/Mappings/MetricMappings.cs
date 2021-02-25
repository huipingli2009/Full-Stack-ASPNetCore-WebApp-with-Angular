using AutoMapper;
using org.cchmc.pho.api.ViewModels;
using org.cchmc.pho.core.DataModels;

namespace org.cchmc.pho.api.Mappings
{
    public class MetricMappings : Profile
    {
        public MetricMappings()
        {
            CreateMap<DashboardMetric, DashboardMetricViewModel>();
            CreateMap<WebChartData, WebChartDataViewModel>();
            CreateMap<EDDetail, EDDetailViewModel>();
            CreateMap<PopulationMetric, PopulationMetricViewModel>();
            CreateMap<DrillthruMetricTable, DrillthruMetricTableViewModel>();
            CreateMap<DrillthruRow, DrillthruRowViewModel>();
            CreateMap<DrillthruColumn, DrillthruColumnViewModel>();
            CreateMap<WebChartFilters, WebChartFiltersViewModel>();
        }
    }
}
