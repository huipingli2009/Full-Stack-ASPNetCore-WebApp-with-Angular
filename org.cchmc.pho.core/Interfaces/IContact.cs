using org.cchmc.pho.core.DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace org.cchmc.pho.core.Interfaces
{
    public interface IContact
    {
        Task<List<Contact>> GetContacts(int userId, bool? qpl, string specialty, string membership, string board, string namesearch);
        Task<ContactPracticeDetails> GetContactPracticeDetails(int userId, int practiceId);
        Task<List<ContactPracticeLocation>> GetContactPracticeLocations(int userId, int practiceId);
        Task<List<ContactPracticeStaff>>GetContactPracticeStaffList(int userId, int practiceId);
        Task<ContactPracticeStaffDetails> GetContactStaffDetails(int userId, int staffId);
        Task<List<PHOMembership>> GetContactPracticePHOMembership();
        Task<List<Specialty>> GetContactPracticeSpecialties();
        Task<List<BoardMembership>> GetContactPracticeBoardMembership();
        Task<List<Staff>> GetContactEmailList(int userId, bool? managers, bool? physicians, bool? all);
    }
}
