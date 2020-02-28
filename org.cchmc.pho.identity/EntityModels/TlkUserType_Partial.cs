using org.cchmc.pho.identity.Models;

namespace org.cchmc.pho.identity.EntityModels
{
    public partial class TlkUserType
    {
        public Role BuildRole()
        {
            return new Role()
            {
                Id = Id,
                Name = Name
            };
        }
    }
}
