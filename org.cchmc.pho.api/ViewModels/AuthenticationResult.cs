using System.Collections.Generic;

namespace org.cchmc.pho.api.ViewModels
{
    public class AuthenticationResult
    {
        public string Status { get; set; }
        public UserViewModel User { get; set; }
    }
}
