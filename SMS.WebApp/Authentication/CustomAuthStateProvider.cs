using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SMS.WebApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SMS.WebApp.Authentication
{
    public class CustomAuthStateProvider(ProtectedLocalStorage localStorage) : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = (await localStorage.GetAsync<LoginResponseModel>("sessionState")).Value?.AccessToken;
            var identity = string.IsNullOrEmpty(token) ? new ClaimsIdentity() : GetClaimsIdentity(token);
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public async Task MarkUserAsAuthenticated(LoginResponseModel model)
        {
            await localStorage.SetAsync("sessionState", model);
            var identity = GetClaimsIdentity(model.AccessToken);
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private static ClaimsIdentity GetClaimsIdentity(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claims = jwtToken.Claims;
            return new ClaimsIdentity(claims, "jwt");
        }

        public async Task MarkUserAsLoggedOut()
        {
            await localStorage.DeleteAsync("sessionState");
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}
