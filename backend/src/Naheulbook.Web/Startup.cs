using System;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Naheulbook.Core.Features.Calendar;
using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Effect;
using Naheulbook.Core.Features.Event;
using Naheulbook.Core.Features.Fight;
using Naheulbook.Core.Features.God;
using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Item.Actions.Executor;
using Naheulbook.Core.Features.Job;
using Naheulbook.Core.Features.Loot;
using Naheulbook.Core.Features.Map;
using Naheulbook.Core.Features.Merchant;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Core.Features.Npc;
using Naheulbook.Core.Features.Origin;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Features.Skill;
using Naheulbook.Core.Features.Stat;
using Naheulbook.Core.Features.Users;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Factories;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Clients.Facebook;
using Naheulbook.Shared.Clients.Google;
using Naheulbook.Shared.Clients.MicrosoftGraph;
using Naheulbook.Shared.Clients.Twitter;
using Naheulbook.Shared.Extensions;
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

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");
        RegisterConfigurations(services);

        services.AddSingleton<DbContextOptions<NaheulbookDbContext>>(sp =>
            {
                var environment = sp.GetRequiredService<IWebHostEnvironment>();
                var dbContextOptionsBuilder = new DbContextOptionsBuilder<NaheulbookDbContext>()
                    .UseMySQL(configuration.GetConnectionString("DefaultConnection").NotNull(), builder => builder.EnableRetryOnFailure());

                if (environment.IsDevelopment())
                    dbContextOptionsBuilder.EnableSensitiveDataLogging();

                return dbContextOptionsBuilder.Options;
            }
        );

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
            .AddMySql(configuration.GetConnectionString("DefaultConnection").NotNull());

        if (redisConnectionString != null)
        {
            services.AddHealthChecks().AddRedis(redisConnectionString);
            services.AddStackExchangeRedisCache(options => { options.Configuration = redisConnectionString; });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddAutoMapper(typeof(RequestToEntityProfile), typeof(Startup));

        services.AddScoped(servicesProvider => servicesProvider.GetRequiredService<IHttpContextAccessor>().HttpContext!.GetExecutionContext());
        services.AddScoped(servicesProvider => servicesProvider.GetRequiredService<IHttpContextAccessor>().HttpContext!.GetIfExistsExecutionContext());

        services.AddSingleton<IUnitOfWorkFactory>(sp => new UnitOfWorkFactory(sp.GetRequiredService<DbContextOptions<NaheulbookDbContext>>()));

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
        services.AddSingleton<IFightService, FightService>();
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
        services.AddSingleton<IMerchantService, MerchantService>();
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
        services.AddSingleton<IMerchantFactory, MerchantFactory>();

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

        services.AddOptions<LaPageAMelkorClient.Options>()
            .BindConfiguration("LaPageAMelkor")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddHttpClient<ILaPageAMelkorClient, LaPageAMelkorClient>((sp, client) =>
        {
            client.BaseAddress = new Uri(sp.GetRequiredService<IOptions<LaPageAMelkorClient.Options>>().Value.Url);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "Naheulbook");
        });
    }

    private void RegisterConfigurations(IServiceCollection services)
    {
        var mailConfiguration = new MailConfiguration();
        configuration.Bind("Mail", mailConfiguration);
        services.AddSingleton<IMailConfiguration>(mailConfiguration);

        var authenticationConfiguration = new AuthenticationConfiguration();
        configuration.Bind("Authentication", authenticationConfiguration);
        services.AddSingleton<IAuthenticationConfiguration>(authenticationConfiguration);

        var googleConfiguration = new GoogleConfiguration();
        configuration.Bind("Authentication:Google", googleConfiguration);
        services.AddSingleton(googleConfiguration);

        var facebookConfiguration = new FacebookConfiguration();
        configuration.Bind("Authentication:Facebook", facebookConfiguration);
        services.AddSingleton(facebookConfiguration);

        var twitterConfiguration = new TwitterConfiguration();
        configuration.Bind("Authentication:Twitter", twitterConfiguration);
        services.AddSingleton(twitterConfiguration);

        var microsoftConfiguration = new MicrosoftGraphConfiguration();
        configuration.Bind("Authentication:MicrosoftGraph", microsoftConfiguration);
        services.AddSingleton(microsoftConfiguration);

        var mapImageConfiguration = new MapImageConfiguration();
        configuration.Bind("MapImage", mapImageConfiguration);
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