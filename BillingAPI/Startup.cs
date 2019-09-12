using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            //var connectionString = @"Server = 192.168.201.119; Database = HangFire; persist security info = True; User Id = vivchenkov; Password = q8URs3bC; multipleactiveresultsets = True;";
            //GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);
            //services.AddHangfire(config =>
            //{
            //    config.UseSqlServerStorage(connectionString);
            //});
            //services.AddHangfire(config => config.UsePostgreSqlStorage(Configuration.GetConnectionString("HangfireConnectionPostGre")));
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
            //app.UseHangfireServer();
            //app.UseHangfireDashboard();
            #endregion

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
