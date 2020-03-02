using org.cchmc.pho.core.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.Interfaces
{
    public interface IWorkbooks
    {       
        Task<List<WorkbooksPatient>> ListPatients(int userId, DateTime reportingDate, string nameSearch);
    }
}
