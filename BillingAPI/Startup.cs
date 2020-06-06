using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingAPI.Extensions;
using BillingAPI.Filters;
using Core;
using Hangfire;
using Hangfire.PostgreSql;
using IoC.Init;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            #region swagger
            services.AddEvaSwaggerGen();
            #endregion

            #region hangfire
            var hfConnectionString = SystemHelper.GetConnectionString("Hangfire");
            services.AddHangfire(config => config.UsePostgreSqlStorage(hfConnectionString));
            #endregion

            #region cors
            services.AddCors(options =>
            {
                options.AddPolicy("FullAccessPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
            #endregion
            //services.AddAuthentication().AddCookie();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });
            #endregion

            #region swagger
            app.UseEvaSwagger();
            #endregion

            #region cors
            app.UseCors("FullAccessPolicy");
            #endregion
            app.UseMvc(routes =>
            {
                routes
                    .MapRoute(name: "default", template: "{controller}/{action=Index}/");
            }).UseStaticFiles(); 
        }
    }
}
