using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using ArcaNews_V2.Services;
using ArcaNews_V2.Database;
using Discord.Interactions;
using Serilog;

namespace ArcaNews_V2
{
    class Program
    {
        private InteractionService _commands;
        private static string _logLevel;

        static void Main(string[] args = null)
        {
            if (args.Count() != 0)
            {
                _logLevel = args[0];
            }

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/arcanewsv2.log", rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();

            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            // call ConfigureServices to create the ServiceCollection/Provider for passing around the services
            using (var services = ConfigureServices())
            {
                // get the client and assign to client 
                var client = services.GetRequiredService<DiscordSocketClient>();

                // setup logging and the ready event
                services.GetRequiredService<LoggingService>();

                // this is where we get the Token value from the configuration file, and start the bot
                await client.LoginAsync(TokenType.Bot,
                    Environment.GetEnvironmentVariable("DiscordToken", EnvironmentVariableTarget.User));
                await client.StartAsync();

                // we get the CommandHandler class here and call the InitializeAsync method to start things up for the CommandHandler service
                await services.GetRequiredService<CommandHandler>().InitializeAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }

        /// <summary>
        /// Handles the ServiceCollection creation/configuration
        /// </summary>
        /// <returns>service provider we can call later</returns>
        private ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<LoggingService>()
                .AddLogging(configure => configure.AddSerilog())
                .AddDbContext<ArcaEntities>();

            if (!string.IsNullOrEmpty(_logLevel))
            {
                switch (_logLevel.ToLower())
                {
                    case "info":
                    {
                        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
                        break;
                    }
                    case "error":
                    {
                        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Error);
                        break;
                    }
                    case "debug":
                    {
                        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);
                        break;
                    }
                    default:
                    {
                        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Error);
                        break;
                    }
                }
            }
            else
            {
                services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
            }

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }
}