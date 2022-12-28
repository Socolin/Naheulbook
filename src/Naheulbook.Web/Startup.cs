using System;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Naheulbook.Core.Actions.Executor;
using Naheulbook.Core.Clients;
using Naheulbook.Core.Configurations;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Mappers;
using Naheulbook.Core.Notifications;
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
using Naheulbook.Web.Notifications;
using Naheulbook.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Naheulbook.Web;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _environment = environment;
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var redisConnectionString = _configuration.GetConnectionString("Redis");
        RegisterConfigurations(services);

        var naheulbookDbContextOptionsBuilder = new DbContextOptionsBuilder<NaheulbookDbContext>()
            .UseMySql(_configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(_configuration.GetConnectionString("DefaultConnection")), builder => builder.EnableRetryOnFailure());
        if (_environment.IsDevelopment())
        {
            naheulbookDbContextOptionsBuilder.EnableSensitiveDataLogging();
        }

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(4);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddSignalR();
        services.AddMvc()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                };
            });
        services.AddValidatorsFromAssemblyContaining<ValidateUserRequest>();

        services.AddHttpContextAccessor();
        services.AddHealthChecks()
            .AddMySql(_configuration.GetConnectionString("DefaultConnection"));

        if (redisConnectionString != null)
        {
            services.AddHealthChecks().AddRedis(redisConnectionString);
            services.AddStackExchangeRedisCache(options => { options.Configuration = redisConnectionString; });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddAutoMapper(typeof(RequestToEntityProfile).Assembly, typeof(Startup).Assembly);

        services.AddScoped(servicesProvider => servicesProvider.GetRequiredService<IHttpContextAccessor>().HttpContext!.GetExecutionContext());
        services.AddScoped(servicesProvider => servicesProvider.GetRequiredService<IHttpContextAccessor>().HttpContext!.GetIfExistsExecutionContext());

        services.AddSingleton<IUnitOfWorkFactory>(new UnitOfWorkFactory(naheulbookDbContextOptionsBuilder.Options));

        services.AddSingleton<IActionsUtil, ActionsUtil>();
        services.AddSingleton<IAddItemExecutor, AddItemExecutor>();
        services.AddSingleton<IRemoveItemExecutor, RemoveItemExecutor>();
        services.AddSingleton<IAddCustomModifierExecutor, AddCustomModifierExecutor>();
        services.AddSingleton<IAddEffectExecutor, AddEffectExecutor>();
        services.AddSingleton<IAddEvExecutor, AddEvExecutor>();
        services.AddSingleton<IAddEaExecutor, AddEaExecutor>();
        services.AddSingleton<ICalendarService, CalendarService>();
        services.AddSingleton<ICharacterRandomNameService, CharacterRandomNameService>();
        services.AddSingleton<ICharacterService, CharacterService>();
        services.AddSingleton<ICharacterBackupService, CharacterBackupService>();
        services.AddSingleton<ICharacterUtil, CharacterUtil>();
        services.AddSingleton<ICharacterModifierUtil, CharacterModifierUtil>();
        services.AddSingleton<ICharacterHistoryUtil, CharacterHistoryUtil>();
        services.AddSingleton<IDurationUtil, DurationUtil>();
        services.AddSingleton<IEventService, EventService>();
        services.AddSingleton<IEffectService, EffectService>();
        services.AddSingleton<IGodService, GodService>();
        services.AddSingleton<IGroupConfigUtil, GroupConfigUtil>();
        services.AddSingleton<IGroupHistoryUtil, GroupHistoryUtil>();
        services.AddSingleton<IGroupService, GroupService>();
        services.AddSingleton<IGroupUtil, GroupUtil>();
        services.AddSingleton<IItemService, ItemService>();
        services.AddSingleton<IItemUtil, ItemUtil>();
        services.AddSingleton<IItemDataUtil, ItemDataUtil>();
        services.AddSingleton<IItemTemplateService, ItemTemplateService>();
        services.AddSingleton<IItemTemplateSubCategoryService, ItemTemplateSubCategoryService>();
        services.AddSingleton<IItemTemplateSectionService, ItemTemplateSectionService>();
        services.AddSingleton<IItemTemplateUtil, ItemTemplateUtil>();
        services.AddSingleton<IItemTypeService, ItemTypeService>();
        services.AddSingleton<IJobService, JobService>();
        services.AddSingleton<ILootService, LootService>();
        services.AddSingleton<IMapService, MapService>();
        services.AddSingleton<IMailService, MailService>();
        services.AddSingleton<IMonsterService, MonsterService>();
        services.AddSingleton<IMonsterTemplateService, MonsterTemplateService>();
        services.AddSingleton<IMonsterTraitService, MonsterTraitService>();
        services.AddSingleton<IMonsterTypeService, MonsterTypeService>();
        services.AddSingleton<INpcService, NpcService>();
        services.AddSingleton<IOriginService, OriginService>();
        services.AddSingleton<IOriginUtil, OriginUtil>();
        services.AddSingleton<IPasswordHashingService, PasswordHashingService>();
        services.AddSingleton<ISkillService, SkillService>();
        services.AddSingleton<ISocialMediaUserLinkService, SocialMediaUserLinkService>();
        services.AddSingleton<IStatService, StatService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IUserAccessTokenService, UserAccessTokenService>();

        services.AddSingleton<IActiveStatsModifierUtil, ActiveStatsModifierUtil>();
        services.AddSingleton<IAuthorizationUtil, AuthorizationUtil>();
        services.AddSingleton<IStringCleanupUtil, StringCleanupUtil>();
        services.AddSingleton<IJsonUtil, JsonUtil>();

        services.AddSingleton<IMapImageUtil, MapImageUtil>();

        services.AddSingleton<ICharacterFactory, CharacterFactory>();
        services.AddSingleton<IItemFactory, ItemFactory>();

        services.AddSingleton<ITimeService, TimeService>();
        services.AddSingleton<IRngUtil, RngUtil>();

        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IHubGroupUtil, HubGroupUtil>();

        services.AddSingleton<INotificationPacketBuilder, NotificationPacketBuilder>();
        services.AddSingleton<INotificationSessionFactory, NotificationSessionFactory>();
        services.AddSingleton<INotificationSender, NotificationSender>();

        services.AddSingleton<IFacebookClient, FacebookClient>();
        services.AddSingleton<IGoogleClient, GoogleClient>();
        services.AddSingleton<ITwitterClient, TwitterClient>();
        services.AddSingleton<IMicrosoftGraphClient, MicrosoftGraphClient>();

        services.AddHttpClient<ILaPageAMelkorClient, LaPageAMelkorClient>(client =>
        {
            client.BaseAddress = new Uri(_configuration["LaPageAMelkor:Url"]);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "Naheulbook");
        });
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

        var mapImageConfiguration = new MapImageConfiguration();
        _configuration.Bind("MapImage", mapImageConfiguration);
        services.AddSingleton(mapImageConfiguration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseCors(x => x.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowCredentials().AllowAnyHeader());

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
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health");
            endpoints.MapHub<ChangeNotifierHub>("/ws/listen");
            endpoints.MapControllers();
        });

        app.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new {Message = "Invalid route: " + context.Request.Method + " " + context.Request.Path}));
        });
    }
}