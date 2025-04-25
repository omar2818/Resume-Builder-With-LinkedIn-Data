using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using ResumeBuilder.Services;

namespace ResumeBuilder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<LinkedInService>();
            builder.Services.AddScoped<ResumeService>();
            builder.Services.AddScoped<PdfService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "LinkedIn";
            })
            .AddCookie()
            .AddOAuth("LinkedIn", options =>
            {
                options.ClientId = "77ddftugrqxo68";
                options.ClientSecret = "WPL_AP1.vhpnT4ZzEfPFRgdt.I9/UEg==";
                options.CallbackPath = "/signin-linkedin";

                options.AuthorizationEndpoint = "https://www.linkedin.com/oauth/v2/authorization";
                options.TokenEndpoint = "https://www.linkedin.com/oauth/v2/accessToken";
                options.UserInformationEndpoint = "https://api.linkedin.com/v2/me";

                options.Scope.Add("r_liteprofile");
                options.Scope.Add("r_emailaddress");

                options.SaveTokens = true;

                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                options.ClaimActions.MapJsonSubKey(ClaimTypes.Name, "localizedFirstName", "localizedLastName");
            });
            

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
