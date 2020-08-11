using org.cchmc.pho.core.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.Interfaces
{
    public interface IMetric
    {
        Task<List<DashboardMetric>> ListDashboardMetrics(int userId);
        Task<List<EDChart>> ListEDChart(int userId);
        Task<List<EDDetail>> ListEDDetails(int userId, DateTime admitDate);
        Task<List<PopulationMetric>> ListPopulationMetrics();
        Task<DrillthruMetricTable> GetDrillthruTable(int userId, int measureId, int filterId);
    }
}
