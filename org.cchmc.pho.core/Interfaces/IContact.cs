using org.cchmc.pho.core.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.Interfaces
{
    public interface IContact
    {
        Task<List<Contact>> GetContacts(int userId, bool? qpl, string specialty, string membership, string board, string namesearch);
        Task<ContactPracticeDetails> GetContactPracticeDetails(int userId, int practiceId);
    }
}
