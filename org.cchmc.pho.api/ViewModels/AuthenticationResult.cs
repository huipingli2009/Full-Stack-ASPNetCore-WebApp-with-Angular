﻿namespace org.cchmc.pho.api.ViewModels
{
    public class AuthenticationResult
    {
        public string Status { get; set; }
        public UserViewModel User { get; set; }
        public string RefreshToken { get; set; }
    }
}
