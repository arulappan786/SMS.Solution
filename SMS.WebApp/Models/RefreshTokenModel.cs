namespace SMS.WebApp.Models
{
    public class RefreshTokenModel
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
