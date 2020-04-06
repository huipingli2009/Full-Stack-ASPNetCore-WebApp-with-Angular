using org.cchmc.pho.core.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.Interfaces
{
    public interface IFiles
    {
        Task<List<File>> ListFiles(int userId, int resourceTypeId, int initiativeId, string tag, bool watch, string name);
    }
}
