using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace ArcaNews_V2.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public CommandHandler(IServiceProvider services)
        {
            _client = services.GetRequiredService<DiscordSocketClient>();
            _commands = services.GetRequiredService<CommandService>();
            _logger = services.GetRequiredService<ILogger<CommandHandler>>();
            _services = services;

            // Action when a command is executed
            _commands.CommandExecuted += CommandExecutedAsync;

            // action when a message is received so it can process it and check if it's a valid command
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ensure it doesn't process other bots messages
            if (!(rawMessage is SocketUserMessage message))
            {
                return;
            }

            if (message.Source != MessageSource.User)
            {
                return;
            }

            // set argument position away from the prefix
            var argPos = 0;
            var prefix =
                Char.Parse(Environment.GetEnvironmentVariable("DiscordPrefix", EnvironmentVariableTarget.User));

            // check if the message has a valid prefix, and adjust argPos based on prefix
            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) ||
                  message.HasCharPrefix(prefix, ref argPos)))
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);

            // exec command if found
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // Command not found, log it and exit
            if (!command.IsSpecified)
            {
                _logger.LogError(
                    $"Command failed to execute for [{context.User.Username}] <-> [{result.ErrorReason}] !");
                return;
            }

            // command found, log success and exit
            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    $"Command [{command.Value.Name}] executed for [{context.User.Username}] <-> [{context.Guild.Name}]");
                return;
            }

            // failure, inform user
            await context.Channel.SendMessageAsync(
                $"Sorry {context.User.Username}... Something went wrong -> [{result}]");
        }
    }
}