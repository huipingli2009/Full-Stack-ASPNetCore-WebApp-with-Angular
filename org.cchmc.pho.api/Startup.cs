using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using org.cchmc.pho.api.Mappings;
using org.cchmc.pho.core.DataAccessLayer;
using org.cchmc.pho.core.Interfaces;
using org.cchmc.pho.core.Models;
using org.cchmc.pho.core.Settings;
using org.cchmc.pho.identity.Extensions;
namespace org.cchmc.pho.api
{
    [ExcludeFromCodeCoverage]
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
            services.AddIdentityServices(Configuration);
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
            services.AddOptions<ConnectionStrings>()
                        .Bind(Configuration.GetSection("ConnectionStrings"))
                        .ValidateDataAnnotations() //todo 
                        .Validate(c =>
                        {
                            return true;
                        }, "failure message");

            services.AddMvc(config =>
                {
                    var policy = Configuration.BuildAuthorizationPolicy();

                    config.Filters.Add(new AuthorizeFilter(policy));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddControllersAsServices();
            services.Configure<ConnectionStrings>(options => Configuration.GetSection("ConnectionStrings").Bind(options));

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

            app.UseAuthentication();
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
                //NOTE: The line below will load ALL the mappings in that assembly, not just the Alert one. 
                //So there's no need to repeat this line for every mapping, since they're all compiled into the same assembly.
                cfg.AddMaps(Assembly.GetAssembly(typeof(AlertMappings)));
                //Chris suggested that if we want to be explicit about what we're loading, we can use the AddProfile method, examples below.
                //cfg.AddProfile<AlertMappings>();
                //cfg.AddProfile<MetricMappings>();
            });
            config.CreateMapper();
        }
    }
}
