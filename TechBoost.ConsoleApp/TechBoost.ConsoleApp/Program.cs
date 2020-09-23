using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.Contracts;
using Shared.Domain.Contracts.Factories;
using Shared.Domain.Contracts.Helper;
using Shared.Domain.Dto;
using Shared.Services.Factories;
using Shared.Services.Helper;
using System;
using System.Threading.Tasks;
using TechBoost.Domain.Contracts.Application;
using TechBoost.Domain.Contracts.TechBoost;
using TechBoost.Domain.Models;
using TechBoost.Services.TechBoost;

namespace TechBoost.ConsoleApp
{
    class Program
    {
        private static ServiceProvider _serviceProvider;

        /// <summary>
        /// Method to read configurations and register services
        /// </summary>
        private static void Startup()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            IConfiguration configs = configurationBuilder.Build();

            // Bind configurations to models
            UserDetailsConfiguration userDetailsConfiguration = configs.GetSection("UserDetailsConfiguration").Get<UserDetailsConfiguration>();
            ConsoleText consoleTexts = configs.GetSection("ConsoleTexts").Get<ConsoleText>();
            ConnectionStringInfo connectionStringInfo = configs.GetSection("ConnectionStringInfo").Get<ConnectionStringInfo>();

            var services = new ServiceCollection();

            // Singleton services and objects
            services.AddSingleton(s => connectionStringInfo);
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<ITextLineUtilityService, TextLineUtilityService>();
            services.AddSingleton(userDetailsConfiguration);
            services.AddSingleton(consoleTexts);

            // Transient services
            services.AddTransient<IApplication, Application>();

            // Scoped services;
            services.AddScoped<IUserDetailsService, UserDetailsService>();
            services.AddScoped<IBlobServiceFactory, BlobServiceFactory>();

            _serviceProvider = services.BuildServiceProvider(true);
        }
        static async Task Main(string[] args)
        {
            // Read configurations and register services
            Startup();

            // Start the Name Sorter Application
            using (var scope = _serviceProvider.CreateScope())
            {
                var application = scope.ServiceProvider.GetRequiredService<IApplication>();

                await application.Run();
            }
        }
    }
}
