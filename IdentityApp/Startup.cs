using IdentityApp.Abstractions;
using IdentityApp.Infrastructure.Database;
using IdentityApp.Infrastructure.Helpers.Responses;
using IdentityApp.Infrastructure.Middleware;
using IdentityApp.Infrastructure.Options;
using IdentityApp.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using IdentityApp.Infrastructure.Helpers.Extensions;

namespace IdentityApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment CurrentEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = CurrentEnvironment.IsDevelopment()
                ? Configuration.GetConnectionString("DefaultConnection")
                : HerokuConfiguration.GetHerokuConnectionString();

            services.AddControllers();

            var jwtOptions = new JwtOptions();
            var configSection = Configuration.GetSection("JwtOptions");

            configSection.Bind(jwtOptions);

            services.Configure<JwtOptions>(configSection);

            // Custom BadRequest response on ModelState validation
            services.Configure<ApiBehaviorOptions>(o =>
            {
                o.CustomInvalidModelStateResponse();
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = false,
                        //ValidAudience = authOptions.Audience,
                        ValidateLifetime = true,
                        IssuerSigningKey = jwtOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                    };
                });

            services.AddDbContext<ApplicationDbContext>(options =>
                { options.UseNpgsql(connectionString); });

            services.AddIdentityCore<IdentityUser>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<IJwtGenerator, JwtGenerator>();

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddCors();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Identity CQRS API",
                    Description = "A simple Asp.Net Core web API which uses CQRS MediatR pattern to handle requests " +
                                  "and Identity to communicate with database. Used jwt authentication."
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IOptionsMonitor<JwtOptions> jwtOptionsMonitor)
        {
            if (CurrentEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
                Secure = CookieSecurePolicy.Always,
                OnAppendCookie = (context =>
                {
                    if (context.CookieName == ".AspNetCore.Application.Id")
                    {
                        context.CookieOptions.Expires = new DateTimeOffset(DateTime.Now.AddMinutes(jwtOptionsMonitor.CurrentValue.Lifetime));
                        context.CookieOptions.HttpOnly = true;
                    }
                })
                //HttpOnly = HttpOnlyPolicy.Always,
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<HttpAuthMiddleware>(); // it is crucial to add this middleware before app.UseAuthentication()
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity CQRS app");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
