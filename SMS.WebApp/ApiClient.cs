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
    public class ApiClient(HttpClient httpClient, ProtectedLocalStorage localStorage, NavigationManager navigationManager, AuthenticationStateProvider authStateProvider)
    {
        public async Task SetAuthorizeHeader()
        {
            try
            {
                var sessionState = (await localStorage.GetAsync<LoginResponseModel>("sessionState")).Value;
                if (sessionState != null && !string.IsNullOrEmpty(sessionState.AccessToken))
                {
                    if (sessionState.ExpiresInSeconds < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                    {
                        await ((CustomAuthStateProvider)authStateProvider).MarkUserAsLoggedOut();
                        navigationManager.NavigateTo("/login", forceLoad: true);
                    }
                    else if (sessionState.ExpiresInSeconds < DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds())
                    {
                        var refreshTokenModel = new RefreshTokenModel
                        {
                            AccessToken = sessionState.AccessToken,
                            RefreshToken = sessionState.RefreshToken
                        };
                        
                        var res = await httpClient.PostAsJsonAsync<RefreshTokenModel>($"/api/auth/refreshtoken", refreshTokenModel);

                        if(res != null && res.IsSuccessStatusCode)
                        {
                            var content = await res.Content.ReadAsStringAsync();
                            var loginResponse = JsonConvert.DeserializeObject<ServiceResponse<LoginResponseModel>>(content);

                            if(loginResponse == null)
                            {
                                await ((CustomAuthStateProvider)authStateProvider).MarkUserAsLoggedOut();
                                navigationManager.NavigateTo("/login", forceLoad: true);
                                return;
                            }
                            
                            await ((CustomAuthStateProvider)authStateProvider).MarkUserAsAuthenticated(loginResponse.Data);
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Data.AccessToken);
                        }
                        else
                        {
                            await ((CustomAuthStateProvider)authStateProvider).MarkUserAsLoggedOut();
                            navigationManager.NavigateTo("/login", forceLoad: true);
                        }                       
                    }
                    else
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessionState.AccessToken);
                    }

                    var requestCulture = new RequestCulture(
                            CultureInfo.CurrentCulture,
                            CultureInfo.CurrentUICulture
                        );
                    var cultureCookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

                    httpClient.DefaultRequestHeaders.Add("Cookie", $"{CookieRequestCultureProvider.DefaultCookieName}={cultureCookieValue}");
                }
            }
            catch (Exception ex)
            {
                navigationManager.NavigateTo("/login", forceLoad: true);
            }
        }

        public async Task<T?> GetFromJsonAsync<T>(string path) where T : class
        {
            await SetAuthorizeHeader();

            try
            {
                // GetFromJsonAsync internally throws if status code is not 2xx
                var result = await httpClient.GetFromJsonAsync<T>(path);

                // result can be null if the API returns 204 No Content or null content
                return result;
            }
            catch (HttpRequestException ex) when (ex.StatusCode.HasValue && (int)ex.StatusCode.Value >= 400)
            {
                // Log the error (e.g., 404 Not Found, 500 Server Error)
                // You might want to re-throw the exception or return null based on your service's policy.
                Console.WriteLine($"API request failed: {ex.StatusCode}.");
                return null;
            }
            catch (Exception ex)
            {
                // Handle serialization errors, network issues, etc.
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<T1> PostAsync<T1, T2>(string path, T2 postModel)
        {
            await SetAuthorizeHeader();

            var res = await httpClient.PostAsJsonAsync(path, postModel);

            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                // Return the deserialized object, or throw if content is null/empty
                var result = JsonConvert.DeserializeObject<T1>(content);

                // Use ?? throw to satisfy non-nullable T1 return type if content is null
                return result ?? throw new Exception("API succeeded but returned null content.");
            }
            else
            {
                // On failure, read the error message for better debugging
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API call failed: {res.StatusCode}. Details: {errorContent}", null, res.StatusCode);
            }
        }

        public async Task<T1> PutAsync<T1, T2>(string path, T2 postModel)
        {
            await SetAuthorizeHeader();

            // Use PutAsJsonAsync to send the update model
            var res = await httpClient.PutAsJsonAsync(path, postModel);

            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();

                // Return the deserialized object, or throw if content is null/empty.
                // T1 must be able to handle null content if the API returns 204 No Content.
                var result = JsonConvert.DeserializeObject<T1>(content);

                return result ?? throw new InvalidOperationException("API PUT succeeded but returned null content.");
            }
            else
            {
                // On failure, read the error message for better diagnostics
                var errorContent = await res.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API PUT failed: {res.StatusCode}. Details: {errorContent}", null, res.StatusCode);
            }
        }


        public async Task<T> DeleteAsync<T>(string path)
        {
            await SetAuthorizeHeader();

            try
            {
                // DeleteFromJsonAsync internally throws if status code is not 2xx
                var result = await httpClient.DeleteFromJsonAsync<T>(path);

                // If the result is null (e.g., API returned 204 No Content),
                // we throw or return a default object *only if T is guaranteed to have one*.
                return result ?? throw new InvalidOperationException("API DELETE succeeded but returned null content.");
            }
            catch (HttpRequestException ex)
            {
                // This catches 4xx/5xx status codes thrown by DeleteFromJsonAsync
                // Log the exception and re-throw, or handle it specifically.
                Console.WriteLine($"API DELETE failed: {ex.StatusCode}.");
                throw;
            }
        }
    }
}

