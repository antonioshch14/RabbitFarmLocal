using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using RabbitFarmLocal.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using DataLibrary;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
//using RabbitFarmLocal.Scheduling;
using RabbitFarmLocal.Scheduler;
using RabbitFarmLocal.Start;
using RabbitFarmLocal.messaging;
using RabbitFarmLocal.BusinessLogic;

namespace RabbitFarmLocal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddSingleton<IHostedService, UpdateRabbitStatus>();
            services.AddSingleton<IHostedService, SendMessage>();
            services.AddSingleton<IHostedService, FinReportForMonth>();
          
            //services.AddServerSideBlazor();
            //services.AddSingleton<Settings>();
            Settings.GetSettings();
            WeighGrow.GetWeightGrow();
            //RabWeightCurve.GetRabWeightCurve();
            MyTelegram.GetTelegram();
            // services.AddHostedService<TimedHostedService>();// scheduling test
            // services.AddSingleton<IDataAccess, DataAccess>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            // DateTime
            //var supportedCultures = new[]
            //    {
            //       new CultureInfo("en-US"),

            //    };

            //app.UseRequestLocalization(new RequestLocalizationOptions
            //{
            //    DefaultRequestCulture = new RequestCulture("en-US"), //en-GB ru-RU
            //    SupportedCultures = supportedCultures,
            //    SupportedUICultures = supportedCultures
            //}); 
            // end DateTime
            app.UseStaticFiles();
            //Routing.Include(app);

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                //endpoints.MapRazorPages();
                //endpoints.MapBlazorHub();
            });
        }
    }
}
