using System;
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
using Naheulbook.Core.Factories;
using Naheulbook.Core.Mappers;
using Naheulbook.Core.Services;
using Naheulbook.Core.Utils;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Factories;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Clients.Facebook;
using Naheulbook.Shared.Clients.Google;
using Naheulbook.Shared.Clients.MicrosoftGraph;
using Naheulbook.Shared.Clients.Twitter;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Configurations;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Hubs;
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
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost";
            });
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(4);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddSignalR();
            services.AddMvc()
                .AddJsonOptions(options => { options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; })
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<ValidateUserRequest>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddHttpContextAccessor();
            services.AddAutoMapper(typeof(RequestToEntityProfile).Assembly, typeof(Startup).Assembly);

            services.AddScoped(servicesProvider => servicesProvider.GetService<IHttpContextAccessor>().HttpContext.GetExecutionContext());

            services.AddSingleton<IUnitOfWorkFactory>(new UnitOfWorkFactory(naheulbookDbContextOptionsBuilder.Options));

            services.AddSingleton<ICalendarService, CalendarService>();
            services.AddSingleton<ICharacterService, CharacterService>();
            services.AddSingleton<ICharacterUtil, CharacterUtil>();
            services.AddSingleton<ICharacterModifierUtil, CharacterModifierUtil>();
            services.AddSingleton<ICharacterHistoryUtil, CharacterHistoryUtil>();
            services.AddSingleton<IEventService, EventService>();
            services.AddSingleton<IEffectService, EffectService>();
            services.AddSingleton<IGodService, GodService>();
            services.AddSingleton<IGroupService, GroupService>();
            services.AddSingleton<IGroupUtil, GroupUtil>();
            services.AddSingleton<IGroupHistoryUtil, GroupHistoryUtil>();
            services.AddSingleton<IIconService, IconService>();
            services.AddSingleton<IItemService, ItemService>();
            services.AddSingleton<IItemUtil, ItemUtil>();
            services.AddSingleton<IItemDataUtil, ItemDataUtil>();
            services.AddSingleton<IItemTemplateService, ItemTemplateService>();
            services.AddSingleton<IItemTemplateCategoryService, ItemTemplateCategoryService>();
            services.AddSingleton<IItemTemplateSectionService, ItemTemplateSectionService>();
            services.AddSingleton<IItemTemplateUtil, ItemTemplateUtil>();
            services.AddSingleton<IJobService, JobService>();
            services.AddSingleton<ILocationService, LocationService>();
            services.AddSingleton<ILootService, LootService>();
            services.AddSingleton<IMailService, MailService>();
            services.AddSingleton<IMonsterService, MonsterService>();
            services.AddSingleton<IMonsterTemplateService, MonsterTemplateService>();
            services.AddSingleton<IMonsterTraitService, MonsterTraitService>();
            services.AddSingleton<IMonsterTypeService, MonsterTypeService>();
            services.AddSingleton<IOriginService, OriginService>();
            services.AddSingleton<IPasswordHashingService, PasswordHashingService>();
            services.AddSingleton<ISkillService, SkillService>();
            services.AddSingleton<ISocialMediaUserLinkService, SocialMediaUserLinkService>();
            services.AddSingleton<IStatService, StatService>();
            services.AddSingleton<IUserService, UserService>();

            services.AddSingleton<IActiveStatsModifierUtil, ActiveStatsModifierUtil>();
            services.AddSingleton<IAuthorizationUtil, AuthorizationUtil>();
            services.AddSingleton<IStringCleanupUtil, StringCleanupUtil>();
            services.AddSingleton<IJsonUtil, JsonUtil>();

            services.AddSingleton<ICharacterFactory, CharacterFactory>();
            services.AddSingleton<IItemFactory, ItemFactory>();

            services.AddSingleton<ITimeService, TimeService>();
            services.AddSingleton<IRngUtil, RngUtil>();

            services.AddSingleton<IJwtService, JwtService>();
            services.AddSingleton<IChangeNotifier, ChangeNotifier>();
            services.AddSingleton<IHubGroupUtil, HubGroupUtil>();

            services.AddSingleton<IFacebookClient, FacebookClient>();
            services.AddSingleton<IGoogleClient, GoogleClient>();
            services.AddSingleton<ITwitterClient, TwitterClient>();
            services.AddSingleton<IMicrosoftGraphClient, MicrosoftGraphClient>();
        }

        private void RegisterConfigurations(IServiceCollection services)
        {
            var mailConfiguration = new MailConfiguration();
            _configuration.Bind("Mail", mailConfiguration);
            services.AddSingleton<IMailConfiguration>(mailConfiguration);

            var authenticationConfiguration = new AuthenticationConfiguration();
            _configuration.Bind("Authentication", authenticationConfiguration);
            services.AddSingleton<IAuthenticationConfiguration>(authenticationConfiguration);

            var googleConfiguration = new GoogleConfiguration();
            _configuration.Bind("Authentication:Google", googleConfiguration);
            services.AddSingleton(googleConfiguration);

            var facebookConfiguration = new FacebookConfiguration();
            _configuration.Bind("Authentication:Facebook", facebookConfiguration);
            services.AddSingleton(facebookConfiguration);

            var twitterConfiguration = new TwitterConfiguration();
            _configuration.Bind("Authentication:Twitter", twitterConfiguration);
            services.AddSingleton(twitterConfiguration);

            var microsoftConfiguration = new MicrosoftGraphConfiguration();
            _configuration.Bind("Authentication:MicrosoftGraph", microsoftConfiguration);
            services.AddSingleton(microsoftConfiguration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod());

            app.UseMiddleware<HttpExceptionMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseMiddleware<DevExceptionMiddleware>();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseSession();
            app.UseMiddleware<JwtAuthenticationMiddleware>();
            app.UseSignalR(options => { options.MapHub<ChangeNotifierHub>("/ws/listen"); });

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