using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Localization;
using Newtonsoft.Json;
using SMS.WebApp.Authentication;
using SMS.WebApp.Models;
using System.Globalization;
using System.Net.Http.Headers;

namespace SMS.WebApp
{
    public class ApiClient(
        HttpClient httpClient,
        ProtectedLocalStorage localStorage,
        NavigationManager navigationManager,
        AuthenticationStateProvider authStateProvider)
    {
        public async Task SetAuthorizeHeader()
        {
            try
            {
                // 1. Retrieve the stored user session (which contains tokens) from local storage.
                var sessionState = (await localStorage.GetAsync<LoginResponseModel>("sessionState")).Value;

                // 2. Check if a valid session and an Access Token exist.
                if (sessionState != null && !string.IsNullOrEmpty(sessionState.AccessToken))
                {
                    // --- Expiration Check Logic ---

                    // If the token is fully expired, log out the user.
                    if (sessionState.ExpiresInSeconds < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                    {
                        // Clear authentication state.
                        await ((CustomAuthStateProvider)authStateProvider).MarkUserAsLoggedOut();
                        // Redirect user to the login page.
                        navigationManager.NavigateTo("/login");
                    }
                    // If the token is nearing expiration (e.g., within the next 10 minutes), attempt to refresh it.
                    else if (sessionState.ExpiresInSeconds < DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds())
                    {
                        // Create the model containing current tokens for the refresh request.
                        var refreshTokenModel = new RefreshTokenModel
                        {
                            AccessToken = sessionState.AccessToken,
                            RefreshToken = sessionState.RefreshToken
                        };

                        // --- API Client Token Interception/Refresh ---

                        // Send a request to the refresh token endpoint.
                        // NOTE: This call typically does NOT include the existing Access Token
                        // in the header, as the token is being sent in the request body.
                        var res = await httpClient.PostAsJsonAsync<RefreshTokenModel>($"/api/auth/refreshtoken", refreshTokenModel, default);

                        // Read and deserialize the new token response into LoginResponseModel.
                        var resContent = await res.Content.ReadFromJsonAsync<LoginResponseModel>();

                        // Check if the refresh request was successful (res != null is often too weak; res.IsSuccessStatusCode is better).
                        if (res != null)
                        {
                            // Update the authentication state with the new tokens (also handles saving to local storage).
                            await ((CustomAuthStateProvider)authStateProvider).MarkUserAsAuthenticated(resContent!);

                            // --- SETTING THE AUTHORIZATION HEADER ---
                            // **Interceptor Action:** Set the new Access Token as the default Authorization header 
                            // for all subsequent requests made by this HttpClient instance.
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", resContent?.AccessToken);
                        }
                        else
                        {
                            // Refresh failed, treat as logged out.
                            await ((CustomAuthStateProvider)authStateProvider).MarkUserAsLoggedOut();
                            navigationManager.NavigateTo("/login");
                        }
                    }
                    // If the token is valid and not close to expiry, use the existing token.
                    else
                    {
                        // --- SETTING THE AUTHORIZATION HEADER (Standard Use) ---
                        // Set the existing Access Token as the default Authorization header 
                        // for all subsequent requests made by this HttpClient instance.
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessionState.AccessToken);
                    }

                    // --- Other Headers (Culture/Localization) ---

                    // Prepare culture information for API requests.
                    var requestCulture = new RequestCulture(
                        CultureInfo.CurrentCulture,
                        CultureInfo.CurrentUICulture
                    );
                    var cultureCookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

                    // Add the culture information to the request headers (simulating a cookie).
                    httpClient.DefaultRequestHeaders.Add("Cookie", $"{CookieRequestCultureProvider.DefaultCookieName}={cultureCookieValue}");
                }
            }
            catch (Exception ex)
            {
                // Catch any exceptions during token retrieval or refresh (e.g., network errors, serialization issues).
                // Treat an error state as a mandatory re-login.
                navigationManager.NavigateTo("/login");
            }
        }

        public async Task<T?> GetFromJsonAsync<T>(string path)
        {
            await SetAuthorizeHeader();
            return await httpClient.GetFromJsonAsync<T>(path);
        }

        public async Task<T1?> PostAsync<T1, T2>(string path, T2 postModel)
        {
            await SetAuthorizeHeader();

            var res = await httpClient.PostAsJsonAsync(path, postModel);

            if (res != null && res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                return string.IsNullOrWhiteSpace(content) ? default : JsonConvert.DeserializeObject<T1>(content);
            }

            return default;
        }
        public async Task<T1?> PutAsync<T1, T2>(string path, T2 postModel)
        {
            await SetAuthorizeHeader();
            
            var res = await httpClient.PutAsJsonAsync(path, postModel);
            
            if (res != null && res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                return string.IsNullOrWhiteSpace(content) ? default : JsonConvert.DeserializeObject<T1>(content);
            }
            
            return default;
        }
        public async Task<T?> DeleteAsync<T>(string path)
        {
            await SetAuthorizeHeader();
            return await httpClient.DeleteFromJsonAsync<T>(path);
        }
    }
}