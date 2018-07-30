using System;
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
            
            var optionsBuilder = new DbContextOptionsBuilder<UserContext>();
            optionsBuilder.UseSqlite("Data Source=http://s3.amazonaws.com/wahskeyclubsite/Users.db");
            using(var context = new UserContext(optionsBuilder.Options))
            {
                try
                {
                    context.Database.OpenConnection();
                }
                catch(Exception e)
                {
                    services.AddDbContext<UserContext>(options => options.UseSqlite("Data Source=Users.db"));
                    return;
                }
                
                services.AddDbContext<UserContext>(options => options.UseSqlite("Data Source=http://s3.amazonaws.com/wahskeyclubsite/Users.db"));
            }
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