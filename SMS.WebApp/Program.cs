using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SMS.WebApp;
using SMS.WebApp.Authentication;
using SMS.WebApp.Components;
using SMS.WebApp.Services.Toaster;
using System.Text.Json;
using System.Text.Json.Serialization; // Explicitly ensure this is available for AuthTokenHandler/Logout

var builder = WebApplication.CreateBuilder(args);

// --- Component and Interactive Services ---
// Add services to the container.
builder.Services.AddRazorComponents()
    // Enables interactive components using Blazor Server technology.
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<ToastService>();

// --- Security and Authentication Services ---

// Configure Antiforgery protection to mitigate Cross-Site Request Forgery (CSRF).
builder.Services.AddAntiforgery(options =>
{
    // Specifies the header name used by HttpClient (and AuthTokenHandler, if implemented)
    // to send the Antiforgery token back to the server.
    options.HeaderName = "X-XSRF-TOKEN";
});

// Adds the authentication services container required by Identity and other auth mechanisms.
builder.Services.AddAuthentication();

// Enables cascading the AuthenticationState to child components via a CascadingParameter.
builder.Services.AddCascadingAuthenticationState();

// Register the core ProtectedBrowserStorage service for safe client-side data storage (tokens).
builder.Services.AddScoped<ProtectedLocalStorage>();

// Register the custom service responsible for managing the user's login state.
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Register the AuthTokenHandler as Transient. This is a crucial interceptor 
// for attaching tokens to authenticated requests and handling refreshes.
builder.Services.AddTransient<AuthTokenHandler>();


// --- HttpClient Factory Setup ---

// 1. Register a dedicated, UNHANDLED HttpClient named "NoAuthClient".
// This client is used ONLY for unauthenticated endpoints (Login, Register, Logout cleanup), 
// ensuring the token handler logic is safely bypassed when tokens don't exist yet.
builder.Services.AddHttpClient("NoAuthClient", client =>
{
    // Set the base URL for the API calls.
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? string.Empty);
});

// 2. Register the main application service (ApiClient).
// This client is used for all PROTECTED API calls.
builder.Services.AddHttpClient<ApiClient>(client =>
{
    // Set the base URL.
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? string.Empty);
});
// CRITICAL: Attach the AuthTokenHandler as an interceptor. 
// It runs before every request to check, refresh, and attach the Bearer token.
//.AddHttpMessageHandler<AuthTokenHandler>();

//// 3. Register the specific LogoutClient.
//// This is used for the authenticated logout call, bypassing the token check/refresh handler
//// for more control over token management during cleanup. (The token is manually set.)
//builder.Services.AddHttpClient<LogoutClient>(client =>
//{
//    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? string.Empty);
//});

// Optionally configure System.Text.Json options globally for consistency.
builder.Services.Configure<JsonSerializerOptions>(options =>
{
    // Ensures property names in JSON are matched to C# model properties regardless of casing (e.g., userId vs. UserId).
    options.PropertyNameCaseInsensitive = true;
    options.ReferenceHandler = ReferenceHandler.Preserve;
});


// --- Application Pipeline ---

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Global error handling for production environment.
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    // HSTS (HTTP Strict Transport Security) enforces HTTPS on compliant clients.
    app.UseHsts();
}

// Redirects HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Adds the Antiforgery middleware to the pipeline (must run before endpoint routing).
app.UseAntiforgery();

// Enables authentication middleware (checks headers/cookies).
app.UseAuthentication();

// Enables authorization middleware (checks user roles/permissions).
app.UseAuthorization();

// Maps static assets (CSS, JS, images, etc.).
app.MapStaticAssets();

// Maps the root Razor Components, enabling the interactive server rendering mode.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();