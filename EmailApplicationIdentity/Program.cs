using EmailApplicationIdentity.Context;
using EmailApplicationIdentity.Entities;
using EmailApplicationIdentity.Models.JwtModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EmailApplicationIdentity;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<EmailContext>();

        builder.Services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<EmailContext>()
            //.AddErrorDescriber<CustomIdentityValidator>() // tükçe diliyle validator uygulamak.
            .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);

        // Configure JWT authentication
        builder.Services.Configure<JwtSettingsModel>(builder.Configuration.GetSection("JwtSettingsKey")); // Bind the JwtSettingsModel to the configuration section "JwtSettings

        builder.Services.AddAuthentication(option =>
        {
            option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.LoginPath = "/Login/UserLogin"; // Redirect to login page if not authenticated
            options.AccessDeniedPath = "/Error/403"; // Redirect to access denied 403 page if authorization fails
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
        {
            var jwtSettings = builder.Configuration.GetSection("JwtSettingsKey").Get<JwtSettingsModel>(); // Retrieve JWT settings from configuration -> appsettings.json
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            };
        })
        // Google authentication Configuration
        .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
        {
            options.ClientId = "68887452084-02rsdrmhomm1t4vto5lv24efbt1fftus.apps.googleusercontent.com";
            options.ClientSecret = "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";
            options.CallbackPath = "/signin-google";
        });

        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        app.UseStatusCodePagesWithReExecute("/Error/{0}");

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication(); // Enable authentication middleware
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Login}/{action=UserLogin}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
