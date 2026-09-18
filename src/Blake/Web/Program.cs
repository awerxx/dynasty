using System.Globalization;

using Dynasty.Carrington.Blake.Application.Abstractions;
using Dynasty.Carrington.Blake.Infrastructure.Database;
using Dynasty.Carrington.Blake.Web.Components;
using Dynasty.Carrington.Blake.Web.Components.Account;
using Dynasty.Carrington.Blake.Web.Data;
using Dynasty.Carrington.Blake.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

const string cultureName = "pl-PL";

CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                          throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddBlakeApplication();
builder.Services.AddBlakeDatabase();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<ScopedCurrentUser>();
builder.Services.AddScoped<ICurrentUser>(provider => provider.GetRequiredService<ScopedCurrentUser>());
builder.Services.AddScoped<HandlerDispatcher>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// One culture for everyone: the browser's Accept-Language must not change how amounts are shown.
app.UseRequestLocalization(options =>
{
    options.SetDefaultCulture(cultureName)
        .AddSupportedCultures(cultureName)
        .AddSupportedUICultures(cultureName);
    options.RequestCultureProviders.Clear();
});

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

await app.Services.InitializeBlakeDatabaseAsync();

await app.RunAsync();
