using Microsoft.Extensions.Options;
using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.DataAccessLayer
{
    // TODO: all ADO and DI setup
    public class AlertDAL : IAlert
    {
        private readonly string _connectionString;

        public AlertDAL(IOptions<ConnectionStrings> options)
        {
            _connectionString = options.Value.phodb;
        }

        public async Task<List<Alert>> ListActiveAlerts(int userId)
        {
            // this is where the code goes to return the alerts by user
            throw new NotImplementedException();
        }

        public async Task MarkAlertAction(int userId, int alertScheduleId, int alertActionId, DateTime actionDateTime)
        {
            // this is where the code goes to mark the alert activity
            throw new NotImplementedException();
        }
    }
}

