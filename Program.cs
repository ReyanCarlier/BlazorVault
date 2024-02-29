using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.EntityFrameworkCore;
using BlazorVault.Data;
using MudBlazor.Services;
using BlazorVault.Hubs;
using Microsoft.AspNetCore.ResponseCompression;

namespace BlazorVault
{
    public class Program
    {
        public static string LastUpdate { get; } = "23/01/2024";
        public static string Version { get; } = "b0.1.4";
        public static string AdminPassword { get; private set; } = "";

        private static readonly string[] MimeType = ["application/octet-stream"];

        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ');

            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
                    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                        .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
                        .AddInMemoryTokenCaches();
            builder.Services.AddControllersWithViews()
                .AddMicrosoftIdentityUI();            

            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = options.DefaultPolicy;
            });
            AdminPassword = builder.Configuration["AdminPassword"]??"";
            if (string.IsNullOrEmpty(AdminPassword))
            {
                throw new Exception("AdminPassword is not set in appsettings.json");
            }
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();
            builder.Services.AddDbContextFactory<VaultDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("SQLite")));
            builder.Services.AddScoped(typeof(ClipboardService));
            builder.Services.AddScoped(typeof(SessionService));
            builder.Services.AddScoped(typeof(DatabaseHubService));
            builder.Services.AddScoped<Func<VaultService>>(serviceProvider =>
            {
                return () =>
                {
                    var dbContext = serviceProvider.GetRequiredService<VaultDbContext>();
                    return new VaultService(dbContext);
                };
            });


            builder.Services.AddMudServices();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            builder.Services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(MimeType);
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseResponseCompression(); // We use response compression to reduce the size of the response body for faster loading times
            app.UseHttpsRedirection(); // We use HTTPS redirection to redirect HTTP requests to HTTPS
            app.UseSession(); // We use session to store the user's session

            // TFA Middleware is disabled since we removed the TFA feature (for now).
            // app.UseMiddleware<TwoFactorAuthenticationMiddleware>();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapControllers();
            app.MapHub<DatabaseHub>("/databasehub");
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}