using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PruebaIngresoBibliotecario.Infrastructure;

namespace PruebaIngresoBibliotecario.Api
{
    public class Program
    {
        protected Program()
        {

        }

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PersistenceContext>();
                dbContext.InitializeData(); // inicializar data cuando trabajo con BD en memoria
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((hostBuilderContext, loggerConfig) =>
                {
                    loggerConfig.MinimumLevel.Information()
                        .ReadFrom.Configuration(hostBuilderContext.Configuration)
                        .WriteTo.Console();                        
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
