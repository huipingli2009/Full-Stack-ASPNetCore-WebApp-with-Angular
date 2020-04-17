using org.cchmc.pho.core.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.Interfaces
{
    public interface IFiles
    {
        Task<List<File>> ListFiles(int userId, int? resourceTypeId, int? initiativeId, string tag, bool? watch, string name);
        Task<FileDetails> GetFileDetails(int userId, int fileId);
        Task<FileDetails> UpdateFileDetails(int userId, FileDetails fileDetails);
        Task<FileDetails> AddFileDetails(int userId, FileDetails fileDetails);
        Task<bool> UpdateFileWatch(int userId, int resourceId);
        Task<bool> RemoveFile(int userId, int resourceId);
        Task<List<FileTag>> GetFileTagsAll();
        Task<List<Resource>> GetResourceAll();
        Task<List<Initiative>> GetInitiativeAll();
        Task<List<FileType>> GetFileTypesAll();
        Task MarkFileAction(int resourceId, int userId, int actionId);
    }
}
