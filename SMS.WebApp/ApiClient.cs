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
        public async Task<T> GetFromJsonAsync<T>(string path)
        {
            await SetAuthorizeHeader();
            var result = await httpClient.GetFromJsonAsync<T>(path);
            if (result == null)
            {
                // You can throw, return default, or handle as needed. Here, returning default to avoid CS8603.
                return default!;
            }
            return result;
        }
        public async Task<T1> PostAsync<T1, T2>(string path, T2 postModel)
        {
            await SetAuthorizeHeader();

            var res = await httpClient.PostAsJsonAsync(path, postModel);
            if (res != null && res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T1>(content);
                return result ?? default!;
            }
            return default!;
        }
        public async Task<T1> PutAsync<T1, T2>(string path, T2 postModel)
        {
            await SetAuthorizeHeader();
            var res = await httpClient.PutAsJsonAsync(path, postModel);
            if (res != null && res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T1>(content);
                return result ?? default!;
            }
            return default!;
        }
        public async Task<T> DeleteAsync<T>(string path)
        {
            await SetAuthorizeHeader();
            var result = await httpClient.DeleteFromJsonAsync<T>(path);
            if (result == null)
            {
                // Return default to avoid CS8603: Possible null reference return.
                return default!;
            }
            return result;
        }
    }
}

