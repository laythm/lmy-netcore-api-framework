using Api.Code;
using Common.Extensions;
using Common.Interfaces;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using Services.Interfaces;
using Services.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich
                .FromLogContext()
                .MinimumLevel
                .Error()
                .WriteTo.File(Path.Combine(configuration["AppSettings:LogsFolder"], "log.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("customAllowSpecificOrigins", builder =>
            {
                builder.WithOrigins(Configuration["AppSettings:AllowOrigins"].Split(','))
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services.AddControllers(options =>
            {
                var noContentFormatter = options.OutputFormatters.OfType<HttpNoContentOutputFormatter>().FirstOrDefault();
                if (noContentFormatter != null)
                {
                    noContentFormatter.TreatNullValueAsNoContent = false;
                }
            });//.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddDbContext<Infrastructure.LmyFrameworkDBContext>(
                options =>
                 options.UseSqlServer(Configuration["ConnectionStrings:SQLServer"])
            );

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                  .AddJwtBearer(options =>
                  {
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuer = true,
                          ValidateAudience = true,
                          ValidateLifetime = true,
                          ValidateIssuerSigningKey = true,
                          ValidIssuer = Configuration["AppSettings:TokenIssuer"],
                          ValidAudience = Configuration["AppSettings:TokenIssuer"],
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["AppSettings:TokenSecret"]))
                      };
                  });

            //services.AddMemoryCache();

            //configure static classes 
            DateExtension.Configure(Configuration["AppSettings:DateTimeFormat"], Configuration["AppSettings:DateFormat"], Configuration["AppSettings:TimeFormat"]);

            services.AddHttpContextAccessor();
            services.AddScoped<IRequestContext, RequestContext>();

            ////db context 
            services.AddScoped<DbContext, LmyFrameworkDBContext>();

            ////Repositories
            services.AddScoped<IGenericRepository<Users>, GenericRepository<Users, LmyFrameworkDBContext>>();
            services.AddScoped<IGenericRepository<Roles>, GenericRepository<Roles, LmyFrameworkDBContext>>();
            services.AddScoped<IGenericRepository<UserRoles>, GenericRepository<UserRoles, LmyFrameworkDBContext>>();
            services.AddScoped<IGenericRepository<Projects>, GenericRepository<Projects, LmyFrameworkDBContext>>();
            services.AddScoped<IGenericRepository<ClientErrors>, GenericRepository<ClientErrors, LmyFrameworkDBContext>>();

            services.AddScoped<IGenericUnitOfwork<LmyFrameworkDBContext>, GenericUnitOfWork<LmyFrameworkDBContext>>();

            ////services
            services.AddScoped<ICommonService, CommonServices>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IProjectsService, ProjectsService>();

            //for testing
            if (Configuration["AppSettings:EnableSwagger"] == "true")
            {
                services.AddSwaggerGen(c =>
                {
                    c.CustomSchemaIds(x => x.FullName);

                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "lmy API", Version = "v1" });
                    //    c.AddSecurityDefinition("Bearer",
                    //        new ApiKeyScheme
                    //        {
                    //            In = "header",
                    //            Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    //            Name = "Authorization",
                    //            Type = "apiKey"
                    //        });
                    //    c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    //{ "Bearer", Enumerable.Empty<string>() },
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                        " Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    },
                                    Scheme = "oauth2",
                                    Name = "Bearer",
                                    In = ParameterLocation.Header,
                                },
                                new List<string>()
                            }
                        });
                    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                });
            }

            //services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
            //{
            //    options.ValueCountLimit = 10; //default 1024
            //    options.ValueLengthLimit = int.MaxValue; //not recommended value
            //    options.MultipartBodyLengthLimit = long.MaxValue; //not recommended value
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerfactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (Configuration["AppSettings:EnableSwagger"] == "true")
            {
                //https://localhost:44357/swagger/index.html
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("v1/swagger.json", "My API V1"); //originally "./swagger/v1/swagger.json"
                });

            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("customAllowSpecificOrigins");

            app.UseAuthorization();

            loggerfactory.AddSerilog();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
