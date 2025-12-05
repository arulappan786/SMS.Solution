using System.Net.Http.Headers;

namespace SMS.WebApp
{
    // The ApiClient no longer needs to worry about tokens or navigation.
    // It only needs the HttpClient injected with the AuthTokenHandler.
    public class ApiClient(HttpClient httpClient)
    {
        // Generic GET method
        public async Task<T?> GetAsync<T>(string path)
        {
            // Token handling happens automatically in the DelegatingHandler
            return await httpClient.GetFromJsonAsync<T>(path);
        }

        // Generic POST method
        public async Task<TResponse?> PostAsync<TResponse, TRequest>(string path, TRequest postModel)
        {
            var res = await httpClient.PostAsJsonAsync(path, postModel);

            // Centralized success check and deserialization
            if (res.IsSuccessStatusCode)
            {
                // Use System.Text.Json extension method for deserialization
                return await res.Content.ReadFromJsonAsync<TResponse>();
            }

            // Handle and throw specific API exceptions (e.g., 400 Bad Request) for production logging
            // Or return default after logging the error status code
            return default;
        }

        // Generic PUT method
        public async Task<TResponse?> PutAsync<TResponse, TRequest>(string path, TRequest putModel)
        {
            var res = await httpClient.PutAsJsonAsync(path, putModel);

            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadFromJsonAsync<TResponse>();
            }

            return default;
        }

        // Generic DELETE method
        public async Task<TResponse?> DeleteAsync<TResponse>(string path)
        {
            // Note: DeleteFromJsonAsync only works if the server returns a body.
            var res = await httpClient.DeleteAsync(path);

            if (res.IsSuccessStatusCode)
            {
                // Check for empty body (204 No Content) vs. body with content
                if (res.Content.Headers.ContentLength == 0) return default;
                return await res.Content.ReadFromJsonAsync<TResponse>();
            }

            return default;
        }

        public async Task<HttpResponseMessage> LogoutAsync(string path)
        {
            // AuthTokenHandler will run and attach the Bearer token.
            // Send an empty request body.
            return await httpClient.PostAsync(path, new StringContent(string.Empty));
        }
    }

    // LogoutClient.cs - In the same namespace as ApiClient
    public class LogoutClient(HttpClient httpClient)
    {
        // Simple method to make a POST without internal token logic
        public async Task<HttpResponseMessage> PostAsync(string path, string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, path);

            // MANUALLY attach the token for this specific request
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(string.Empty); // Empty body

            return await httpClient.SendAsync(request);
        }
    }
}