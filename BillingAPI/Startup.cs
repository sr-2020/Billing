using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BillingAPI.Filters;
using Hangfire;
using Hangfire.PostgreSql;
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
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            #region hangfire
            var user = Environment.GetEnvironmentVariable("POSTGRESQL_USER");
            var password = Environment.GetEnvironmentVariable("POSTGRESQL_PASSWORD");
            var connectionString = $"{Configuration.GetConnectionString("HangFirePsgConnectionString")} User Id = {user}; Password = {password}";
            services.AddHangfire(config => config.UsePostgreSqlStorage(connectionString));
            #endregion

            services.AddAuthentication().AddCookie();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
