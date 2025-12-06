namespace SMS.WebApp.Services.Toaster
{
    public class ToastService
    {
        // 1. The event components subscribe to
        public event Action<string, ToastLevel>? OnShow;

        // 2. The method components call to trigger the event
        public void ShowToast(string message, ToastLevel level)
        {
            OnShow?.Invoke(message, level);
        }

        // Convenience methods for easier use
        public void ShowSuccess(string message) => ShowToast(message, ToastLevel.Success);
        public void ShowError(string message) => ShowToast(message, ToastLevel.Error);
        // ... add more for Info and Warning as needed
    }
}
