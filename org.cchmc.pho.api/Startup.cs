using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using org.cchmc.pho.api.Mappings;
using org.cchmc.pho.core.DataAccessLayer;
using org.cchmc.pho.core.DataModels;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.Models;
using org.cchmc.pho.core.Settings;

namespace org.cchmc.pho.api
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
            services.AddAutoMapper(typeof(Startup));

            // TODO: Load AppSettings, LDAP, DBContext, authentication
            services.AddOptions<CustomOptions>()
                        .Bind(Configuration.GetSection(CustomOptions.SECTION_KEY))
                        //https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.optionsbuilderdataannotationsextensions.validatedataannotations?view=dotnet-plat-ext-3.1
                        .ValidateDataAnnotations() //todo 
                        .Validate(c =>
                        {
                            //NOTE: can add additional validation
                            //https://www.stevejgordon.co.uk/asp-net-core-2-2-options-validation

                            //if (c.Api_Key != default)
                            //{
                            //    return c.Endpoint_BaseUrl != default;
                            //}
                            return true;
                        }, "failure message");

            var connStr = Configuration.GetConnectionString("pho-db");

            services.Configure<ConnectionStrings>(options => Configuration.GetSection("ConnectionStrings").Bind(options));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "PHO API", Version = "v1" });
            });


            //NOTE: register service    
            services.AddTransient<IAlert, AlertDAL>();
            services.AddTransient<IMetric, MetricDAL>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                logger.LogInformation($"Environment is Development");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "PHO API v1");
            });

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
                cfg.AddMaps(Assembly.GetAssembly(typeof(AlertMappings)));
                cfg.AddMaps(Assembly.GetAssembly(typeof(MetricMappings)));
            });
            config.CreateMapper();
        }
    }
}
