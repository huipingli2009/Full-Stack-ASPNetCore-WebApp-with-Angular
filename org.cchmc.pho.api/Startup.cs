using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private const string PHO_WebsitePolicy = "PHO_WebsitePolicy";
        public IConfiguration Configuration { get; }

        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

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
                        }, "Failed to validate custom options.");
            services.AddOptions<ConnectionStrings>()
                        .Bind(Configuration.GetSection("ConnectionStrings"))
                        .ValidateDataAnnotations()
                        .Validate(c =>
                        {
                            return true;
                        }, "Failed to validate connection strings.");

            //setting up CORS policy only in the development environment
            if (_environment.IsDevelopment())
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(PHO_WebsitePolicy,
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200")
                                     .AllowAnyHeader()
                                     .AllowAnyMethod();
                    });
                });
            }

            services.AddMvc(config =>
                {
                    // CJENKINSON - Uncomment out when ready to apply Authorize attributes
                    //var policy = Configuration.BuildAuthorizationPolicy();
                    //config.Filters.Add(new AuthorizeFilter(policy));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddControllersAsServices();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "PHO API", Version = "v1" });
            });

            //NOTE: register service
            services.AddTransient<IAlert, AlertDAL>();
            services.AddTransient<IContent, ContentDAL>();
            services.AddTransient<IMetric, MetricDAL>();
            services.AddTransient<IPatient, PatientDAL>();
            services.AddTransient<IStaff, StaffDAL>();
            services.AddTransient<IWorkbooks, WorkbooksDal>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            if (_environment.IsDevelopment())
            {
                //setting up CORS policy only in the development environment
                app.UseCors(PHO_WebsitePolicy);
                logger.LogInformation($"Environment is Development");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            //NOTE: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-3.1 needed to add this package : Microsoft.AspNetCore.App metapackage
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            // CJENKINSON - Uncomment out when ready to apply Authorize attributes
            //app.UseAuthentication();
            //app.UseAuthorization();

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
                //if we want to be explicit about what we're loading, we can use the AddProfile method, examples below.
                //cfg.AddProfile<AlertMappings>();
                //cfg.AddProfile<MetricMappings>();
            });
            config.CreateMapper();


            app.Run(async (context) =>
            {
                //wanting to disable caching... on index.html
                // -- need to dig more into that this only does it for the index.html not the assets
                context.Response.ContentType = "text/html";
                context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                context.Response.Headers.Add("Expires", "-1");
                await context.Response.SendFileAsync(Path.Combine(_environment.WebRootPath, "index.html"));
            });

        }
    }
}
