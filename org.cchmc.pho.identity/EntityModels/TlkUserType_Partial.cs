using org.cchmc.pho.identity.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
