using org.cchmc.pho.core.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.Interfaces
{
    public interface IAlert
    {
        Task<List<Alert>> ListActiveAlerts(int userId);
        Task MarkAlertAction(int alertScheduleId, int userId, int alertActionId);
      
    }
}
