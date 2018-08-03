using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WahsKeyClubSite.Models;

namespace WahsKeyClubSite
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            string url = Configuration["DATABASE_URL"];

            if(url == null) //If run in local environment
            {
                var p = new Process {StartInfo = {UseShellExecute = false, RedirectStandardOutput = true, FileName = "/bin/bash"}};
                p.StartInfo.Arguments = "heroku config:get -a wahskeyclub DATABASE_URL";
                p.Start();
                url = p.StandardOutput.ReadToEnd();
                Console.WriteLine(url);
                p.WaitForExit();
            }

            var builder = new PostgreSqlConnectionStringBuilder(url)
            {
                Pooling = true,
                TrustServerCertificate = true,
                SslMode = SslMode.Require
            };            

            services.AddEntityFrameworkNpgsql().AddDbContext<UserContext>(options => options.UseNpgsql(builder.ConnectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}