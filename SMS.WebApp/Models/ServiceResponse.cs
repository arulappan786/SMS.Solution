namespace SMS.WebApp.Models
{
    public class ServiceResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ServiceResponse<T> : ServiceResponse where T : class
    {
        
        public T? Data { get; set; }
    }
}
