//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
//using SMS.WebApp.Models;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;

//namespace SMS.WebApp.Authentication
//{
//    public class CustomAuthStateProvider(IHttpContextAccessor httpContextAccessor, ProtectedLocalStorage localStorage) : AuthenticationStateProvider
//    {
//        private const string TokenCookieName = "AuthToken";

//        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//        {
//            // --- 1. Attempt to read from the COOKIE (available on initial HTTP request) ---
//            var token = httpContextAccessor.HttpContext?.Request.Cookies[TokenCookieName];

//            // --- 2. Fallback to ProtectedLocalStorage (for client-side SPA-like use) ---
//            // If running in Blazor Server, ProtectedLocalStorage is still used for setting/deleting

//            // IF the circuit is active.
//            if (string.IsNullOrEmpty(token))
//            {
//                // Use try/catch as ProtectedBrowserStorage calls may fail during pre-rendering
//                try
//                {
//                    var result = await localStorage.GetAsync<LoginResponseModel>("sessionState");
//                    if (result.Success)
//                    {
//                        token = result.Value?.AccessToken;
//                    }
//                }
//                catch { /* Ignore exception if storage access fails during initial render */ }
//            }

//            var identity = string.IsNullOrEmpty(token) ? new ClaimsIdentity() : GetClaimsIdentity(token);
//            var user = new ClaimsPrincipal(identity);
//            return new AuthenticationState(user);
//        }

//        public async Task MarkUserAsAuthenticated(LoginResponseModel model)
//        {
//            // --- Set the JWT as a cookie ---
//            var cookieOptions = new CookieOptions
//            {
//                HttpOnly = true,
//                Secure = true,
//                IsEssential = true,                
//                Expires = DateTimeOffset.UtcNow.AddSeconds(model.ExpiresInSeconds)
//            };

//            httpContextAccessor.HttpContext?.Response.Cookies.Append(TokenCookieName, model.AccessToken, cookieOptions);

//            // *** RE-ADD: Set the token in ProtectedLocalStorage for client-side API handler ***
//            await localStorage.SetAsync("sessionState", model);

//            // --- Your existing logic (for client-side state update) ---
//            var identity = GetClaimsIdentity(model.AccessToken);
//            var user = new ClaimsPrincipal(identity);
//            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
//        }

//        public async Task MarkUserAsLoggedOut()
//        {
//            // --- Delete the cookie ---
//            httpContextAccessor.HttpContext?.Response.Cookies.Delete(TokenCookieName);

//            // *** RE-ADD: Delete the token from ProtectedLocalStorage for cleanup ***
//            await localStorage.DeleteAsync("sessionState");

//            // --- Your existing logic (for client-side state update) ---
//            var identity = new ClaimsIdentity();
//            var user = new ClaimsPrincipal(identity);
//            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
//        }

//        private static ClaimsIdentity GetClaimsIdentity(string token)
//        {
//            var handler = new JwtSecurityTokenHandler();
//            var jwtToken = handler.ReadJwtToken(token);
//            var claims = jwtToken.Claims;
//            return new ClaimsIdentity(claims, "jwt");
//        }
//    }
//}
