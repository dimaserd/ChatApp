using Croco.Core.Abstractions.Application;
using CrocoChat.Logic.Services;
using CrocoChat.Model.Contexts;
using CrocoChat.Model.Entities.Clt.Default;
using CrocoChat.Simple.Configuration.Hangfire;
using CrocoChat.Simple.Configuration.Swagger;
using CrocoChat.Simple.CrocoStuff;
using CrocoChat.Simple.Extensions;
using CrocoChat.Simple.Hubs;
using CrocoChat.Simple.Implementations;
using CrocoChat.Simple.Implementations.StateCheckers;
using CrocoShop.CrocoStuff;
using Ecc.Logic.RegistrationModule;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CrocoChat.Simple
{
    public class Startup
    {
        public const string DevelopmentEnvironmentName = "Development";

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Croco = new StartupCroco(new StartUpCrocoOptions
            {
                Configuration = configuration,
                Env = env,
                ApplicationActions = new List<Action<ICrocoApplication>>
                {
                    //Первым должно проверяться состояние миграций
                    CrocoMigrationStateChecker.CheckApplicationState,

                    //Только затем регистрация сервисов
                    EccServiceRegistrator.Register,
                    ApplicationServiceRegistrator.Register
                },
            });
        }

        StartupCroco Croco { get; }
        public IConfiguration Configuration { get; }



        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddJsonOptions(options => ConfigureJsonSerializer(options.JsonSerializerOptions)); ;

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp";
            });

            services.AddTransient<ApplicationUserManager>();
            services.AddTransient<ApplicationSignInManager>();

            services.AddDbContext<ChatDbContext>(options =>
            {
                options.UseSqlServer(
                    Configuration.GetConnectionString(ChatDbContext.ConnectionString));

            });

            // register it
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppClaimsPrincipalFactory>();

            services.AddIdentity<ApplicationUser, ApplicationRole>(opts =>
            {
                opts.Password.RequiredLength = 5;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<ChatDbContext>()
            .AddDefaultTokenProviders();

            SwaggerConfiguration.ConfigureSwagger(services, new List<string>
            {
            });


            services.AddSignalR().AddJsonProtocol(options => {
                ConfigureJsonSerializer(options.PayloadSerializerOptions);
            });

            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString(ChatDbContext.ConnectionString)));

            //Установка приложения
            Croco.SetCrocoApplication(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapHub<MessagingHub>("/messagingHub");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
            });


            app.ConfigureExceptionHandler(new ApplicationLoggerManager());

            HangfireConfiguration.AddHangfire(app, env.EnvironmentName == DevelopmentEnvironmentName);
        }

        private static void ConfigureJsonSerializer(JsonSerializerOptions settings)
        {
            settings.PropertyNameCaseInsensitive = true;
            settings.PropertyNamingPolicy = null;
            settings.Converters.Add(new JsonStringEnumConverter());
        }
    }
}