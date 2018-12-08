using System.Net;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Naheulbook.Core.Configurations;
using Naheulbook.Core.Services;
using Naheulbook.Core.Utils;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Factories;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Configurations;
using Naheulbook.Web.Filters;
using Naheulbook.Web.Middlewares;
using Naheulbook.Web.Services;
using Newtonsoft.Json;

namespace Naheulbook.Web
{
    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory, IHostingEnvironment environment)
        {
            _loggerFactory = loggerFactory;
            _environment = environment;
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            RegisterConfigurations(services);

            var naheulbookDbContextOptionsBuilder = new DbContextOptionsBuilder<NaheulbookDbContext>()
                .UseLoggerFactory(_loggerFactory)
                .ConfigureWarnings(w => w.Throw(RelationalEventId.QueryClientEvaluationWarning))
                .UseMySql(_configuration.GetConnectionString("DefaultConnection"));
            if (_environment.IsDevelopment())
            {
                naheulbookDbContextOptionsBuilder.EnableSensitiveDataLogging();
            }

            services.AddMvc()
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<Startup>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAutoMapper();

            services.AddSingleton<IUnitOfWorkFactory>(new UnitOfWorkFactory(naheulbookDbContextOptionsBuilder.Options));

            services.AddSingleton<IEffectService, EffectService>();
            services.AddSingleton<IJobService, JobService>();
            services.AddSingleton<IMailService, MailService>();
            services.AddSingleton<IOriginService, OriginService>();
            services.AddSingleton<IPasswordHashingService, PasswordHashingService>();
            services.AddSingleton<ISkillService, SkillService>();
            services.AddSingleton<IUserService, UserService>();

            services.AddSingleton<IAuthorizationUtil, AuthorizationUtil>();

            services.AddSingleton<ITimeService, TimeService>();

            services.AddSingleton<IJwtService, JwtService>();
            services.AddScoped<JwtAuthorizationFilter>();
        }

        private void RegisterConfigurations(IServiceCollection services)
        {
            var mailConfiguration = new MailConfiguration();
            _configuration.Bind("Mail", mailConfiguration);
            services.AddSingleton<IMailConfiguration>(mailConfiguration);

            var authenticationConfiguration = new AuthenticationConfiguration();
            _configuration.Bind("Authentication", authenticationConfiguration);
            services.AddSingleton<IAuthenticationConfiguration>(authenticationConfiguration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            if (env.IsDevelopment())
            {
                app.UseMiddleware<DevExceptionMiddleware>();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseMvc();
            app.Run(async (context) =>
            {
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new {Message = "Invalid route"}));
            });
        }
    }
}