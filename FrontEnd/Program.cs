using Microsoft.AspNetCore.Localization;
using Playground.FrontEnd;
using Playground.FrontEnd.Components.Dialog;
using Playground.Services;
using System.Globalization;
using Playground.Templating.Email;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddAuthorizationCore();

//ToastService
builder.Services.AddSingleton<ToastService>();

//DialogService
builder.Services.AddScoped<IDialogService, DialogService>();

builder.Services.AddScoped<Mail>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Define supported languages
var supportedCultures = new[]
{
    new CultureInfo("nl"),
    new CultureInfo("en"),
};

app.UseRequestLocalization(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.FallBackToParentCultures = true;
    options.FallBackToParentUICultures = true;
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new UserAccountCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider(),
    };
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseStatusCodePagesWithRedirects("/StatusCode/{0}");

app.Run();
