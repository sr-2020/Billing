using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingAPI.Extensions;
using BillingAPI.Filters;
using Core;
using IoC.Init;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PubSubService;

namespace BillingAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            IocInitializer.Init();
            SystemHelper.Init(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(new HttpResponseExceptionFilter());
                options.Filters.Add(new EvarunAuthorizationFilter());
            })
            .AddNewtonsoftJson()
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            #region swagger
            services.AddEvaSwaggerGen();
            #endregion

            #region hangfire
            //var hfConnectionString = SystemHelper.GetConnectionString("Hangfire");
            //services.AddHangfire(config => config.UsePostgreSqlStorage(hfConnectionString));
            #endregion

            #region cors
            services.AddCors(options =>
            {
                options.AddPolicy("FullAccessPolicy", builder =>
                {
                    builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(origin => true) // allow any origin
                        .AllowCredentials(); // allow credentials
                });
            });

            #endregion

            #region pubsub

            var replica = Environment.GetEnvironmentVariable(SystemHelper.Replica);
            if (replica != "true")
            {

                services.AddSingleton<IPubSubFoodService, PubSubFoodService>();
                services.AddSingleton<IPubSubHealthService, PubSubHealthService>();
                services.AddSingleton<IPubSubAbilityService, PubSubAbilityService>();
                services.AddSingleton<IPubSubDampshockService, PubSubDampshockService>();
                services.AddSingleton<IPubSubImplantInstallService, PubSubImplantInstallService>();
                services.AddSingleton<IPubSubPillConsumptionService, PubSubPillConsumptionService>();
                services.AddHostedService<PubSubSubscriber>();
            }

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Console.WriteLine("configuring");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            #region hangfire
            //app.UseHangfireServer();
            //app.UseHangfireDashboard("/hangfire", new DashboardOptions
            //{
            //    Authorization = new[] { new HangfireAuthorizationFilter() }
            //});
            #endregion

            #region swagger
            app.UseEvaSwagger();
            #endregion

            #region cors
            app.UseCors("FullAccessPolicy");
            app.UseCookiePolicy(
                new CookiePolicyOptions()
                {
                    MinimumSameSitePolicy = SameSiteMode.None,
                    Secure = CookieSecurePolicy.Always,
                    HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.None
                }
            );

            #endregion

            app.UseRouting();
            app.UseEndpoints(options =>
            {
                options.MapControllers();
            });
            //app.UseMvc(routes =>
            //{
            //    routes
            //        .MapRoute(name: "default", template: "{controller}/{action=Index}/");
            //}).UseStaticFiles(); 
        }
    }
}
