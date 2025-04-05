using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Core.Features.Calendar;
using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Effect;
using Naheulbook.Core.Features.Event;
using Naheulbook.Core.Features.Fight;
using Naheulbook.Core.Features.God;
using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Item;
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
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Clients.Facebook;
using Naheulbook.Shared.Clients.Google;
using Naheulbook.Shared.Clients.MicrosoftGraph;
using Naheulbook.Shared.Clients.Twitter;
using Naheulbook.Shared.Extensions;
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
        services.AddSingleton<DbContextOptions<NaheulbookDbContext>>(sp =>
            {
                var environment = sp.GetRequiredService<IWebHostEnvironment>();
                var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");
                var dbContextOptionsBuilder = new DbContextOptionsBuilder<NaheulbookDbContext>()
                    .UseMySQL(connectionString.NotNull(), builder => builder.EnableRetryOnFailure());

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
            .AddMySql(sp => sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection").NotNull());

        var redisConnectionString = configuration.GetConnectionString("Redis");
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

        services.AddCalendarService();
        services.AddCharacterService();
        services.AddEffectService();
        services.AddEventService();
        services.AddFightService();
        services.AddGodService();
        services.AddGroupService();
        services.AddItemService();
        services.AddJobService();
        services.AddLootService();
        services.AddMapService();
        services.AddMerchantService();
        services.AddMonsterService();
        services.AddNpcService();
        services.AddOriginService();
        services.AddSkillService();
        services.AddStatService();
        services.AddUserService();

        services.AddSharedUtilities();

        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IHubGroupUtil, HubGroupUtil>();
        services.AddOptions<AuthenticationOptions>()
            .BindConfiguration("Authentication")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<INotificationPacketBuilder, NotificationPacketBuilder>();
        services.AddSingleton<INotificationSessionFactory, NotificationSessionFactory>();
        services.AddSingleton<INotificationSender, NotificationSender>();

        services.AddFacebookClient();
        services.AddGoogleClient();
        services.AddMicrosoftGraphClient();
        services.AddTwitterClient();
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