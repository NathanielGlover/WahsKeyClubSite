using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WahsKeyClubSite.Areas.Identity.Data;

[assembly: HostingStartup(typeof(WahsKeyClubSite.Areas.Identity.IdentityHostingStartup))]
namespace WahsKeyClubSite.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {            
            builder.ConfigureServices((context, services) => 
            {
                services.AddTransient<IEmailSender, GoogleEmailSender>();
                services.AddSingleton<IEmailSender, GoogleEmailSender>();
                services.AddScoped<IEmailSender, GoogleEmailSender>();
                
                string url = context.Configuration["DATABASE_URL"];

                if(url == null) //If run in local environment
                {
                    var p = new Process {StartInfo = {UseShellExecute = false, RedirectStandardOutput = true, FileName = "/bin/bash"}};
                    p.StartInfo.Arguments = "heroku config:get -a wahskeyclub DATABASE_URL";
                    p.Start();
                    url = p.StandardOutput.ReadToEnd();
                    Console.WriteLine(url);
                    p.WaitForExit();
                }

                var stringBuilder = new PostgreSqlConnectionStringBuilder(url)
                {
                    Pooling = true,
                    TrustServerCertificate = true,
                    SslMode = SslMode.Require
                };            

                services.AddEntityFrameworkNpgsql().AddDbContext<UserDbContext>(options => options.UseNpgsql(stringBuilder.ConnectionString));

                services.AddDefaultIdentity<User>().AddEntityFrameworkStores<UserDbContext>();

                services.Configure<IdentityOptions>(options =>
                {                    
                    // Password settings
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 5;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                    options.Lockout.MaxFailedAccessAttempts = 10;
                    options.Lockout.AllowedForNewUsers = true;

                    // User settings
                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters += " ";
                });

                services.ConfigureApplicationCookie(options =>
                {
                    // Cookie settings
                    options.Cookie.HttpOnly = false;
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);
                    // If the LoginPath isn't set, ASP.NET Core defaults 
                    // the path to /Account/Login.
                    options.LoginPath = "/Account/Login";
                    // If the AccessDeniedPath isn't set, ASP.NET Core defaults 
                    // the path to /Account/AccessDenied.
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.SlidingExpiration = true;
                });
            });
        }
    }
}