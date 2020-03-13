namespace org.cchmc.pho.api.ViewModels
{
    public class AuthenticationRefreshRequest
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
