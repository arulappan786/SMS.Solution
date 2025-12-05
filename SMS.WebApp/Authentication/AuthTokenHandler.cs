using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Localization;
using SMS.WebApp.Models;
using System.Net;
using System.Net.Http.Headers;

namespace SMS.WebApp.Authentication
{
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly ProtectedLocalStorage _localStorage;
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IHttpClientFactory _httpClientFactory; // Used for a dedicated, unhandled refresh client

        // Injection via handler constructor
        public AuthTokenHandler(
            ProtectedLocalStorage localStorage,
            NavigationManager navigationManager,
            AuthenticationStateProvider authStateProvider,
            IHttpClientFactory httpClientFactory)
        {
            _localStorage = localStorage;
            _navigationManager = navigationManager;
            _authStateProvider = authStateProvider;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // 1. Skip token check for the refresh endpoint itself to prevent infinite loops.
            if (request.RequestUri?.AbsolutePath.Contains("/api/auth/refreshtoken", StringComparison.OrdinalIgnoreCase) == true)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            var sessionResult = await _localStorage.GetAsync<LoginResponseModel>("sessionState");
            var sessionState = sessionResult.Success ? sessionResult.Value : null;

            if (sessionState == null || string.IsNullOrEmpty(sessionState.AccessToken))
            {
                // No token, proceed without header (API handles 401 if needed)
                return await base.SendAsync(request, cancellationToken);
            }

            // 2. Token Refresh Check
            if (sessionState.ExpiresInSeconds < DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds())
            {
                // Attempt to refresh the token using a dedicated client that doesn't use this handler.
                var isRefreshed = await TryRefreshTokenAsync(sessionState);

                if (!isRefreshed)
                {
                    // If refresh fails or token is fully expired, log out and redirect.
                    await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
                    _navigationManager.NavigateTo("/login", forceLoad: true);
                    // Return 401 to stop the current request immediately.
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
                // Update session state with the new tokens before proceeding.
                sessionResult = await _localStorage.GetAsync<LoginResponseModel>("sessionState");
                sessionState = sessionResult.Value;
            }

            // 3. Set Authorization Header for the current request.
            if (sessionState != null && !string.IsNullOrEmpty(sessionState.AccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sessionState.AccessToken);
            }

            // Add Culture Headers (optional but included from original code)
            var requestCulture = new RequestCulture(
                System.Globalization.CultureInfo.CurrentCulture,
                System.Globalization.CultureInfo.CurrentUICulture
            );
            var cultureCookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);
            request.Headers.Add("X-Culture-Cookie", cultureCookieValue); // Use a custom header instead of simulating 'Cookie'

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<bool> TryRefreshTokenAsync(LoginResponseModel oldSessionState)
        {
            // Use a dedicated, unhandled client to prevent recursion/infinite loop
            var refreshClient = _httpClientFactory.CreateClient("NoAuthHandlerClient");

            var refreshTokenModel = new RefreshTokenModel
            {
                AccessToken = oldSessionState.AccessToken,
                RefreshToken = oldSessionState.RefreshToken
            };

            try
            {
                var res = await refreshClient.PostAsJsonAsync($"/api/auth/refreshtoken", refreshTokenModel);

                if (res.IsSuccessStatusCode)
                {
                    var newSession = await res.Content.ReadFromJsonAsync<LoginResponseModel>();
                    if (newSession != null)
                    {
                        // Update auth state (and local storage) with the new tokens.
                        await ((CustomAuthStateProvider)_authStateProvider).MarkUserAsAuthenticated(newSession);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                // Log the refresh failure (omitted for brevity)
            }
            return false;
        }
    }
}
