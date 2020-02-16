using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BillingAPI.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddEvaSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Eva BillingAPI", Version = "v1" });
                // XML Documentation
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.OrderActionsBy((apiDesc) => apiDesc.RelativePath);
            }
            );

            return services;
        }
        public static IApplicationBuilder UseEvaSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger/v1";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Evarun BillingAPI");
                c.DefaultModelsExpandDepth(-1);
            });

            return app;
        }
    }
}
