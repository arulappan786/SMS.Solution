namespace SMS.WebApp.Models
{
    public class BaseResponseModel
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}
