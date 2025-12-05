namespace SMS.WebApp.Models
{
    public class ServiceResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}
