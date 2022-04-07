using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ArcaNews_V2.Services;

public class LoggingService
{
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;

    public LoggingService(IServiceProvider services)
    {
        _client = services.GetRequiredService<DiscordSocketClient>();
        _commands = services.GetRequiredService<CommandService>();
        _logger = services.GetRequiredService<ILogger<LoggingService>>();

        _client.Ready += OnReadyAsync;
        _client.Log += OnLogAsync;
        _commands.Log += OnLogAsync;
    }

    /// <summary>
    /// log informations when bot is connected
    /// </summary>
    public Task OnReadyAsync()
    {
        _logger.LogInformation($"Connected as -> [{_client.CurrentUser}]");
        _logger.LogInformation($"Present on [{_client.Guilds.Count}] servers");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Switches out the severity level from Discord.Net's API and logs them
    /// </summary>
    public Task OnLogAsync(LogMessage msg)
    {
        string logText = $"{msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
        switch (msg.Severity.ToString())
        {
            case "Critical":
            {
                _logger.LogCritical(logText);
                break;
            }
            case "Warning":
            {
                _logger.LogWarning(logText);
                break;
            }
            case "Info":
            {
                _logger.LogInformation(logText);
                break;
            }
            case "Verbose":
            {
                _logger.LogInformation(logText);
                break;
            }
            case "Debug":
            {
                _logger.LogDebug(logText);
                break;
            }
            case "Error":
            {
                _logger.LogError(logText);
                break;
            }
        }

        return Task.CompletedTask;
    }
}